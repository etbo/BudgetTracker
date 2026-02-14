import { Injectable, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class AccountManagementService {
    private http = inject(HttpClient);
    // On change l'URL pour pointer vers le nouveau contrôleur 'Accounts'
    private apiUrl = `${environment.apiUrl}/Accounts`;

    accounts = signal<any[]>([]);

    loadAccounts() {
        this.http.get<any[]>(this.apiUrl).subscribe(data => {
            this.accounts.set(data); // Le signal prévient Angular de rafraîchir la grille
        });
    }

    createAccount(account: any): Observable<any> {
        return this.http.post(this.apiUrl, account).pipe(tap(() => this.loadAccounts()));
    }

    updateAccount(account: any): Observable<any> {
        // Route simplifiée : api/Accounts/{id}
        return this.http.put(`${this.apiUrl}/${account.id}`, account);
    }

    deleteAccount(id: number): Observable<any> {
        return this.http.delete(`${this.apiUrl}/${id}`).pipe(tap(() => this.loadAccounts()));
    }
}