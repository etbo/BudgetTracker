import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AgGridModule } from 'ag-grid-angular';
import { ColDef, GridOptions } from 'ag-grid-community';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar } from '@angular/material/snack-bar';
import { AccountManagementService } from '../services/account-management.service';
import { GridDeleteButton } from '../shared/components/grid-delete-button/grid-delete-button';

@Component({
  selector: 'app-account-list',
  standalone: true, // Ajouté pour cohérence
  imports: [CommonModule, AgGridModule, MatButtonModule, MatIconModule],
  templateUrl: './account-list.html',
  styleUrl: './account-list.scss',
})
export class AccountList implements OnInit {
  private accountService = inject(AccountManagementService);
  private snackBar = inject(MatSnackBar);

  accounts = this.accountService.accounts;

  public gridOptions: GridOptions = {
    context: { componentParent: this },
    stopEditingWhenCellsLoseFocus: true,
    undoRedoCellEditing: true,
    rowClassRules: {
      'inactive-row': (params: any) => !params.data.isActive
    }
  };

  public columnDefs: ColDef[] = [
    {
      field: 'isActive', headerName: 'Actif', width: 90, flex: 0,
      editable: true, cellDataType: 'boolean'
    },
    {
      field: 'isLate',
      headerName: 'Status',
      width: 200,
      flex: 0,
      cellRenderer: (params: any) => {
        if (!params.data.isActive) {
          return `<span style="color: #9e9e9e; font-style: italic;">Inactif</span>`;
        }
        const color = params.value ? '#f44336' : '#4caf50';
        return `<span style="color: ${color}; font-weight: bold;">●</span> ${params.data.statusMessage}`;
      }
    },
    {
      field: 'type', headerName: 'Type', width: 120,
      editable: true,
      cellEditor: 'agSelectCellEditor',
      cellEditorParams: { values: [0, 1, 2] },
      valueFormatter: params => ['CC', 'Livret', 'AV'][params.value] || '?'
    },
    { field: 'name', headerName: 'Nom', editable: true },
    { field: 'bankName', headerName: 'Établissement', editable: true },
    { field: 'owner', headerName: 'Propriétaire', editable: true },
    { field: 'lastEntryDate', headerName: 'Dernier relevé', valueFormatter: p => p.value ? new Date(p.value).toLocaleDateString() : '-' },
    {
      field: 'updateFrequencyInMonths', headerName: 'Fréq.', width: 80,
      editable: true,
      cellEditor: 'agSelectCellEditor',
      cellEditorParams: { values: [1, 3, 6, 12] },
      valueFormatter: params => params.value + 'm'
    },
    {
      headerName: '',
      width: 60,
      flex: 0,
      cellRenderer: GridDeleteButton,
      cellRendererParams: { methodName: 'deleteAccount' }
    }
  ];

  public defaultColDef: ColDef = { 
    flex: 1, 
    sortable: true, 
    resizable: true, 
    filter: true,
    filterParams: {
      buttons: ['clear']
    }
  };

  ngOnInit() {
    this.accountService.loadAccounts();
  }

  addAccount() {
    const newAccount = {
      name: 'Nouveau compte',
      owner: 'À définir',
      bankName: 'Banque',
      isActive: true,
      updateFrequencyInMonths: 1,
      type: 0 
    };

    this.accountService.createAccount(newAccount).subscribe({
      next: () => {
        this.snackBar.open('✅ Compte créé', 'OK', { duration: 3000 });
        this.accountService.loadAccounts();
      }
    });
  }

  deleteAccount(account: any) {
    if(confirm(`Voulez-vous vraiment supprimer le compte "${account.name}" ?`)) {
      this.accountService.deleteAccount(account.id).subscribe({
        next: () => {
          this.snackBar.open('✅ Compte supprimé', 'OK', { duration: 3000 });
          this.accountService.loadAccounts();
        },
        error: () => this.snackBar.open('❌ Erreur lors de la suppression', 'Fermer')
      });
    }
  }

  onCellValueChanged(event: any) {
    this.accountService.updateAccount(event.data).subscribe({
      next: () => {
        this.snackBar.open('✅ Mise à jour réussie', '', { duration: 3000 });
        this.accountService.loadAccounts();
      },
      error: () => {
        this.snackBar.open('❌ Erreur lors de la mise à jour', 'Fermer');
        this.accountService.loadAccounts();
      }
    });
  }
}