import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AgGridModule } from 'ag-grid-angular';
import { ColDef, GridOptions, CellClickedEvent } from 'ag-grid-community';
import { LifeInsuranceService } from '../services/life-insurance.service';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { LifeInsuranceInput } from '../life-insurance-input/life-insurance-input';
import { customDateFormatter, localDateSetter } from '../shared/utils/grid-utils';

interface HistoryRow {
  id?: number;
  isHeader: boolean;
  accountName: string;
  accountOwner: string;
  groupKey: string;
  expanded?: boolean;
  date?: string;
  lineLabel?: string;
  unitCount?: number;
  unitValue?: number;
  accountInfo?: string;
  total?: number;
}

@Component({
  selector: 'app-life-insurance-statement',
  standalone: true,
  imports: [CommonModule, AgGridModule, MatButtonModule, MatIconModule, MatDialogModule, MatSnackBarModule],
  templateUrl: './life-insurance-statement.html',
  styleUrl: './life-insurance-statement.scss',
})
export class LifeInsuranceStatement implements OnInit {
  private dialog = inject(MatDialog);
  private liService = inject(LifeInsuranceService);
  private snackBar = inject(MatSnackBar);

  private rawHistoryData = signal<any[]>([]);
  private expandedGroups = signal<Set<string>>(new Set());

  public historyData = computed(() => {
    const raw = this.rawHistoryData();
    const expanded = this.expandedGroups();
    const result: HistoryRow[] = [];

    const groups = new Map<string, any[]>();
    raw.forEach(item => {
      const key = item.groupKey;
      if (!groups.has(key)) groups.set(key, []);
      groups.get(key)?.push(item);
    });

    groups.forEach((items, key) => {
      const groupTotal = items.reduce((sum, i) => sum + (i.unitCount * i.unitValue), 0);
      const isExpanded = expanded.has(key);

      result.push({
        isHeader: true,
        groupKey: key,
        expanded: isExpanded,
        total: groupTotal,
        date: items[0].date,
        accountName: items[0].accountName,
        accountOwner: items[0].accountOwner
      });

      if (isExpanded) {
        items.forEach(i => result.push({ ...i, isHeader: false }));
      }
    });

    return result;
  });

  public columnDefsHistory: ColDef[] = [
    {
      headerName: 'Compte / Actif',
      field: 'accountName',
      flex: 1,
      valueGetter: (p) => p.data.isHeader
        ? `${p.data.expanded ? '▼' : '▶'} ${p.data.accountName} (${p.data.accountOwner})`
        : `      ${p.data.lineLabel}`,
      cellStyle: (p): any => p.data.isHeader ? { fontWeight: 'bold', cursor: 'pointer' } : { paddingLeft: '50px' }
    },
    {
      headerName: 'Date',
      field: 'date',
      width: 250,
      editable: (p) => p.data.isHeader,
      cellEditor: 'agDateCellEditor',
      cellDataType: false,
      singleClickEdit: true,
      cellStyle: (p): any => p.data.isHeader ? { fontWeight: 'bold' } : '',
      valueParser: params => params.newValue,
      valueFormatter: customDateFormatter,
      valueSetter: localDateSetter
    },
    {
      headerName: 'Parts',
      field: 'unitCount',
      type: 'rightAligned',
      width: 150,
      editable: (p) => !p.data.isHeader && p.data.isScpi,
      valueFormatter: (p) => p.data.isHeader ? '' : (p.data.isScpi ? p.value?.toFixed(5) : '-'),
    },
    {
      headerName: 'Valeur (unitaire)',
      field: 'unitValue',
      type: 'rightAligned',
      width: 150,
      editable: (p) => !p.data.isHeader,
      valueFormatter: (p) => p.data.isHeader ? '' : (p.value ? p.value.toFixed(2) + ' €' : ''),
    },
    {
      headerName: 'Total',
      type: 'rightAligned',
      width: 150,
      valueGetter: (p) => p.data.isHeader ? p.data.total : (p.data.unitCount * p.data.unitValue),
      valueFormatter: (p) => p.value?.toLocaleString('fr-FR', { style: 'currency', currency: 'EUR' }),
      cellStyle: (p): any => p.data.isHeader ? { fontWeight: 'bold' } : { color: '#666' }
    },
    {
      headerName: '',
      width: 60,
      cellRenderer: (p: any) => p.data.isHeader
        ? `<i class="material-icons">delete_outline</i>`
        : '',
      onCellClicked: (p) => { if (p.data.isHeader) this.deleteGroup(p.data); }
    }
  ];

  public defaultColDef: ColDef = { resizable: true, sortable: true, filter: true };

  public gridOptions: GridOptions = {
    onCellClicked: (event: CellClickedEvent) => {
      // On ne toggle que si on clique sur la première colonne
      if (event.column.getColId() === 'accountName') this.toggleGroup(event);
    },
    suppressNoRowsOverlay: false
  };

  ngOnInit() {
    this.loadAllHistory();
  }

  loadAllHistory() {
    this.liService.getHistory(0).subscribe(data => this.rawHistoryData.set(data));
  }

  toggleGroup(event: CellClickedEvent) {
    if (event.data.isHeader) {
      const key = event.data.groupKey;
      const newExpanded = new Set(this.expandedGroups());
      if (newExpanded.has(key)) newExpanded.delete(key);
      else newExpanded.add(key);
      this.expandedGroups.set(newExpanded);
    }
  }

  deleteGroup(data: HistoryRow) {
    if (confirm(`Supprimer ce relevé du ${new Date(data.date!).toLocaleDateString()} ?`)) {
      this.liService.deleteStatementGroup(data.groupKey).subscribe({
        next: () => {
          this.snackBar.open('✅ Relevé supprimé', 'OK', { duration: 2000 });
          this.loadAllHistory();
        }
      });
    }
  }

  onCellValueChanged(event: any) {
    if (event.data.isHeader && event.column.getColId() === 'date') {

      // Au lieu d'envoyer event.value (qui est l'objet Date brut de l'éditeur)
      // On envoie event.data.date (qui a été formaté par localDateSetter avec le T12:00:00)

      this.liService.updateStatementGroupDate({
        groupKey: event.data.groupKey,
        newDate: event.data.date // <-- Utilise la donnée formatée
      }).subscribe({
        next: () => this.loadAllHistory(),
        error: () => this.loadAllHistory()
      });
      return;
    }

    // Cas 2: Modif valeur sur une ligne détail
    if (!event.data.isHeader) {
      const payload = {
        lifeInsuranceLineId: event.data.lineId,
        date: event.data.date,
        unitCount: event.data.unitCount,
        unitValue: event.data.unitValue
      };
      this.liService.updateStatement(event.data.id, payload).subscribe({
        next: () => this.loadAllHistory(),
        error: () => this.loadAllHistory()
      });
    }
  }

  openSaisie() {
    this.liService.getAccounts().subscribe(accounts => {
      const dialogRef = this.dialog.open(LifeInsuranceInput, {
        width: '700px',
        data: { accounts: accounts }
      });
      dialogRef.afterClosed().subscribe(result => { if (result) this.saveData(result); });
    });
  }

  saveData(result: any) {
    const formattedItems = result.items.map((row: any) => ({
      lifeInsuranceLineId: row.lifeInsuranceLineId || 0,
      label: row.label,
      isScpi: row.isScpi,
      unitCount: row.isScpi ? (row.unitCount || 0) : 1,
      unitValue: row.unitValue || 0
    }));

    this.liService.saveSaisie({
      accountId: result.accountId,
      date: result.date,
      items: formattedItems
    }).subscribe(() => this.loadAllHistory());
  }
}