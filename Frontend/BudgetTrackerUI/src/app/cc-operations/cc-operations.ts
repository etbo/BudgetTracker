import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';

import { AgGridModule, ICellRendererAngularComp } from 'ag-grid-angular';
import {
  ModuleRegistry,
  AllCommunityModule,
  ValueFormatterParams,
  GridApi,
  GridReadyEvent
} from 'ag-grid-community';

import { MatSelectModule } from '@angular/material/select';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';

import { OperationsService } from '../services/operations.service';
import { RulesService } from '../services/rules.service';
import { CcOperation } from '../models/operation-cc.model';
import { filtersService } from '../services/filters.service';
import { DateFilter } from '../date-filter/date-filter';

ModuleRegistry.registerModules([AllCommunityModule]);

@Component({
  standalone: true,
  imports: [CommonModule, MatButtonModule, MatIconModule],
  template: `
    <div class="flex justify-center items-center h-full">
      <button mat-icon-button color="primary" *ngIf="params.data.isModified" (click)="onSave()">
        <mat-icon>save_alt</mat-icon>
      </button>
    </div>
  `
})
export class SaveButtonRenderer implements ICellRendererAngularComp {
  params: any;
  agInit(params: any): void { this.params = params; }
  refresh(params: any): boolean { this.params = params; return true; }
  onSave() { this.params.context.componentParent.save(this.params.data); }
}

@Component({
  selector: 'app-cc-operations',
  standalone: true,
  imports: [
    CommonModule, FormsModule, AgGridModule,
    MatSelectModule, MatInputModule, MatButtonModule,
    MatIconModule, MatProgressSpinnerModule, MatCardModule, MatChipsModule,
    DateFilter
  ],
  templateUrl: './cc-operations.html',
  styleUrls: ['./cc-operations.scss']
})
export class CcOperations implements OnInit {
  private gridApi!: GridApi;

  // État des données
  resultatOperations = signal<CcOperation[]>([]);
  isLoading = signal(false);

  // Filtres (Synchronisés avec l'URL via ngOnInit)
  currentMode = 'C';
  searchString = '';
  filterMissingCat = false;
  filterOnlyCheques = false;
  filterSuggestedCat = false;

  gridContext = { componentParent: this };
  defaultColDef = { resizable: true, sortable: true, filter: true };

  columnDefs: any[] = [
    { headerName: 'Date', field: 'date', width: 110, sort: 'desc' },
    { headerName: 'Description', field: 'description', flex: 2 },
    {
      headerName: 'Montant',
      field: 'montant',
      width: 110,
      valueFormatter: (p: ValueFormatterParams) => p.value?.toFixed(2) + ' €',
      cellClassRules: {
        'text-red-600': (p: any) => p.value < 0,
        'text-green-600': (p: any) => p.value >= 0
      }
    },
    {
      headerName: 'Catégorie',
      field: 'categorie',
      editable: true,
      cellEditor: 'agSelectCellEditor',
      cellEditorParams: { values: [] },
      width: 180,
      cellClassRules: { 'bg-yellow-100 italic': (p: any) => p.data.isModified }
    },
    {
      headerName: '',
      width: 60,
      cellRenderer: SaveButtonRenderer,
      sortable: false, filter: false,
      cellClassRules: { 'bg-yellow-100-bis': (p: any) => p.data.isModified }
    },
    { headerName: 'Comment', field: 'Comment', editable: true, flex: 1 },
    { headerName: 'Banque', field: 'banque', width: 120, cellClass: 'text-gray-400 text-sm' }
  ];

  constructor(
    private opService: OperationsService,
    private rulesService: RulesService,
    private http: HttpClient
  ) { }

  ngOnInit() {
    // 1. Initialisation de l'état depuis l'URL au chargement de la page
    const initFilters = filtersService.getFilters();
    this.currentMode = initFilters.view || 'C';
    this.filterMissingCat = !!initFilters.missingCat;
    this.filterOnlyCheques = !!initFilters.onlyCheques;

    // 2. Écouteur unique : Toute modification d'URL déclenche le rechargement
    window.addEventListener('filterChanged', (event: any) => {
      const f = event.detail;
      this.currentMode = f.view || 'C';
      this.filterMissingCat = !!f.missingCat;
      this.filterOnlyCheques = !!f.onlyCheques;
      
      this.loadData(); 
    });

    // 3. Charger les catégories pour l'éditeur AG Grid
    this.rulesService.getCcCategories().subscribe(cats => {
      const catCol = this.columnDefs.find(c => c.field === 'categorie');
      if (catCol) {
        catCol.cellEditorParams = { values: ['', ...cats.map(c => c.name)] };
      }
    });

    // 4. Lancement du premier chargement
    this.loadData();
  }

  onGridReady(params: GridReadyEvent) {
    this.gridApi = params.api;
  }

  loadData() {
    this.isLoading.set(true);
    const f = filtersService.getFilters();

    // Construction de l'URL pour le Backend C#
    let url = `http://localhost:5000/api/operations?mode=${this.currentMode}`;

    if (this.filterMissingCat) url += `&missingCat=true`;
    if (this.filterOnlyCheques) url += `&onlyCheques=true`;
    if (this.filterSuggestedCat) url += `&suggestedCat=true`;

    // On n'envoie les dates que si le mode est 'custom'
    if (this.currentMode === 'custom' && f.start && f.end) {
      url += `&startDate=${f.start}&endDate=${f.end}`;
    }

    this.http.get<CcOperation[]>(url).subscribe({
      next: (data) => {
        this.resultatOperations.set(data);
        this.isLoading.set(false);
      },
      error: (err) => {
        console.error("Erreur API:", err);
        this.isLoading.set(false);
      }
    });
  }

  // Actions utilisateur (mettent à jour l'URL, l'event listener fait le reste)
  onModeChange() {
    filtersService.updateFilters({ view: this.currentMode });
  }

  toggleExtraFilter(type: 'missing' | 'suggested' | 'cheque') {
    if (type === 'missing') this.filterMissingCat = !this.filterMissingCat;
    if (type === 'cheque') this.filterOnlyCheques = !this.filterOnlyCheques;
    if (type === 'suggested') this.filterSuggestedCat = !this.filterSuggestedCat;

    filtersService.updateFilters({
      missingCat: this.filterMissingCat,
      suggestedCat: this.filterSuggestedCat,
      onlyCheques: this.filterOnlyCheques,
      view: this.currentMode
    });
  }

  save(op: CcOperation) {
    this.opService.updateOperation(op).subscribe({
      next: () => {
        op.isModified = false;
        this.gridApi.applyTransaction({ update: [op] });
        this.resultatOperations.update(currentOps => [...currentOps]);
      },
      error: (err) => console.error("Erreur sauvegarde:", err)
    });
  }

  onCellValueChanged(event: any) {
    const field = event.colDef.field;
    const op = event.data;

    if (field === 'Comment') {
      const partialUpdate = { id: op.id, Comment: event.newValue } as CcOperation;
      this.opService.updateOperation(partialUpdate).subscribe();
    } else if (field === 'categorie') {
      op.isModified = false;
      this.save(op);
    }
  }

  onQuickFilterChanged() {
    this.gridApi.setGridOption('quickFilterText', this.searchString);
  }

  getRowId = (params: any) => params.data.id.toString();
}