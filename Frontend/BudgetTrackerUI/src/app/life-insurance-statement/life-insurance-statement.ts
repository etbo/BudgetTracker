import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AgGridModule } from 'ag-grid-angular';
import { ColDef, GridOptions } from 'ag-grid-community';
import { LifeInsuranceService } from '../services/life-insurance.service';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { LifeInsuranceInput } from '../life-insurance-input/life-insurance-input';

@Component({
  selector: 'app-life-insurance-statement',
  standalone: true,
  imports: [CommonModule, AgGridModule, MatButtonModule, MatIconModule, MatDialogModule],
  templateUrl: './life-insurance-statement.html',
  styleUrl: './life-insurance-statement.scss',
})
export class LifeInsuranceStatement implements OnInit {
  private dialog = inject(MatDialog);
  private liService = inject(LifeInsuranceService);

  // Données de l'historique
  public historyData = signal<any[]>([]);

  // Configuration des colonnes pour l'HISTORIQUE
  public columnDefsHistory: ColDef[] = [
    {
      headerName: 'Contrat',
      field: 'accountInfo',
      filter: 'agTextColumnFilter',
      flex: 1,
      // cellStyle: { color: '#666', fontStyle: 'italic' }
    },
    {
      headerName: 'Date',
      field: 'date',
      sort: 'desc',
      editable: true,
      valueFormatter: (p) => p.value ? new Date(p.value).toLocaleDateString('fr-FR', { month: 'long', year: 'numeric' }) : ''
    },
    { headerName: 'Actif', field: 'lineLabel', flex: 1 },
    { headerName: 'Parts', field: 'unitCount', editable: true, valueFormatter: (p) => p.value?.toFixed(5), width: 200 },
    { headerName: 'Prix Unitaire', editable: true, field: 'unitValue', valueFormatter: (p) => p.value ? p.value.toFixed(2) + ' €' : '', width: 200 },
    {
      headerName: 'Total',
      valueGetter: (p) => (p.data.unitCount || 0) * (p.data.unitValue || 0),
      valueFormatter: (p) => p.value?.toLocaleString('fr-FR', { style: 'currency', currency: 'EUR' }),
      cellStyle: { fontWeight: 'bold' },
      width: 200
    }
  ];

  public gridOptions: GridOptions = {
    rowHeight: 45,
    defaultColDef: { resizable: true, sortable: true, filter: true }
  };

  ngOnInit() {
    this.loadAllHistory();
  }

  // Charge l'historique global (ou tu peux passer un ID spécifique si besoin)
  loadAllHistory() {
    // Si ton backend le permet, on peut passer 0 ou rien pour tout voir, 
    // sinon on garde l'ID par défaut pour l'instant
    this.liService.getHistory(0).subscribe(data => this.historyData.set(data));
  }

  openSaisie() {
    this.liService.getAccounts().subscribe(accounts => {
      const dialogRef = this.dialog.open(LifeInsuranceInput, {
        width: '700px',
        data: { accounts: accounts }
      });

      dialogRef.afterClosed().subscribe(result => {
        if (result) {
          this.saveData(result);
        }
      });
    });
  }

  saveData(result: any) {
    const payload = result.items.map((row: any) => ({
      lifeInsuranceLineId: row.lineId,
      date: result.date,
      unitCount: row.isScpi ? row.lastUnitCount : 1,
      unitValue: row.lastUnitValue
    }));

    this.liService.saveSaisie(payload).subscribe(() => {
      this.loadAllHistory(); // Rafraîchit la grille après sauvegarde
    });
  }

  onCellValueChanged(event: any) {
    const data = event.data;
    const payload = {
      lifeInsuranceLineId: 0, // Non utilisé par le PUT mais requis par le DTO si tu réutilises le même
      date: data.date,
      unitCount: data.unitCount,
      unitValue: data.unitValue
    };

    // Appel au service pour faire le PUT
    this.liService.updateStatement(data.id, payload).subscribe({
      next: () => console.log('Mise à jour réussie'),
      error: (err) => {
        console.error('Erreur MAJ', err);
        this.loadAllHistory(); // On recharge en cas d'erreur pour annuler visuellement
      }
    });
  }
}