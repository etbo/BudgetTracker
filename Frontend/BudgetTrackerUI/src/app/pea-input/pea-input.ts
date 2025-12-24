import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AgGridModule } from 'ag-grid-angular';
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
import { OperationPea } from '../models/operation-pea.model';
import { MatIconModule } from '@angular/material/icon';

import { currencyFormatter, amountParser, localDateSetter, customDateFormatter } from '../shared/utils/grid-utils';

ModuleRegistry.registerModules([AllCommunityModule]);

@Component({
  selector: 'app-pea-input',
  standalone: true,
  imports: [CommonModule, AgGridModule, MatButtonModule, MatIconModule],
  templateUrl: './pea-input.html'
})
export class PeaInputComponent implements OnInit {
  private gridApi!: GridApi;
  operations = signal<OperationPea[]>([]);

  // Définition des colonnes
  columnDefs: ColDef[] = [
    { 
      headerName: "Date d'achat", 
      field: 'date', 
      editable: true,
      cellEditor: 'agDateCellEditor', // Éditeur de date natif AG Grid
      width: 250 ,
      valueFormatter: customDateFormatter,
      valueSetter: localDateSetter
    },
    { headerName: 'Titulaire', field: 'titulaire', editable: true, flex: 1 },
    { headerName: 'Code', field: 'code', editable: true, width: 120 },
    { 
      headerName: 'Quantité', 
      field: 'quantite', 
      editable: true, 
      width: 200,
      valueParser: params => Number(params.newValue) 
    },
    { 
      headerName: 'Montant Brut', 
      field: 'montantBrutUnitaire', 
      editable: true, 
      width: 230,
      valueFormatter: currencyFormatter,
      valueParser: amountParser
    },
    { 
      headerName: 'Montant Net', 
      field: 'montantNet', 
      editable: true, 
      width: 230,
      valueFormatter: currencyFormatter,
      valueParser: amountParser
    },
    { 
      headerName: 'Frais', 
      width: 230,
      valueGetter: params => this.calculateFrais(params.data),
      editable: false,
      cellClass: 'text-gray-500 bg-gray-50' 
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
    this.save(event.data);
  }

  save(op: OperationPea) {
    this.peaService.update(op).subscribe({
      next: () => {
        console.log('Sauvegardé');
        // On force le rafraîchissement de la colonne "Frais" car elle dépend des autres valeurs
        this.gridApi.refreshCells({ rowNodes: [this.gridApi.getRowNode(op.id.toString())!], columns: ['frais'] });
      },
      error: () => alert('Erreur lors de la sauvegarde')
    });
  }

  calculateFrais(op: OperationPea): string {
    if (op.montantNet > 0 && op.montantBrutUnitaire > 0 && op.montantNet > op.montantBrutUnitaire) {
      const pourcentage = ((op.montantNet / op.montantBrutUnitaire) - 1) * 100;
      return pourcentage.toFixed(2) + ' %';
    }
    return '-';
  }

  getRowId = (params: any) => params.data.id.toString();
}