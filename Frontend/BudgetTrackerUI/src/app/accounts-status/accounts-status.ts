import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { Router } from '@angular/router';
import { AccountService } from '../services/account.service';
import { AccountStatus } from '../models/account-status.model';


@Component({
  selector: 'app-accounts-status',
  standalone: true,
  imports: [
    CommonModule, 
    MatCardModule, 
    MatButtonModule, 
    MatIconModule, 
    MatProgressSpinnerModule
  ],
  templateUrl: './accounts-status.html',
  styleUrls: ['./accounts-status.scss']
})
export class AccountsStatus  implements OnInit {
  private accountService = inject(AccountService);
  private router = inject(Router);

  // Signaux pour la gestion d'état
  public statusList = signal<AccountStatus[]>([]);
  public isLoading = signal<boolean>(true);
  public error = signal<string | null>(null);

  ngOnInit() {
    this.loadStatus();
  }

  loadStatus() {
    this.isLoading.set(true);
    this.accountService.getAccountsStatus().subscribe({
      next: (data) => {
        this.statusList.set(data);
        this.isLoading.set(false);
      },
      error: (err) => {
        this.error.set("Erreur lors de la récupération des statuts");
        this.isLoading.set(false);
        console.error(err);
      }
    });
  }

  /**
   * Redirige vers la page d'import en pré-remplissant le compte
   */
  goToImport(accountName: string) {
    // Exemple : passer le nom du compte en paramètre d'URL
    this.router.navigate(['/import'], { queryParams: { bank: accountName } });
  }

  /**
   * Calculer la classe CSS selon l'urgence
   */
  getStatusClass(item: AccountStatus): string {
    if (!item.lastEntryDate) return 'status-critical'; // Jamais importé
    return item.actionRequired ? 'status-warning' : 'status-ok';
  }
}