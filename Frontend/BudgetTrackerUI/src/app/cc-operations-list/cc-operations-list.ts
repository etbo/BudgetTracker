import { Component, OnInit, OnDestroy, OnChanges, SimpleChanges, Input, signal } from '@angular/core';
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

import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

import { OperationsService } from '../services/operations.service';
import { RulesService } from '../services/rules.service';
import { CcOperation } from '../models/operation-cc.model';
import { FilterState, filtersService } from '../services/filters.service';
import { customDateFormatter } from '../shared/utils/grid-utils';

ModuleRegistry.registerModules([AllCommunityModule]);

// --- Renderer pour le bouton de sauvegarde ---
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
  selector: 'app-cc-operations-list',
  standalone: true,
  imports: [CommonModule, AgGridModule, MatProgressSpinnerModule, FormsModule],
  templateUrl: './cc-operations-list.html',
  styleUrl: './cc-operations-list.scss',
})
export class CcOperationsList implements OnInit, OnDestroy, OnChanges {
  private gridApi!: GridApi;

  // --- Paramètres de configuration (Inputs) ---
  @Input() customFilters?: FilterState; // Si présent, ignore le service global (ex: Zoom Dashboard)
  @Input() pageSize: number = 50;
  @Input() domLayout: 'autoHeight' | 'normal' = 'autoHeight';
  @Input() sortColumn: string = 'date';

  // État des données
  resultatOperations = signal<CcOperation[]>([]);
  isLoading = signal(false);

  gridContext = { componentParent: this };
  defaultColDef = { resizable: true, sortable: true, filter: true };

  columnDefs: any[] = [
    { headerName: 'Date', field: 'date', width: 130, valueFormatter: customDateFormatter },
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
        'bg-blue-50': (p: any) => p.data.isSuggested
      }
    },
    {
      headerName: '',
      width: 60,
      cellRenderer: SaveButtonRenderer,
      sortable: false, filter: false,
      cellClassRules: { 'bg-yellow-100-no-border': (p: any) => p.data.isModified }
    },
    { headerName: 'Comment', field: 'Comment', editable: true, flex: 1 },
    {
      headerName: 'Banque', field: 'banque', width: 120, cellClass: 'text-gray-400 text-sm',
      filterParams: { filterOptions: ['contains'], maxNumConditions: 1, debounceMs: 200 }
    }
  ];

  private filterListener = (event: any) => {
    // On ne recharge via le service global que si on n'est pas en mode "filtres forcés" (Input)
    if (!this.customFilters) {
      this.loadData(event.detail);
    }
  };

  constructor(
    private opService: OperationsService,
    private rulesService: RulesService
  ) { }

  ngOnInit() {
    // Écoute les changements globaux (URL/Chips)
    window.addEventListener('filterChanged', this.filterListener);

    // Initialisation des catégories
    this.rulesService.getCcCategories().subscribe(cats => {
      const catCol = this.columnDefs.find(c => c.field === 'categorie');
      if (catCol) {
        catCol.cellEditorParams = { values: ['', ...cats.map(c => c.name)] };
        this.gridApi?.setGridOption('columnDefs', [...this.columnDefs]);
      }
    });

    // Premier chargement
    this.loadData(this.customFilters || filtersService.getFilters());

    this.applySorting();
  }

  ngOnChanges(changes: SimpleChanges) {
    // Si l'Input customFilters change (ex: Zoom Dashboard), on recharge
    if (changes['customFilters'] && this.customFilters) {
      this.loadData(this.customFilters);

      if (changes['sortColumn'] || changes['sortDirection']) {
        this.applySorting();
      }
    }
  }

  private applySorting() {
    this.columnDefs = this.columnDefs.map(col => {
      // On réinitialise le tri sur toutes les colonnes
      const newCol = { ...col, sort: null };
      // On applique le tri uniquement sur la colonne demandée
      if (col.field === this.sortColumn) {
        newCol.sort = 'asc';
      }
      return newCol;
    });

    // Si la grille est déjà prête, on lui pousse les nouvelles defs
    if (this.gridApi) {
      this.gridApi.setGridOption('columnDefs', this.columnDefs);
    }
  }

  ngOnDestroy() {
    window.removeEventListener('filterChanged', this.filterListener);
  }

  onGridReady(params: GridReadyEvent) {
    this.gridApi = params.api;
  }

  loadData(f: FilterState) {
    this.isLoading.set(true);
    this.opService.getOperations(f).subscribe({
      next: (data) => {
        this.resultatOperations.set(data);
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  save(op: CcOperation) {
    this.opService.updateOperation(op).subscribe({
      next: () => {
        op.isModified = false;
        this.gridApi.applyTransaction({ update: [op] });
        this.resultatOperations.update(current => [...current]);
      }
    });
  }

  onCellValueChanged(event: any) {
    const field = event.colDef.field;
    const op = event.data;

    if (field === 'Comment') {
      this.opService.updateOperation({ id: op.id, Comment: event.newValue } as CcOperation).subscribe();
    } else if (field === 'categorie') {
      this.save(op);
    }
  }

  getRowId = (params: any) => params.data.id.toString();
}