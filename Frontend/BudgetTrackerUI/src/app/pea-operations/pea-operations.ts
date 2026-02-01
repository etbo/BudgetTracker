import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AgGridModule, ICellRendererAngularComp } from 'ag-grid-angular';
import {
  ColDef,
  GridReadyEvent,
  GridApi,
  ModuleRegistry,
  AllCommunityModule,
  CellValueChangedEvent
} from 'ag-grid-community';
import { MatButtonModule } from '@angular/material/button';
import { PeaService } from '../services/pea.service';
import { PeaOperation } from '../models/operation-pea.model';
import { MatIconModule } from '@angular/material/icon';
import { currencyFormatter, amountParser, localDateSetter, customDateFormatter } from '../shared/utils/grid-utils';

ModuleRegistry.registerModules([AllCommunityModule]);

@Component({
  selector: 'app-delete-button-renderer',
  standalone: true,
  imports: [CommonModule, MatButtonModule, MatIconModule],
  template: `
    <div class="flex justify-center items-center h-full">
      <button mat-icon-button color="warn" (click)="onDelete()">
        <mat-icon>delete_outline</mat-icon>
      </button>
    </div>
  `
})
export class DeleteButtonRenderer implements ICellRendererAngularComp {
  params: any;
  agInit(params: any): void { this.params = params; }
  refresh(): boolean { return false; }
  onDelete() {
    this.params.context.componentParent.delete(this.params.data);
  }
}

@Component({
  selector: 'app-pea-operations',
  standalone: true,
  imports: [CommonModule, AgGridModule, MatButtonModule, MatIconModule],
  templateUrl: './pea-operations.html',
  styleUrl: './pea-operations.scss',
})
export class PeaOperations implements OnInit {
  private gridApi!: GridApi;
  operations = signal<PeaOperation[]>([]);

  gridContext = { componentParent: this };

  // Définition des colonnes
  columnDefs: ColDef[] = [
    {
      headerName: "Date d'achat",
      field: 'date',
      editable: true,
      cellEditor: 'agDateCellEditor', // Éditeur de date natif AG Grid
      cellDataType: false, // Désactive la détection automatique de type
      valueParser: params => params.newValue,
      width: 250,
      valueFormatter: customDateFormatter,
      valueSetter: localDateSetter
    },
    { headerName: 'Titulaire', field: 'titulaire', editable: true, flex: 1 },
    { headerName: 'Code', field: 'code', editable: true, width: 120 },
    {
      headerName: 'Quantité',
      type: 'rightAligned',
      field: 'quantite',
      editable: true,
      width: 200,
      valueParser: params => Number(params.newValue)
    },
    {
      headerName: 'Montant Brut',
      field: 'grossUnitAmount',
      type: 'rightAligned',
      editable: true,
      width: 230,
      valueFormatter: currencyFormatter,
      valueParser: amountParser
    },
    {
      headerName: 'Montant Net',
      type: 'rightAligned',
      field: 'netAmount',
      editable: true,
      width: 230,
      valueFormatter: currencyFormatter,
      valueParser: amountParser
    },
    {
      headerName: 'Frais',
      type: 'rightAligned',
      width: 230,
      valueGetter: params => this.calculateFrais(params.data),
      editable: false,
      cellClass: 'text-gray-500 bg-gray-50'
    },
    {
      headerName: '',
      width: 60,
      cellRenderer: DeleteButtonRenderer, // Utilisation du nouveau renderer
      sortable: false,
      filter: false
    }
  ];

  defaultColDef: ColDef = {
    resizable: true,
    sortable: true,
    filter: true
  };

  constructor(private peaService: PeaService) { }

  ngOnInit() {
    this.loadOperations();
  }

  onGridReady(params: GridReadyEvent) {
    this.gridApi = params.api;
  }

  loadOperations() {
    this.peaService.getAll().subscribe(data => {
      this.operations.set(data);
    });
  }

  addOperation() {
    this.peaService.create({}).subscribe(newOp => {
      // On ajoute au signal et la grille se met à jour automatiquement via [rowData]
      this.operations.update(list => [...list, newOp]);

      // Optionnel : focus sur la nouvelle ligne
      setTimeout(() => {
        this.gridApi.ensureIndexVisible(this.operations().length - 1);
        this.gridApi.startEditingCell({
          rowIndex: this.operations().length - 1,
          colKey: 'date'
        });
      }, 100);
    });
  }

  // Se déclenche dès qu'une cellule est validée (Entrée ou clic ailleurs)
  onCellValueChanged(event: CellValueChangedEvent) {
    console.log("Valeur brute envoyée au service :", event.data.date);
    this.save(event.data);
  }

  save(op: PeaOperation) {
    console.log("Date avant envoi au serveur :", op.date);
    this.peaService.update(op).subscribe({
      next: () => {
        console.log('Sauvegardé');
        // On force le rafraîchissement de la colonne "Frais" car elle dépend des autres valeurs
        this.gridApi.refreshCells({ rowNodes: [this.gridApi.getRowNode(op.id.toString())!], columns: ['frais'] });
      },
      error: () => alert('Erreur lors de la sauvegarde')
    });
  }

  delete(op: PeaOperation) {
    if (confirm('Voulez-vous vraiment supprimer cette ligne ?')) {
      this.peaService.delete(op.id).subscribe({
        next: () => {
          // Mise à jour du signal pour retirer la ligne de l'affichage
          this.operations.update(list => list.filter(item => item.id !== op.id));
        },
        error: () => alert('Erreur lors de la suppression')
      });
    }
  }

  calculateFrais(op: PeaOperation): string {
    if (op.netAmount > 0 && op.grossUnitAmount > 0 && op.netAmount > op.grossUnitAmount) {
      const pourcentage = ((op.netAmount / op.grossUnitAmount) - 1) * 100;
      return pourcentage.toFixed(2) + ' %';
    }
    return '-';
  }

  getRowId = (params: any) => params.data.id.toString();
}