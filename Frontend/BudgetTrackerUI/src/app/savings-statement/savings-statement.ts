import { Component, effect, OnInit, signal, inject } from '@angular/core'; // Ajout de inject
import { AgGridAngular } from 'ag-grid-angular';
import { ColDef, GridReadyEvent, GridApi, ValueFormatterParams, GridOptions } from 'ag-grid-community';
import { SavingsService } from '../services/savings.service';
import { customDateFormatter, localDateSetter } from '../shared/utils/grid-utils';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar'; // Ajout des imports Material
import { GridDeleteButton } from '../shared/components/grid-delete-button/grid-delete-button';
import { BaseGrid } from '../shared/base-grid';

@Component({
  selector: 'app-savings-statement',
  standalone: true, // Assure-toi qu'il est standalone si tu l'utilises ainsi
  imports: [AgGridAngular, MatIconModule, MatButtonModule, MatSnackBarModule], // Ajout du module
  templateUrl: './savings-statement.html',
  styleUrl: './savings-statement.scss',
})
export class SavingsStatement extends BaseGrid implements OnInit {
  private snackBar = inject(MatSnackBar);
  private savingsService = inject(SavingsService);

  // On initialise les options en passant 'this'
  public gridOptions: GridOptions = this.createGridOptions(this);

  private gridApi!: GridApi;
  rowData = signal<any[]>([]);

  defaultColDef = { resizable: true, sortable: true, filter: true, flex: 1 };

  // On l'initialise vide, l'effect va le remplir
  columnDefs: ColDef[] = [];

  constructor() {
    super();
    
    // L'effect surveille le signal savingsService.accounts()
    effect(() => {
      const accounts = this.savingsService.accounts();

      // On définit les colonnes à l'intérieur pour avoir accès à 'accounts'
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
            values: accounts.map(a => a.id) // Plus d'erreur ici !
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

      // Si la grille est déjà prête, on lui pousse les nouvelles définitions
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
      console.log('Données reçues du serveur:', data);
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

  deleteStatement(row: any) {
    if (!row.id) {
        // Si c'est une ligne locale pas encore sauvée, on l'enlève juste du signal
        this.rowData.set(this.rowData().filter(r => r !== row));
        return;
    }

    if(confirm('Voulez-vous vraiment supprimer ce relevé ?')) {
      this.savingsService.deleteStatement(row.id).subscribe({
        next: () => {
          this.snackBar.open('✅ Relevé supprimé', 'OK', { duration: 3000 });
          this.loadData();
        },
        error: () => this.snackBar.open('❌ Erreur lors de la suppression', 'Fermer')
      });
    }
  }
}