import { Component, Inject, signal, computed, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { AgGridModule } from 'ag-grid-angular';
import { ColDef, CellValueChangedEvent } from 'ag-grid-community'; // Ajout de CellValueChangedEvent
import { MatButtonModule } from '@angular/material/button';
import { FormsModule } from '@angular/forms';
import { LifeInsuranceService } from '../services/life-insurance.service';

@Component({
  selector: 'app-life-insurance-input',
  standalone: true,
  imports: [CommonModule, AgGridModule, MatDialogModule, MatButtonModule, FormsModule],
  templateUrl: './life-insurance-input.html'
})
export class LifeInsuranceInput {
  private liService = inject(LifeInsuranceService);

  public accounts = signal<any[]>([]);
  public selectedAccountId = signal<number | null>(null);
  public rowData = signal<any[]>([]);
  public saisieDate: string = new Date().toISOString().split('T')[0];

  public columnDefs: ColDef[] = [
    {
      headerName: 'Actif',
      field: 'label',
      flex: 1,
      cellRenderer: (params: any) => {
        const icon = params.data.isScpi ? 'domain' : 'euro_symbol';
        return `<span><i class="material-icons" style="font-size:18px; vertical-align:middle; margin-right:8px">${icon}</i>${params.value}</span>`;
      }
    },
    {
      headerName: 'Nb Parts',
      field: 'lastUnitCount',
      editable: p => p.data.isScpi,
      valueParser: p => Number(p.newValue), // Force la conversion en nombre lors de la saisie
      valueFormatter: p => p.value?.toFixed(5)
    },
    {
      headerName: 'Valeur/Montant',
      field: 'lastUnitValue',
      editable: true,
      valueParser: p => Number(p.newValue), // Force la conversion en nombre lors de la saisie
      valueFormatter: p => p.value?.toLocaleString('fr-FR', { style: 'currency', currency: 'EUR' })
    }
  ];

  // CALCUL DYNAMIQUE
  public totalSaisie = computed(() => {
    const rows = this.rowData();
    if (!rows || rows.length === 0) return 0;

    return rows.reduce((acc, row) => {
      const count = Number(row.lastUnitCount) || 0;
      const value = Number(row.lastUnitValue) || 0;

      // Si c'est une SCPI : Parts x Valeur, sinon direct le Montant (Fonds Euro)
      const ligneTotal = row.isScpi ? (count * value) : value;
      return acc + ligneTotal;
    }, 0);
  });

  constructor(
    public dialogRef: MatDialogRef<LifeInsuranceInput>,
    @Inject(MAT_DIALOG_DATA) public data: { accounts: any[] }
  ) {
    this.accounts.set(data.accounts);
    if (this.data.accounts.length > 0) {
      this.onAccountChange(this.data.accounts[0].id);
    }
  }

  onAccountChange(accountId: number) {
    this.selectedAccountId.set(accountId);
    this.liService.getPrepareSaisie(accountId).subscribe(prepData => {
      console.log('Données reçues du serveur :', prepData[0]);
      this.rowData.set(prepData);

      if (prepData.length > 0 && prepData[0].lastStatementDate) {
        this.saisieDate = this.calculateSmartDate(new Date(prepData[0].lastStatementDate));
      } else {
        this.saisieDate = new Date().toISOString().split('T')[0];
      }
    });
  }

  private calculateSmartDate(lastDate: Date): string {
    const today = new Date();

    console.log('lastDate', lastDate);

    // 1. Calculer Date Dernière + 3 mois
    const targetDate = new Date(lastDate);
    targetDate.setMonth(targetDate.getMonth() + 3);

    // 2. Vérifier si l'écart entre aujourd'hui et la date cible est faible 
    // (par exemple, si on est à moins de 15 jours de la cible, on peut la proposer)
    // Ou si la date cible est déjà passée.
    if (targetDate > today) {
      // Si la date cible (+3 mois) est encore dans le futur, 
      // on propose la date du jour pour permettre une saisie anticipée.
      return today.toISOString().split('T')[0];
    }

    // Sinon, on propose pile poil +3 mois pour garder une régularité parfaite
    return targetDate.toISOString().split('T')[0];
  }

  // CETTE MÉTHODE DÉCLENCHE LA MISE À JOUR DU TOTAL
  onCellValueChanged(event: CellValueChangedEvent) {
    console.log('Cellule modifiée, nouveau tableau :', this.rowData());
    // On propage le changement pour que le computed() se relance
    this.rowData.set([...this.rowData()]);
  }

  onSave() {
    this.dialogRef.close({
      date: this.saisieDate,
      items: this.rowData(),
      accountId: this.selectedAccountId()
    });
  }
}