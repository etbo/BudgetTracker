import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { SavingAccount, SavingStatement } from '../models/saving-account.model';
import { map, Observable, tap } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
    providedIn: 'root'
})
export class SavingsService {
    private apiUrl = `${environment.apiUrl}/SavingAccounts`;

    // On utilise un signal pour exposer la liste des comptes globalement
    accounts = signal<SavingAccount[]>([]);

    constructor(private http: HttpClient) { }

    // Charger tous les comptes
    loadAccounts(): void {
        this.http.get<SavingAccount[]>(this.apiUrl).subscribe(data => {
            this.accounts.set(data);
        });
    }

    // Créer un nouveau livret
    createAccount(account: SavingAccount): Observable<SavingAccount> {
        return this.http.post<SavingAccount>(this.apiUrl, account).pipe(
            tap(() => this.loadAccounts()) // Rafraîchit la liste après création
        );
    }

    // Ajouter un relevé de solde
    addStatement(accountId: number, statement: SavingStatement): Observable<any> {
        return this.http.post(`${this.apiUrl}/${accountId}/statements`, statement).pipe(
            tap(() => this.loadAccounts()) // Rafraîchit pour avoir le nouveau solde
        );
    }

    // Retourne tous les statements de tous les comptes avec le nom du compte inclus
    getFlattenedStatements(): Observable<any[]> {
        return this.http.get<SavingAccount[]>(this.apiUrl).pipe(
            map(accounts => {
                const allStatements: any[] = [];
                accounts.forEach(acc => {
                    acc.statements?.forEach(st => {
                        allStatements.push({
                            ...st,
                            accountName: acc.name, // On ajoute le nom pour la grille
                            owner: acc.owner       // On ajoute le propriétaire pour le filtre
                        });
                    });
                });
                return allStatements.sort((a, b) => new Date(b.date).getTime() - new Date(a.date).getTime());
            })
        );
    }

    saveStatement(statement: SavingStatement): Observable<any> {
        // Si le statement a déjà un ID, c'est une modification (PUT)
        if (statement.id && statement.id > 0) {
            return this.http.put(`${environment.apiUrl}/SavingAccounts/${statement.savingAccountId}/statements/${statement.id}`, statement);
        }
        // Sinon, c'est une création (POST)
        else {
            return this.http.post(`${environment.apiUrl}/SavingAccounts/${statement.savingAccountId}/statements`, statement);
        }
    }

}