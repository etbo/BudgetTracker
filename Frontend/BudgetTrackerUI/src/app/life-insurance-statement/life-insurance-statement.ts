import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AgGridModule } from 'ag-grid-angular';
import { ColDef, GridOptions, CellClickedEvent } from 'ag-grid-community';
import { LifeInsuranceService } from '../services/life-insurance.service';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { LifeInsuranceInput } from '../life-insurance-input/life-insurance-input';
import { customDateFormatter } from '../shared/utils/grid-utils';

// Interface pour gérer la structure Header/Détail
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
  imports: [CommonModule, AgGridModule, MatButtonModule, MatIconModule, MatDialogModule],
  templateUrl: './life-insurance-statement.html',
  styleUrl: './life-insurance-statement.scss',
})
export class LifeInsuranceStatement implements OnInit {
  private dialog = inject(MatDialog);
  private liService = inject(LifeInsuranceService);

  // Stockage brut et état des groupes
  private rawHistoryData = signal<any[]>([]);
  private expandedGroups = signal<Set<string>>(new Set());

  // Signal calculé pour AG Grid (Simulation du groupage)
  public historyData = computed(() => {
    const raw = this.rawHistoryData();
    const expanded = this.expandedGroups();
    const result: HistoryRow[] = [];

    // Regroupement par GroupKey (formaté par le backend)
    const groups = new Map<string, any[]>();
    raw.forEach(item => {
      const key = item.groupKey;
      if (!groups.has(key)) groups.set(key, []);
      groups.get(key)?.push(item);
    });

    groups.forEach((items, key) => {
      // 1. Ligne de résumé (Header)
      const groupTotal = items.reduce((sum, i) => sum + (i.unitCount * i.unitValue), 0);
      const isExpanded = expanded.has(key);

      result.push({
        isHeader: true,
        groupKey: key,
        expanded: isExpanded,
        total: groupTotal,
        date: items[0].date, // Pour le tri
        accountName: items[0].accountName,
        accountOwner: items[0].accountOwner
      });

      // 2. Lignes de détails (si le groupe est déplié)
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
      // Si c'est un header, on affiche le nom du compte, sinon le libellé de l'actif
      valueGetter: (p) => p.data.isHeader
        ? `${p.data.expanded ? '▼' : '▶'} ${p.data.accountName} (${p.data.accountOwner})`
        : `      ${p.data.lineLabel}`,
      cellStyle: (p): any => p.data.isHeader ? { fontWeight: 'bold', cursor: 'pointer' } : { paddingLeft: '50px' }
    },
    {
      headerName: 'Date',
      field: 'date',
      width: 300,
      // On n'affiche la date que sur la ligne de Header pour éviter la répétition
      // valueFormatter: customDateFormatter,
      valueFormatter: (p) => {
        if (!p.value) return '';
        if (p.data.isHeader) {
          return customDateFormatter(p); // ou customDateFormatter(p.value) selon ta définition
        }
        return "";
      },
      cellStyle: (p): any => p.data.isHeader ? { fontWeight: 'bold' } : null
    },
    {
      headerName: 'Parts',
      field: 'unitCount',
      type: 'rightAligned',
      width: 250,
      editable: (p) => !p.data.isHeader && p.data.isScpi,
      valueFormatter: (p) => {
        if (p.data.isHeader) return '';
        // Si c'est un Fonds Euro (pas SCPI), on n'affiche rien ou un tiret
        if (!p.data.isScpi) return '-';
        return p.value?.toFixed(5);
      },
      cellStyle: (p): any => {
        if (p.data.isHeader) return { color: '#ccc' };
        return null;
      }
    },
    {
      headerName: 'Valeur (unitaire)',
      field: 'unitValue',
      type: 'rightAligned',
      width: 250,
      editable: (p) => !p.data.isHeader,
      valueFormatter: (p) => p.data.isHeader ? '' : (p.value ? p.value.toFixed(2) + ' €' : ''),
      cellStyle: (p) => p.data.isHeader
        ? { fontWeight: 'bold', cursor: 'pointer' }
        : null
    },
    {
      headerName: 'Total',
      type: 'rightAligned',
      width: 250,
      valueGetter: (p) => p.data.isHeader ? p.data.total : (p.data.unitCount * p.data.unitValue),
      valueFormatter: (p) => p.value?.toLocaleString('fr-FR', { style: 'currency', currency: 'EUR' }),
      cellStyle: (p): any => {
        if (p.data.isHeader) {
          return { fontWeight: 'bold' };
        }
        return { color: '#666' };
      }
    }
  ];

  defaultColDef = {
    resizable: true, sortable: true, filter: true,
    filterParams: {
      buttons: ['clear']
    }
  };

  public gridOptions: GridOptions = {
    onCellClicked: (event: CellClickedEvent) => this.toggleGroup(event),
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

  openSaisie() {
    this.liService.getAccounts().subscribe(accounts => {
      const dialogRef = this.dialog.open(LifeInsuranceInput, {
        width: '700px',
        data: { accounts: accounts }
      });

      dialogRef.afterClosed().subscribe(result => {
        if (result) this.saveData(result);
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

    this.liService.saveSaisie(payload).subscribe(() => this.loadAllHistory());
  }

  onCellValueChanged(event: any) {
    if (event.data.isHeader) return;

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