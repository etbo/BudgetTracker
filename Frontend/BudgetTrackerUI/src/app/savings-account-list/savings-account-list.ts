import { Component, OnInit, inject } from '@angular/core';
import { SavingsService } from '../services/savings.service';
import { ColDef } from 'ag-grid-community';
import { AgGridAngular } from 'ag-grid-angular';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-savings-account-list',
  imports: [AgGridAngular, MatIconModule, MatButtonModule],
  templateUrl: './savings-account-list.html',
  styleUrl: './savings-account-list.scss',
})
export class SavingsAccountList implements OnInit {
  private savingsService = inject(SavingsService);

  // On utilise le signal du service
  accounts = this.savingsService.accounts;

  public columnDefs: ColDef[] = [
    { field: 'name', headerName: 'Nom du Livret', editable: true },
    { field: 'owner', headerName: 'Propriétaire', editable: true, filter: true },
    { field: 'bankName', headerName: 'Bank', editable: true, filter: true },
    { field: 'isActive', headerName: 'Actif', editable: true, cellEditor: 'agCheckboxCellEditor', width: 100 },
    {
      headerName: 'Fréquence MàJ',
      field: 'updateFrequencyInMonths',
      editable: true,
      width: 150,
      cellEditor: 'agSelectCellEditor',
      cellEditorParams: {
        values: [1, 2, 3, 4, 6, 12] // Propose des choix logiques (mensuel, trimestriel, annuel...)
      },
      valueFormatter: params => params.value + ' mois',
      // Cette fonction se déclenche quand tu valides la modification dans la grille
      onCellValueChanged: (params) => {
        this.saveAccountChanges(params.data);
      }
    }
  ];

  saveAccountChanges(accountData: any) {
    this.savingsService.updateAccount(accountData).subscribe({
      next: () => {
        console.log('Mise à jour réussie');
        // Optionnel : afficher un petit message de succès (Toast/SnackBar)
      },
      error: (err) => {
        console.error('Erreur lors de la sauvegarde', err);
      }
    });
  }

  public defaultColDef: ColDef = {
    flex: 1,
    sortable: true,
    resizable: true
  };

  ngOnInit() {
    this.savingsService.loadAccounts();
  }

  addAccount() {
    const newAccount = { name: 'Nouveau Livret', owner: 'Nom', bank: 'Bank', isActive: true };
    this.savingsService.createAccount(newAccount as any).subscribe();
  }

  onCellValueChanged(event: any) {
    // Ici, tu appelleras une méthode updateAccount dans ton service
    console.log('Mise à jour du compte:', event.data);
    this.savingsService.updateAccount(event.data).subscribe();
  }

}
