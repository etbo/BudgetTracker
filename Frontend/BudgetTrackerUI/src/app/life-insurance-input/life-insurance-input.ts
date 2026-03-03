import { Component, Inject, signal, computed, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { AgGridModule } from 'ag-grid-angular';
import { ColDef, CellValueChangedEvent } from 'ag-grid-community';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { FormsModule } from '@angular/forms';
import { LifeInsuranceService } from '../services/life-insurance.service';
import { BaseGrid } from '../shared/base-grid';
import { GridDeleteButton } from '../shared/components/grid-delete-button/grid-delete-button';

// Material Modules
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule, MAT_DATE_LOCALE } from '@angular/material/core';

@Component({
  selector: 'app-life-insurance-input',
  standalone: true,
  providers: [
    { provide: MAT_DATE_LOCALE, useValue: 'fr-FR' }
  ],
  imports: [
    CommonModule,
    AgGridModule,
    MatDialogModule,
    MatButtonModule,
    MatIconModule,
    FormsModule,
    MatSelectModule,
    MatFormFieldModule,
    MatInputModule,
    MatDatepickerModule,
    MatNativeDateModule
  ],
  templateUrl: './life-insurance-input.html'
})
export class LifeInsuranceInput extends BaseGrid {
  private liService = inject(LifeInsuranceService);

  public accounts = signal<any[]>([]);
  public selectedAccountId = signal<number | null>(null);
  public rowData = signal<any[]>([]);

  // Utilisation d'un signal Date
  public saisieDate = signal<Date>(new Date());

  public gridOptions = this.createGridOptions(this);

  public columnDefs: ColDef[] = [
    { headerName: 'Actif', field: 'label', flex: 1, editable: true },
    {
      headerName: 'Type',
      field: 'isScpi',
      width: 130,
      editable: true,
      cellEditor: 'agSelectCellEditor',
      cellEditorParams: { values: [true, false] },
      valueFormatter: params => params.value ? 'SCPI (Parts)' : 'Euro / UC (Montant)'
    },
    {
      headerName: 'Nb Parts',
      field: 'unitCount',
      width: 120,
      editable: (params) => params.data.isScpi,
      valueParser: params => Number(params.newValue),
    },
    {
      headerName: 'Valeur/Montant',
      field: 'unitValue',
      width: 140,
      editable: true,
      valueParser: params => Number(params.newValue),
      valueFormatter: params => params.value?.toLocaleString('fr-FR', { style: 'currency', currency: 'EUR' })
    },
    {
      headerName: '',
      width: 60,
      cellRenderer: GridDeleteButton,
      cellRendererParams: { methodName: 'removeLine' }
    }
  ];

  public totalSaisie = computed(() => {
    return this.rowData().reduce((acc, row) => {
      const count = Number(row.unitCount) || 0;
      const value = Number(row.unitValue) || 0;
      return acc + (row.isScpi ? (count * value) : value);
    }, 0);
  });

  constructor(
    public dialogRef: MatDialogRef<LifeInsuranceInput>,
    @Inject(MAT_DIALOG_DATA) public data: { accounts: any[] }
  ) {
    super();
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
        this.addLine();
      } else {
        const mappedData = prepData.map(d => ({
          lineId: d.lineId,
          label: d.label,
          isScpi: d.isScpi,
          unitCount: d.lastUnitCount,
          unitValue: d.lastUnitValue
        }));
        this.rowData.set(mappedData);
        if (prepData[0].lastStatementDate) {
          this.saisieDate.set(new Date(prepData[0].lastStatementDate));
        }
      }
    });
  }

  addLine() {
    // Utilisation de unitCount ici pour correspondre aux columnDefs
    this.rowData.set([...this.rowData(), { label: '', isScpi: false, unitCount: 1, unitValue: 0 }]);
  }

  removeLine(row: any) {
    this.rowData.set(this.rowData().filter(r => r !== row));
  }

  onCellValueChanged(event: CellValueChangedEvent) {
    if (event.column.getColId() === 'isScpi' && !event.newValue) {
      event.data.unitCount = 1;
    }
    this.rowData.set([...this.rowData()]);
  }

  onDateChange(event: any) {
    if (event.value) {
      this.saisieDate.set(event.value);
    }
  }

  onSave() {
    const d = this.saisieDate();

    // Si la date est invalide (NaN), on ne sauve pas
    if (!d || isNaN(d.getTime())) {
      return;
    }

    const dateStr = `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')}`;

    this.dialogRef.close({
      accountId: this.selectedAccountId(),
      date: dateStr,
      items: this.rowData()
        .filter(item => item.label?.trim())
        .map(item => ({
          lifeInsuranceLineId: item.lineId || 0,
          label: item.label,
          isScpi: item.isScpi,
          unitCount: item.unitCount,
          unitValue: item.unitValue
        }))
    });
  }
}