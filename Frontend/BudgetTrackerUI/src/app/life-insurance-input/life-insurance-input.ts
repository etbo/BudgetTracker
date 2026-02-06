import { Component, Inject, signal, computed, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { AgGridModule } from 'ag-grid-angular';
import { ColDef, CellValueChangedEvent } from 'ag-grid-community';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { FormsModule } from '@angular/forms';
import { LifeInsuranceService } from '../services/life-insurance.service';

@Component({
  selector: 'app-life-insurance-input',
  standalone: true,
  imports: [
    CommonModule,
    AgGridModule,
    MatDialogModule,
    MatButtonModule,
    MatIconModule,
    FormsModule
  ],
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
      editable: true,
      cellRenderer: (params: any) => {
        if (!params.value) return `<span style="color: #999; font-style: italic">Saisir un nom...</span>`;
        const icon = params.data.isScpi ? 'domain' : 'euro_symbol';
        return `<span><i class="material-icons" style="font-size:18px; vertical-align:middle; margin-right:8px">${icon}</i>${params.value}</span>`;
      }
    },
    {
      headerName: 'Type',
      field: 'isScpi',
      width: 130,
      editable: true,
      cellEditor: 'agSelectCellEditor',
      cellEditorParams: {
        values: [true, false]
      },
      valueFormatter: params => params.value ? 'SCPI (Parts)' : 'Euro / UC (Montant)'
    },
    {
      headerName: 'Nb Parts',
      field: 'lastUnitCount',
      width: 120,
      editable: (params) => params.data.isScpi,
      valueParser: params => Number(params.newValue),
      valueFormatter: params => params.value?.toFixed(5)
    },
    {
      headerName: 'Valeur/Montant',
      field: 'lastUnitValue',
      width: 140,
      editable: true,
      valueParser: params => Number(params.newValue),
      valueFormatter: params => params.value?.toLocaleString('fr-FR', { style: 'currency', currency: 'EUR' })
    },
    {
      headerName: '',
      width: 50,
      cellRenderer: () => `<i class="material-icons" style="color: #f44336; cursor: pointer; margin-top: 10px">delete</i>`,
      onCellClicked: (params) => this.removeLine(params.node.rowIndex!)
    }
  ];

  public totalSaisie = computed(() => {
    const rows = this.rowData();
    return rows.reduce((acc, row) => {
      const count = Number(row.lastUnitCount) || 0;
      const value = Number(row.lastUnitValue) || 0;
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
      if (!prepData || prepData.length === 0) {
        this.rowData.set([]);
        this.addLine(); // On force une ligne vide si rien n'existe
      } else {
        this.rowData.set(prepData);
        if (prepData[0].lastStatementDate) {
          this.saisieDate = this.calculateSmartDate(new Date(prepData[0].lastStatementDate));
        }
      }
    });
  }

  addLine() {
    const newLine = {
      label: '',
      isScpi: false,
      lastUnitCount: 1,
      lastUnitValue: 0
    };
    this.rowData.set([...this.rowData(), newLine]);
  }

  removeLine(index: number) {
    const current = this.rowData();
    current.splice(index, 1);
    this.rowData.set([...current]);
  }

  onCellValueChanged(event: CellValueChangedEvent) {
    // Si on change le type (isScpi), on s'assure que le Nb de parts est cohérent
    if (event.column.getColId() === 'isScpi' && !event.newValue) {
      event.data.lastUnitCount = 1;
    }
    this.rowData.set([...this.rowData()]);
  }

  private calculateSmartDate(lastDate: Date): string {
    const today = new Date();
    const targetDate = new Date(lastDate);
    targetDate.setMonth(targetDate.getMonth() + 3);
    return (targetDate > today) ? today.toISOString().split('T')[0] : targetDate.toISOString().split('T')[0];
  }

  onSave() {
    // On filtre les lignes sans nom
    const items = this.rowData()
      .filter(item => item.label && item.label.trim() !== '')
      .map(item => ({
        lifeInsuranceLineId: item.lineId || 0, // Utilise lineId (le nom dans ton DTO)
        label: item.label,
        isScpi: item.isScpi,
        unitCount: item.lastUnitCount,
        unitValue: item.lastUnitValue
      }));

    const payload = {
      accountId: this.selectedAccountId(),
      date: this.saisieDate,
      items: items
    };

    this.dialogRef.close(payload);
  }
}