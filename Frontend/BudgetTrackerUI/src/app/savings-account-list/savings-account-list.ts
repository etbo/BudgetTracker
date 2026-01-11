import { Component, OnInit, inject } from '@angular/core';
import { SavingsService } from '../services/savings.service';
import { ColDef } from 'ag-grid-community';
import { AgGridAngular } from 'ag-grid-angular';

@Component({
  selector: 'app-savings-account-list',
  imports: [AgGridAngular],
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
    { field: 'bankName', headerName: 'Banque', editable: true, filter: true },
    { field: 'isActive', headerName: 'Actif', editable: true, cellEditor: 'agCheckboxCellEditor', width: 100 }
  ];

  public defaultColDef: ColDef = {
    flex: 1,
    sortable: true,
    resizable: true
  };

  ngOnInit() {
    this.savingsService.loadAccounts();
  }

  addAccount() {
    const newAccount = { name: 'Nouveau Livret', owner: 'Nom', bank: 'Banque', isActive: true };
    this.savingsService.createAccount(newAccount as any).subscribe();
  }

  onCellValueChanged(event: any) {
    // Ici, tu appelleras une méthode updateAccount dans ton service
    console.log('Mise à jour du compte:', event.data);
    this.savingsService.updateAccount(event.data).subscribe();
  }

}
