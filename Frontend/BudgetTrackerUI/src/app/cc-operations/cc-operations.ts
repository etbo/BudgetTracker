import { Component, OnInit, OnDestroy, signal } from '@angular/core';
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
import { DateFilter } from '../date-filter/date-filter'; // Vérifie le chemin exact
import { customDateFormatter } from '../shared/utils/grid-utils';

ModuleRegistry.registerModules([AllCommunityModule]);

// --- Rendu du bouton Sauvegarder (Restauré) ---
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
export class CcOperations implements OnInit, OnDestroy {
  private gridApi!: GridApi;

  // État des données
  resultatOperations = signal<CcOperation[]>([]);
  isLoading = signal(false);

  // État des filtres
  currentMode = 'last';
  searchString = '';
  filterMissingCat = false;
  filterOnlyCheques = false;
  filterSuggestedCat = false;

  gridContext = { componentParent: this };
  defaultColDef = { resizable: true, sortable: true, filter: true };

  // --- Configuration des colonnes (Restaurée à l'identique) ---
  columnDefs: any[] = [
    { headerName: 'Date', field: 'date', width: 130, sort: 'desc', valueFormatter: customDateFormatter },
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
      cellClassRules: { 
        'bg-yellow-100 italic': (p: any) => p.data.isModified,
        'bg-blue-50': (p: any) => p.data.isSuggested // Ajout si tu as un flag suggestion
      }
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

  private filterListener = (event: any) => {
    const f = event.detail;
    this.currentMode = f.view || 'last';
    this.filterMissingCat = !!f.missingCat;
    this.filterOnlyCheques = !!f.onlyCheques;
    this.filterSuggestedCat = !!f.suggestedCat;
    this.loadData();
  };

  constructor(
    private opService: OperationsService,
    private rulesService: RulesService,
    private http: HttpClient
  ) { }

  ngOnInit() {
    const initFilters = filtersService.getFilters();
    this.currentMode = initFilters.view || 'last';
    this.filterMissingCat = !!initFilters.missingCat;
    this.filterOnlyCheques = !!initFilters.onlyCheques;
    this.filterSuggestedCat = !!initFilters.suggestedCat;

    if (!window.location.search) {
      this.syncUrl();
    }

    window.addEventListener('filterChanged', this.filterListener);

    // Chargement des catégories pour le select de la grille
    this.rulesService.getCcCategories().subscribe(cats => {
      const catCol = this.columnDefs.find(c => c.field === 'categorie');
      if (catCol) {
        catCol.cellEditorParams = { values: ['', ...cats.map(c => c.name)] };
        this.gridApi?.setGridOption('columnDefs', [...this.columnDefs]);
      }
    });

    this.loadData();
  }

  ngOnDestroy() {
    window.removeEventListener('filterChanged', this.filterListener);
  }

  onGridReady(params: GridReadyEvent) {
    this.gridApi = params.api;
  }

  loadData() {
    this.isLoading.set(true);
    const f = filtersService.getFilters();
    this.opService.getOperations(f).subscribe({
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

  // --- Actions provenant du nouveau DateFilter ---
  onFilterChanged(event: { start: string, end: string }) {
    let view = 'custom';
    if (event.start === 'last') view = 'last';
    else if (!event.start) view = 'all';

    filtersService.updateFilters({
      start: event.start === 'last' ? undefined : event.start,
      end: event.end === 'last' ? undefined : event.end,
      view: view
    });
  }

  toggleExtraFilter(type: 'missing' | 'suggested' | 'cheque') {
    const f = filtersService.getFilters();
    const update: any = { view: this.currentMode };
    
    if (type === 'missing') update.missingCat = !f.missingCat;
    if (type === 'suggested') update.suggestedCat = !f.suggestedCat;
    if (type === 'cheque') update.onlyCheques = !f.onlyCheques;

    filtersService.updateFilters(update);
  }

  save(op: CcOperation) {
    this.opService.updateOperation(op).subscribe({
      next: () => {
        op.isModified = false;
        this.gridApi.applyTransaction({ update: [op] });
        // On force le rafraîchissement du signal pour l'UI
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
      op.isModified = false; // On considère que le select valide direct ou tu peux mettre à true
      this.save(op);
    }
  }

  onQuickFilterChanged() {
    this.gridApi.setGridOption('quickFilterText', this.searchString);
  }

  getRowId = (params: any) => params.data.id.toString();

  syncUrl() {
    filtersService.updateFilters({
      view: this.currentMode,
      missingCat: this.filterMissingCat,
      onlyCheques: this.filterOnlyCheques,
      suggestedCat: this.filterSuggestedCat
    });
  }

  onImport() {
    // Logique d'import à implémenter
  }
}