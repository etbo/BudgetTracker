import { Component, OnInit, OnDestroy, OnChanges, SimpleChanges, Input, signal, Inject, Output, EventEmitter } from '@angular/core';
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
import { MAT_DIALOG_DATA, MatDialog, MatDialogModule, MatDialogRef } from '@angular/material/dialog';

import { OperationsService } from '../services/operations.service';
import { RulesService } from '../services/rules.service';
import { CcOperation } from '../models/operation-cc.model';
import { FilterState, filtersService } from '../services/filters.service';
import { customDateFormatter } from '../shared/utils/grid-utils';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';

import { Subscription } from 'rxjs';

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
      <button mat-icon-button color="primary" *ngIf="params.data.isSuggested" (click)="onSave()">
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
  standalone: true,
  imports: [CommonModule, MatDialogModule, MatFormFieldModule, MatInputModule, MatSelectModule, FormsModule, MatButtonModule],
  template: `
    <h2 mat-dialog-title>Créer une règle automatique</h2>
    <mat-dialog-content class="flex flex-col gap-4 pt-4">
      
      <mat-form-field appearance="outline" class="w-full">
        <mat-label>Mot-clé détecté</mat-label>
        <input matInput [(ngModel)]="data.keyword" cdkFocusInitial>
      </mat-form-field>
      
      <mat-form-field appearance="outline" class="w-full">
        <mat-label>Category à associer</mat-label>
        <mat-select [(ngModel)]="data.category">
          <mat-option *ngFor="let cat of categories" [value]="cat.name">
            {{cat.name}}
          </mat-option>
        </mat-select>
      </mat-form-field>
    </mat-dialog-content>

    <mat-dialog-actions align="end">
      <button mat-button mat-dialog-close>Annuler</button>
      <button mat-raised-button color="primary" [mat-dialog-close]="data" [disabled]="!data.keyword || !data.category">
        Enregistrer la règle
      </button>
    </mat-dialog-actions>
  `,
  styles: [`
  .w-full { width: 100%; margin-top: 20px}
`]
})
export class CreateRuleDialog {
  categories: any[] = [];

  constructor(
    public dialogRef: MatDialogRef<CreateRuleDialog>,
    @Inject(MAT_DIALOG_DATA) public data: { keyword: string, category: string },
    private rulesService: RulesService
  ) {
    // On charge les catégories pour le menu déroulant
    this.rulesService.getCcCategories().subscribe(cats => this.categories = cats);
  }

  onNoClick(): void {
    this.dialogRef.close();
  }
}

@Component({
  selector: 'app-cc-operations-list',
  standalone: true,
  imports: [CommonModule, FormsModule, AgGridModule, MatProgressSpinnerModule, FormsModule, MatIconModule, MatButtonModule, MatInputModule],
  templateUrl: './cc-operations-list.html',
  styleUrl: './cc-operations-list.scss',
})
export class CcOperationsList implements OnInit, OnDestroy, OnChanges {
  private filterSub?: Subscription;
  private gridApi!: GridApi;
  private isInitialising = false;

  @Input() customFilters?: FilterState;
  @Input() pageSize: number = 50;
  @Input() domLayout: 'autoHeight' | 'normal' = 'autoHeight';
  @Input() sortColumn: string = 'date';
  @Input() SaveAllSuggestedButton: boolean = true;
  @Input() operations: CcOperation[] = [];
  @Input() externalSearch: string = '';

  // Faire remonter le besoin de refresh après une modification
  @Output() refresh = new EventEmitter<void>();

  searchString = '';

  isLoading = signal(false);
  showSaveAll = signal(false); // Signal pour piloter l'affichage du bouton global

  gridContext = { componentParent: this };
  defaultColDef = {
    resizable: true, sortable: true, filter: true,
    filterParams: {
      buttons: ['clear']
    }
  };

  columnDefs: any[] = [
    { headerName: 'Date', field: 'date', width: 130, valueFormatter: customDateFormatter },
    { headerName: 'Description', field: 'label', flex: 2 },
    {
      headerName: 'Montant',
      type: 'rightAligned',
      field: 'amount',
      width: 150,
      valueFormatter: (p: ValueFormatterParams) => p.value?.toFixed(2) + ' €',
      cellClassRules: {
        'text-red-600': (p: any) => p.value < 0,
        'text-green-600': (p: any) => p.value >= 0
      }
    },
    {
      headerName: 'Catégorie',
      field: 'category',
      editable: true,
      cellEditor: 'agSelectCellEditor',
      singleClickEdit: true,
      cellEditorParams: { values: [] },
      width: 180,
      cellClassRules: {
        'bg-yellow-100 italic': (p: any) => p.data.isSuggested,
        'bg-blue-50': (p: any) => p.data.isSuggested
      }
    },
    {
      headerName: '',
      width: 60,
      cellRenderer: SaveButtonRenderer,
      sortable: false, filter: false,
      cellClassRules: { 'bg-yellow-100-no-border': (p: any) => p.data.isSuggested }
    },
    { headerName: 'Commentaire', field: 'comment', editable: true, flex: 1 },
    {
      headerName: 'Banque', field: 'banque', width: 120, cellClass: 'text-gray-400 text-sm',
      filterParams: { filterOptions: ['contains'], maxNumConditions: 1, debounceMs: 200 }
    }
  ];

  constructor(
    private opService: OperationsService,
    private rulesService: RulesService,
    private dialog: MatDialog
  ) { }

  ngOnInit() {

    // Chargement des catégories pour le select
    this.rulesService.getCcCategories().subscribe(cats => {
      const catCol = this.columnDefs.find(c => c.field === 'category');
      if (catCol) {
        catCol.cellEditorParams = { values: ['', ...cats.map(c => c.name)] };
        this.gridApi?.setGridOption('columnDefs', [...this.columnDefs]);
      }
    });
  }

  ngOnChanges(changes: SimpleChanges) {
    // Si les opérations changent (depuis le parent CcOperations)
    if (changes['operations'] && this.gridApi) {
      this.gridApi.setGridOption('rowData', this.operations);
      this.updateSaveAllVisibility();
    }

    if (changes['externalSearch'] && this.gridApi) {
      this.gridApi.setGridOption('quickFilterText', this.externalSearch);
    }
  }

  // --- Gestion de la visibilité du bouton global ---
  updateSaveAllVisibility() {
    if (this.isInitialising) return;
    if (!this.gridApi) return;
    if (!this.SaveAllSuggestedButton) return;
    let found = false;
    this.gridApi.forEachNodeAfterFilter((node) => {
      if (node.data?.isSuggested) found = true;
    });
    this.showSaveAll.set(found);
  }

  onFilterChanged() { this.updateSaveAllVisibility(); }
  onModelUpdated() { this.updateSaveAllVisibility(); }

  onGridReady(params: GridReadyEvent) {
    this.isInitialising = true;
    this.gridApi = params.api;
    if (this.externalSearch) {
      this.gridApi.setGridOption('quickFilterText', this.externalSearch);
    }
    this.applySorting();
    this.isInitialising = false;
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

  save(op: CcOperation) {
    this.opService.updateOperation(op).subscribe({
      next: () => {
        op.isSuggested = false;
        this.gridApi.applyTransaction({ update: [op] });
        this.updateSaveAllVisibility();

        this.refresh.emit();
      }
    });
  }

  onCellValueChanged(event: any) {
    const field = event.colDef.field;
    const op = event.data;

    if (field === 'category') {
      const newValue = event.newValue?.trim();

      if (!newValue || newValue === '') {
        // 1. SUPPRESSION RÉELLE : On met à jour la base avec null
        this.opService.updateOperation({ ...op, category: null }).subscribe({
          next: () => {
            // 2. RECHERCHE DE SUGGESTION : Une fois que c'est vide en base, on cherche une règle
            this.opService.getSuggestion(op).subscribe(res => {
              if (res.isSuggested) {
                op.category = res.category;
                op.isSuggested = true;
                op.isSuggested = true; // S'affiche en jaune pour indiquer la proposition
              } else {
                op.category = null;
                op.isSuggested = false;
                op.isSuggested = false;
              }

              // Mise à jour de l'affichage AG Grid
              this.gridApi.applyTransaction({ update: [op] });
              this.updateSaveAllVisibility();

              this.refresh.emit();
            });
          },
          error: (err) => console.error("Erreur lors de la suppression :", err)
        });
      } else {
        // Choix manuel : sauvegarde directe
        op.isSuggested = false;
        op.isSuggested = false;
        this.save(op);
      }
    }
    else if (field === 'Comment') {
      this.opService.updateOperation({ id: op.id, comment: event.newValue } as CcOperation).subscribe();
    }
  }

  saveAllSuggested() {
    const operationsToSave: CcOperation[] = [];
    this.gridApi.forEachNodeAfterFilter((node) => {
      if (node.data.isSuggested) operationsToSave.push(node.data);
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
          operationsToSave.forEach(op => op.isSuggested = false);
          this.gridApi.applyTransaction({ update: operationsToSave });
          this.updateSaveAllVisibility();
          this.isLoading.set(false);

          this.refresh.emit();
        },
        error: () => this.isLoading.set(false)
      });
    });
  }

  ngOnDestroy() {
    this.filterSub?.unsubscribe();
  }

  getRowId = (params: any) => params.data.id.toString();

  onCellContextMenu(event: any) {
    // On cible uniquement la colonne Description
    if (event.column.getColId() !== 'description') return;

    // Empêche le menu contextuel du navigateur
    event.event.preventDefault();

    const description = event.value;
    const currentCategory = event.data.category;

    const dialogRef = this.dialog.open(CreateRuleDialog, {
      width: '800px',
      maxWidth: '90vw',
      data: {
        keyword: description,
        category: currentCategory
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        // On utilise ta méthode 'create' déjà existante dans RulesService
        this.rulesService.create({
          pattern: result.keyword,
          category: result.category,
          isUsed: true
        }).subscribe({
          next: () => {
            this.refresh.emit();
          }
        });
      }
    });
  }
}
