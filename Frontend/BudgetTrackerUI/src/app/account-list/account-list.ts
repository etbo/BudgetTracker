import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AgGridModule } from 'ag-grid-angular';
import { ColDef } from 'ag-grid-community';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar } from '@angular/material/snack-bar';
import { AccountManagementService } from '../services/account-management.service';

@Component({
  selector: 'app-account-list',
  imports: [CommonModule, AgGridModule, MatButtonModule, MatIconModule],
  templateUrl: './account-list.html',
  styleUrl: './account-list.scss',
})
export class AccountList implements OnInit {
  private accountService = inject(AccountManagementService);
  private snackBar = inject(MatSnackBar);

  accounts = this.accountService.accounts;

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
      field: 'type', headerName: 'Type', width: 150,
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
      field: 'updateFrequencyInMonths', headerName: 'Fréq.', width: 100,
      editable: true,
      cellEditor: 'agSelectCellEditor',
      cellEditorParams: { values: [1, 3, 6, 12] },
      valueFormatter: params => params.value + 'm'
    }
  ];

  public defaultColDef: ColDef = { flex: 1, sortable: true, resizable: true, filter: true };

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
      type: 0 // Par défaut Compte Courant
    };

    this.accountService.createAccount(newAccount).subscribe({
      next: () => this.snackBar.open('Compte créé', 'OK', { duration: 3000 })
    });
  }

  onCellValueChanged(event: any) {
    this.accountService.updateAccount(event.data).subscribe({
      next: () => {
        this.snackBar.open('✅ Mise à jour réussie', '', { duration: 3000 });
        // ON RECHARGE ICI : Seulement quand le serveur a fini de bosser
        this.accountService.loadAccounts();
      },
      error: () => {
        this.snackBar.open('❌ Erreur lors de la mise à jour', 'Fermer');
        // En cas d'erreur, on recharge aussi pour remettre la checkbox 
        // dans son état réel en BDD (annulation visuelle)
        this.accountService.loadAccounts();
      }
    });
  }
}