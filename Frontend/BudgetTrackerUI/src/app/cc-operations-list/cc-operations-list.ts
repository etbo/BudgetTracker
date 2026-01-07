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
import { MatDialog, MatDialogModule } from '@angular/material/dialog';

import { OperationsService } from '../services/operations.service';
import { RulesService } from '../services/rules.service';
import { CcOperation } from '../models/operation-cc.model';
import { FilterState, filtersService } from '../services/filters.service';
import { customDateFormatter } from '../shared/utils/grid-utils';

ModuleRegistry.registerModules([AllCommunityModule]);

@Component({
  standalone: true,
  imports: [MatDialogModule, MatButtonModule],
  template: `
    <h2 mat-dialog-title>Confirmer la sauvegarde</h2>
    <mat-dialog-content>
      Voulez-vous sauvegarder les catégories suggérées pour les opérations actuellement affichées ?
    </mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-button mat-dialog-close>Annuler</button>
      <button mat-raised-button color="primary" [mat-dialog-close]="true">Confirmer</button>
    </mat-dialog-actions>
  `
})
export class ConfirmDialogComponent { }

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
  imports: [CommonModule, AgGridModule, MatProgressSpinnerModule, FormsModule, MatIconModule, MatButtonModule],
  templateUrl: './cc-operations-list.html',
  styleUrl: './cc-operations-list.scss',
})
export class CcOperationsList implements OnInit, OnDestroy, OnChanges {
  private gridApi!: GridApi;

  @Input() customFilters?: FilterState;
  @Input() pageSize: number = 50;
  @Input() domLayout: 'autoHeight' | 'normal' = 'autoHeight';
  @Input() sortColumn: string = 'date';

  resultatOperations = signal<CcOperation[]>([]);
  isLoading = signal(false);
  showSaveAll = signal(false); // Signal pour piloter l'affichage du bouton global

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
    if (!this.customFilters) {
      this.loadData(event.detail);
    }
  };

  constructor(
    private opService: OperationsService,
    private rulesService: RulesService,
    private dialog: MatDialog
  ) { }

  ngOnInit() {
    window.addEventListener('filterChanged', this.filterListener);
    this.rulesService.getCcCategories().subscribe(cats => {
      const catCol = this.columnDefs.find(c => c.field === 'categorie');
      if (catCol) {
        catCol.cellEditorParams = { values: ['', ...cats.map(c => c.name)] };
        this.gridApi?.setGridOption('columnDefs', [...this.columnDefs]);
      }
    });
    this.loadData(this.customFilters || filtersService.getFilters());
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['customFilters'] && this.customFilters) {
      this.loadData(this.customFilters);
    }
    if (changes['sortColumn'] || changes['sortDirection']) {
      this.applySorting();
    }
  }

  // --- Gestion de la visibilité du bouton global ---
  updateSaveAllVisibility() {
    if (!this.gridApi) return;
    let found = false;
    this.gridApi.forEachNodeAfterFilter((node) => {
      if (node.data?.isModified) found = true;
    });
    this.showSaveAll.set(found);
  }

  onFilterChanged() { this.updateSaveAllVisibility(); }
  onModelUpdated() { this.updateSaveAllVisibility(); }

  onGridReady(params: GridReadyEvent) {
    this.gridApi = params.api;
    this.applySorting();
    this.updateSaveAllVisibility();
  }

  private applySorting() {
    this.columnDefs = this.columnDefs.map(col => ({
      ...col,
      sort: col.field === this.sortColumn ? 'asc' : null
    }));
    if (this.gridApi) {
      this.gridApi.setGridOption('columnDefs', this.columnDefs);
    }
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
        this.updateSaveAllVisibility();
      }
    });
  }

  onCellValueChanged(event: any) {
    const field = event.colDef.field;
    const op = event.data;

    if (field === 'categorie') {
      const newValue = event.newValue?.trim();

      if (!newValue || newValue === '') {
        // 1. SUPPRESSION RÉELLE : On met à jour la base avec null
        this.opService.updateOperation({ ...op, categorie: null }).subscribe({
          next: () => {
            // 2. RECHERCHE DE SUGGESTION : Une fois que c'est vide en base, on cherche une règle
            this.opService.getSuggestion(op).subscribe(res => {
              if (res.isSuggested) {
                op.categorie = res.categorie;
                op.isSuggested = true;
                op.isModified = true; // S'affiche en jaune pour indiquer la proposition
              } else {
                op.categorie = null;
                op.isSuggested = false;
                op.isModified = false;
              }

              // Mise à jour de l'affichage AG Grid
              this.gridApi.applyTransaction({ update: [op] });
              this.updateSaveAllVisibility();
            });
          },
          error: (err) => console.error("Erreur lors de la suppression :", err)
        });
      } else {
        // Choix manuel : sauvegarde directe
        op.isModified = false;
        op.isSuggested = false;
        this.save(op);
      }
    }
    else if (field === 'Comment') {
      this.opService.updateOperation({ id: op.id, Comment: event.newValue } as CcOperation).subscribe();
    }
  }

  saveAllSuggested() {
    const operationsToSave: CcOperation[] = [];
    this.gridApi.forEachNodeAfterFilter((node) => {
      if (node.data.isModified) operationsToSave.push(node.data);
    });

    if (operationsToSave.length === 0) return;

    this.dialog.open(ConfirmDialogComponent).afterClosed().subscribe(result => {
      if (result === true) this.executeBulkSave(operationsToSave);
    });
  }

  private executeBulkSave(operationsToSave: CcOperation[]) {
    this.isLoading.set(true);
    import('rxjs').then(({ forkJoin }) => {
      const requests = operationsToSave.map(op => this.opService.updateOperation(op));
      forkJoin(requests).subscribe({
        next: () => {
          operationsToSave.forEach(op => op.isModified = false);
          this.gridApi.applyTransaction({ update: operationsToSave });
          this.updateSaveAllVisibility();
          this.isLoading.set(false);
        },
        error: () => this.isLoading.set(false)
      });
    });
  }

  ngOnDestroy() {
    window.removeEventListener('filterChanged', this.filterListener);
  }

  getRowId = (params: any) => params.data.id.toString();
}