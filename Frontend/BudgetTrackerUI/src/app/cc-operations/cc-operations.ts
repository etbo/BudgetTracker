import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { AgGridModule } from 'ag-grid-angular';
import { ICellRendererAngularComp } from 'ag-grid-angular';
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

import { OperationsService } from '../services/operations.service';
import { RulesService } from '../services/rules.service';
import { OperationCC } from '../models/operation-cc.model';
import { MatCardModule } from '@angular/material/card';

ModuleRegistry.registerModules([AllCommunityModule]);

/* --- Renderer pour le bouton de validation des suggestions --- */
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
  refresh(params: any): boolean {
    this.params = params;
    return true;
  }
  onSave() {
    this.params.context.componentParent.save(this.params.data);
  }
}

@Component({
  selector: 'app-cc-operations',
  standalone: true,
  imports: [
    CommonModule, FormsModule, AgGridModule,
    MatSelectModule, MatInputModule, MatButtonModule,
    MatIconModule, MatProgressSpinnerModule, MatCardModule
  ],
  templateUrl: './cc-operations.html',
  styleUrls: ['./cc-operations.scss']
})
export class CcOperations implements OnInit {
  private gridApi!: GridApi;

  resultatOperations = signal<OperationCC[]>([]);
  isLoading = signal(false);
  filterType = 'C';
  searchString = '';
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
      cellClassRules: {
        'bg-yellow-100 italic': (p: any) => p.data.isModified // Jaune si suggestion backend
      }
    },
    {
      headerName: '',
      width: 60,
      cellRenderer: SaveButtonRenderer,
      sortable: false, filter: false,
      cellClassRules: {
        'bg-yellow-100-bis': (p: any) => p.data.isModified // Jaune si suggestion backend
      }
    },
    { headerName: 'Commentaire', field: 'commentaire', editable: true, flex: 1 },
    { headerName: 'Banque', field: 'banque', width: 120, cellClass: 'text-gray-400 text-sm' }
  ];

  constructor(private opService: OperationsService, private rulesService: RulesService) { }

  ngOnInit() {
    this.rulesService.getCategories().subscribe(cats => {
      const catCol = this.columnDefs.find(c => c.field === 'categorie');
      if (catCol) {
        catCol.cellEditorParams = { values: ['', ...cats.map(c => c.name)] };
      }
    });
    this.refresh();
  }

  onGridReady(params: GridReadyEvent) {
    this.gridApi = params.api;
  }

  refresh() {
    this.isLoading.set(true);
    this.opService.getOperations(this.filterType).subscribe({
      next: (data) => {
        this.resultatOperations.set(data);
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  save(op: OperationCC) {
    this.opService.updateOperation(op).subscribe({
      next: () => {
        // 1. On met à jour l'objet localement
        op.isModified = false;

        this.gridApi.applyTransaction({ update: [op] });

        // 2. On déclenche la mise à jour immuable du signal
        // Cela crée une nouvelle référence de tableau, AG Grid détecte le changement
        this.resultatOperations.update(currentOps => [...currentOps]);

        // Note : Pas besoin de refreshCells ou de Transactions ici, 
        // la nouvelle référence du signal s'en occupe via le binding [rowData]
      },
      error: (err) => {
        console.error("Erreur lors de la sauvegarde :", err);
        // Optionnel : ajouter une notification d'erreur ici
      }
    }
    );
  }

  onCellValueChanged(event: any) {
    const field = event.colDef.field;
    const op = event.data;

    if (field === 'commentaire') {
      // SCÉNARIO A : On modifie le commentaire
      // On crée un objet temporaire avec uniquement l'ID et le nouveau commentaire
      const partialUpdate = {
        id: op.id,
        commentaire: event.newValue
      } as OperationCC;

      this.opService.updateOperation(partialUpdate).subscribe({
        next: () => {
          // On ne touche pas à isModified ! 
          // La disquette doit rester si elle était là.
          console.log('Commentaire sauvegardé');
        }
      });
    }
    else if (field === 'categorie') {
      // SCÉNARIO B : On modifie manuellement la catégorie
      // Ici, on considère que l'utilisateur a fait un choix, on peut soit auto-save,
      // soit simplement marquer isModified pour faire apparaître la disquette.
      op.isModified = false;
      this.save(op);
    }
  }

  onQuickFilterChanged() {
    this.gridApi.setGridOption('quickFilterText', this.searchString);
  }

  // Pour s'assurer que AG Grid suit bien les objets (Remplacez 'id' par votre clé primaire)
  getRowId = (params: any) => params.data.id.toString();
}