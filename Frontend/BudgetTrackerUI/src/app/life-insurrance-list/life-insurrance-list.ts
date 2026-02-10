import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar } from '@angular/material/snack-bar';
import { LifeInsuranceService } from '../services/life-insurance.service';
import { ColDef } from 'ag-grid-community';
import { AgGridModule } from 'ag-grid-angular';

@Component({
  selector: 'app-life-insurrance-list',
  standalone: true,
  imports: [CommonModule, FormsModule, MatCardModule, MatFormFieldModule, MatInputModule, MatButtonModule, MatIconModule, AgGridModule],
  templateUrl: './life-insurrance-list.html',
  styleUrl: './life-insurrance-list.scss',
})
export class LifeInsurranceList implements OnInit {
  private liService = inject(LifeInsuranceService);
  private snackBar = inject(MatSnackBar);

  // On utilise un signal local pour la grille
  accounts = signal<any[]>([]);

  public columnDefs: ColDef[] = [
    { 
      field: 'isActive', 
      headerName: 'Actif', 
      editable: true, 
      cellDataType: 'boolean', 
      width: 100,
      flex: 0,
      cellStyle: { 'display': 'flex', 'justify-content': 'center' }
    },
    { field: 'name', headerName: 'Nom du Contrat', editable: true, minWidth: 200 },
    { field: 'owner', headerName: 'Propriétaire', editable: true, minWidth: 200 },
    { field: 'bankName', headerName: 'Assureur / Plateforme', editable: true, filter: true }, // Ex: Spirica, Bourso...
    {
      headerName: 'Fréquence MàJ',
      field: 'updateFrequencyInMonths',
      editable: true,
      cellEditor: 'agSelectCellEditor',
      cellEditorParams: {
        values: [1, 3, 6, 12] 
      },
      valueFormatter: params => params.value + ' mois'
    }
  ];

  public defaultColDef: ColDef = {
    flex: 1,
    sortable: true,
    resizable: true,
    filter: true
  };

  ngOnInit() {
    this.loadAccounts();
  }

  loadAccounts() {
    this.liService.getAccounts().subscribe(data => {
      this.accounts.set(data);
    });
  }

  addAccount() {
    const newAccount = { 
      name: 'Nouveau Contrat AV', 
      bankName: 'Assureur', 
      owner: 'À définir',
      isActive: true, 
      updateFrequencyInMonths: 3,
      type: 2
    };
    
    // On appelle la méthode createAccount que nous avons ajoutée au service précédemment
    this.liService.createAccount(newAccount).subscribe({
      next: () => {
        this.snackBar.open('Nouveau contrat ajouté', 'OK', { duration: 2000 });
        this.loadAccounts();
      }
    });
  }

  onCellValueChanged(event: any) {
    console.log('Mise à jour du contrat:', event.data);
    // On utilise la méthode updateAccount (à vérifier/ajouter dans ton service)
    this.liService.updateAccount(event.data).subscribe({
      next: () => this.snackBar.open('Modification enregistrée', '', { duration: 1500 }),
      error: () => this.snackBar.open('Erreur lors de la sauvegarde', 'Fermer')
    });
  }
}