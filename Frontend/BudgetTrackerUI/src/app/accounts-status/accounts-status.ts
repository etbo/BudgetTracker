import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AgGridModule } from 'ag-grid-angular';
import { ColDef, GridOptions } from 'ag-grid-community';
import { AccountService } from '../services/account.service';
import { AccountStatus } from '../models/account-status.model';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-accounts-status',
  standalone: true,
  imports: [CommonModule, AgGridModule, MatIconModule, MatButtonModule],
  templateUrl: './accounts-status.html',
  styleUrls: ['./accounts-status.scss']
})
export class AccountsStatus implements OnInit {
  private accountService = inject(AccountService);
  public statusList = signal<AccountStatus[]>([]);

  // Configuration des colonnes
  public columnDefs: ColDef[] = [
    {
      headerName: 'Statut',
      field: 'actionRequired',
      width: 150,
      cellRenderer: (params: any) => {
        const icon = params.value ? 'priority_high' : 'check_circle';
        const color = params.value ? '#f44336' : '#4caf50';
        return `<span style="color: ${color}"><i class="material-icons" style="font-size: 20px; vertical-align: middle;">${icon}</i></span>`;
      }
    },
    { headerName: 'Type', field: 'type', filter: true, width: 350 },
    { headerName: 'Compte', field: 'accountName', filter: true, flex: 1 },
    { headerName: 'Propriétaire', field: 'owner', filter: true, width: 300 },
    {
      headerName: 'Dernière MàJ',
      field: 'lastEntryDate',
      valueFormatter: (p) => p.value ? new Date(p.value).toLocaleDateString('fr-FR', { month: 'long', year: 'numeric' }) : 'Jamais',
      width: 300
    }
  ];

  public gridOptions: GridOptions = {
    rowHeight: 45,
    defaultColDef: { sortable: true, resizable: true }
  };

  ngOnInit() {
    this.loadStatus();
  }

  loadStatus() {
    this.accountService.getAccountsStatus().subscribe(data => this.statusList.set(data));
  }
}