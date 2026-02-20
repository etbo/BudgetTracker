import { Component, effect, OnInit, signal, inject } from '@angular/core'; // Ajout de inject
import { AgGridAngular } from 'ag-grid-angular';
import { ColDef, GridReadyEvent, GridApi, ValueFormatterParams } from 'ag-grid-community';
import { SavingsService } from '../services/savings.service';
import { customDateFormatter, localDateSetter } from '../shared/utils/grid-utils';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar'; // Ajout des imports Material

@Component({
  selector: 'app-savings-statement',
  standalone: true, // Assure-toi qu'il est standalone si tu l'utilises ainsi
  imports: [AgGridAngular, MatIconModule, MatButtonModule, MatSnackBarModule], // Ajout du module
  templateUrl: './savings-statement.html',
  styleUrl: './savings-statement.scss',
})
export class SavingsStatement implements OnInit {
  private savingsService = inject(SavingsService);
  private snackBar = inject(MatSnackBar); // Injection de la snackbar

  public columnDefs: ColDef[] = [];
  private gridApi!: GridApi;
  rowData = signal<any[]>([]);

  constructor() {
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
        cellDataType: false,
        cellEditor: 'agDateCellEditor',
        valueFormatter: customDateFormatter,
        valueSetter: localDateSetter,
        filter: 'agTextColumnFilter',
        filterValueGetter: params => {
          return customDateFormatter({ value: params.data.date } as any);
        },
      },
      {
        field: 'accountId',
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
          const account = accounts.find(a => a.id === params.data.accountId);
          return account ? `${account.name} ${account.owner}` : '';
        }
      },
      {
        field: 'amount', headerName: 'Solde', editable: true, type: 'rightAligned',
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
      accountId: this.savingsService.accounts()[0]?.id,
      amount: 0,
      note: ''
    };
    this.rowData.set([newRow, ...this.rowData()]);
  }

  onCellValueChanged(event: any) {
    const row = event.data;
    this.savingsService.saveStatement(row).subscribe({
      next: () => {
        this.snackBar.open('✅ Relevé enregistré avec succès', 'OK', {
          duration: 3000
        });
        this.loadData();
      },
      error: (err) => {
        this.snackBar.open('❌ Erreur lors de la sauvegarde', 'Fermer', { duration: 3000 });
        console.error(err);
      }
    });
  }
}