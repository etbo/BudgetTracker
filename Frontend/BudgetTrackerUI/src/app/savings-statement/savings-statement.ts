import { Component, effect, OnInit, signal, inject } from '@angular/core';
import { AgGridAngular } from 'ag-grid-angular';
import { ColDef, GridReadyEvent, GridApi, ValueFormatterParams, GridOptions } from 'ag-grid-community';
import { SavingsService } from '../services/savings.service';
import { customDateFormatter, localDateSetter } from '../shared/utils/grid-utils';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { GridDeleteButton } from '../shared/components/grid-delete-button/grid-delete-button';
import { BaseGrid } from '../shared/base-grid';

@Component({
  selector: 'app-savings-statement',
  standalone: true,
  imports: [
    AgGridAngular,
    MatIconModule,
    MatButtonModule,
    MatSnackBarModule
  ],
  templateUrl: './savings-statement.html',
  styleUrl: './savings-statement.scss',
})
export class SavingsStatement extends BaseGrid implements OnInit {
  private snackBar = inject(MatSnackBar);
  private savingsService = inject(SavingsService);

  // Initialisation des options héritées de BaseGrid
  public gridOptions: GridOptions = this.createGridOptions(this);
  private gridApi!: GridApi;

  rowData = signal<any[]>([]);
  columnDefs: ColDef[] = [];
  defaultColDef = { 
    resizable: true, 
    sortable: true, 
    filter: true, 
    flex: 1,
    filterParams: {
      buttons: ['clear']
    }
  };

  constructor() {
    super();

    effect(() => {
      const accounts = this.savingsService.accounts();

      const newDefs: ColDef[] = [
        {
          field: 'date',
          headerName: 'Date',
          editable: true,
          cellDataType: false,
          cellEditor: 'agDateCellEditor',
          valueFormatter: customDateFormatter,
          valueSetter: localDateSetter,
          filter: 'agTextColumnFilter',
          filterValueGetter: params => customDateFormatter({ value: params.data.date } as any),
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
          field: 'amount',
          headerName: 'Solde',
          editable: true,
          type: 'rightAligned',
          valueFormatter: (p: ValueFormatterParams) => p.value ? p.value.toFixed(2) + ' €' : '0.00 €',
        },
        { field: 'note', headerName: 'Note', editable: true },
        {
          headerName: '',
          width: 60,
          flex: 0,
          cellRenderer: GridDeleteButton,
          cellRendererParams: { methodName: 'deleteStatement' }
        }
      ];

      this.columnDefs = newDefs;

      if (this.gridApi) {
        this.gridApi.setGridOption('columnDefs', newDefs);
      }
    });
  }

  ngOnInit() {
    this.savingsService.loadAccounts();
    this.loadData();
  }

  loadData() {
    this.savingsService.getFlattenedStatements().subscribe(data => {
      this.rowData.set(data);
    });
  }

  onGridReady(params: GridReadyEvent) {
    this.gridApi = params.api;
  }

  addNewRow() {
    const currentAccounts = this.savingsService.accounts();
    const newRow = {
      date: new Date().toISOString().split('T')[0],
      accountId: currentAccounts.length > 0 ? currentAccounts[0].id : null,
      amount: 0,
      note: ''
    };
    this.rowData.set([newRow, ...this.rowData()]);
  }

  onCellValueChanged(event: any) {
    const row = event.data;
    this.savingsService.saveStatement(row).subscribe({
      next: () => {
        this.snackBar.open('✅ Relevé enregistré', 'OK', { duration: 3000 });
        this.loadData();
      },
      error: (err) => {
        this.snackBar.open('❌ Erreur sauvegarde', 'Fermer', { duration: 3000 });
        console.error(err);
      }
    });
  }

  deleteStatement(row: any) {
    if (!row.id) {
      this.rowData.set(this.rowData().filter(r => r !== row));
      return;
    }

    if (confirm('Voulez-vous vraiment supprimer ce relevé ?')) {
      this.savingsService.deleteStatement(row.id).subscribe({
        next: () => {
          this.snackBar.open('✅ Relevé supprimé', 'OK', { duration: 3000 });
          this.loadData();
        },
        error: () => this.snackBar.open('❌ Erreur suppression', 'Fermer')
      });
    }
  }
}