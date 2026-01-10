import { Component, effect, OnInit, signal } from '@angular/core';
import { AgGridAngular, AgGridModule } from 'ag-grid-angular';
import { ColDef, GridReadyEvent, CellValueChangedEvent, GridApi, ValueFormatterParams } from 'ag-grid-community';
import { SavingsService } from '../services/savings.service';
import { SavingStatement } from '../models/saving-account.model';
import { customDateFormatter } from '../shared/utils/grid-utils';

@Component({
  selector: 'app-savings-statement',
  imports: [AgGridAngular],
  templateUrl: './savings-statement.html',
  styleUrl: './savings-statement.scss',
})
export class SavingsStatement implements OnInit {
  public columnDefs: ColDef[] = [];
  private gridApi!: GridApi;
  rowData = signal<any[]>([]);

  accounts = signal<{ id: number, name: string }[]>([]);

  constructor(public savingsService: SavingsService) {
    // Cet effect va se déclencher dès que 'accounts' reçoit les données du serveur
    effect(() => {
      const accounts = this.savingsService.accounts();
      if (accounts.length > 0) {
        this.refreshColumnDefs(accounts);
      }
    });
  }

  ngOnInit() {
    this.savingsService.loadAccounts();

    this.loadData();
  }

  private refreshColumnDefs(accounts: any[]) {
    this.columnDefs = [
      {
        field: 'date',
        headerName: 'Date',
        editable: true,
        cellEditor: 'agDateCellEditor',
        cellEditorParams: {
          // Force le sélecteur natif (plus simple, pas d'heure)
          useBrowserDatePicker: true,
          // On peut aussi spécifier min/max si besoin
        },

        // 2. TON FORMATEUR (Affichage dans la grille)
        valueFormatter: customDateFormatter,

        // 3. CONFIGURATION DU FILTRE (Barre flottante)
        filter: 'agDateColumnFilter',
        filterParams: {
          browserDatePicker: true,
          // Supprime les heures lors de la comparaison
          comparator: (filterLocalDateAtMidnight: Date, cellValue: string) => {
            if (!cellValue) return 0;
            const cellDate = new Date(cellValue);
            cellDate.setHours(0, 0, 0, 0); // On ignore l'heure

            if (filterLocalDateAtMidnight.getTime() === cellDate.getTime()) return 0;
            if (cellDate < filterLocalDateAtMidnight) return -1;
            return 1;
          }
        },
      },
      {
        field: 'savingAccountId',
        headerName: 'Livret',
        editable: true,
        cellEditor: 'agSelectCellEditor',
        cellEditorParams: {
          values: accounts.map(a => a.id)
        },
        valueFormatter: params => {
          const account = accounts.find(a => a.id === params.value);
          return account ? `${account.name} (${account.owner})` : 'Choisir...';
        },
        filter: 'agTextColumnFilter',
        filterValueGetter: params => {
          const account = accounts.find(a => a.id === params.data.savingAccountId);
          return account ? `${account.name} ${account.owner}` : '';
        }
      },
      {
        field: 'amount', headerName: 'Solde', editable: true,
        valueFormatter: (p: ValueFormatterParams) => p.value?.toFixed(2) + ' €',
      },
      { field: 'note', headerName: 'Note', editable: true }
    ];

    // On force AG Grid à redessiner les colonnes avec les nouvelles "values"
    if (this.gridApi) {
      this.gridApi.setGridOption('columnDefs', this.columnDefs);
    }
  }

  public defaultColDef: ColDef = {
    sortable: true,
    filter: true,
    resizable: true,
    floatingFilter: true,
    flex: 1
  };

  loadData() {
    this.savingsService.getFlattenedStatements().subscribe(data => {
      this.rowData.set(data);
    });
  }

  onGridReady(params: GridReadyEvent) {
    this.gridApi = params.api;
  }

  addNewRow() {
    const newRow = {
      date: new Date().toISOString().split('T')[0],
      savingAccountId: this.accounts()[0]?.id, // Par défaut le premier compte
      amount: 0,
      note: ''
    };

    // On l'ajoute au début du tableau
    this.rowData.set([newRow, ...this.rowData()]);
  }

  onCellValueChanged(event: any) {
    const row = event.data;
    // Si la ligne a déjà un ID, on fait un PUT, sinon un POST
    this.savingsService.saveStatement(row).subscribe(() => {
      console.log('Donnée sauvegardée !');
      this.loadData(); // On rafraîchit pour être sûr d'avoir les IDs à jour
    });
  }
}