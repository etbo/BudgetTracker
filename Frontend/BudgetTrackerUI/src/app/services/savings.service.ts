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
        return this.http.get<any[]>(this.apiUrl).pipe( // Le type retourné est désormais Account[]
            map(accounts => {
                const allStatements: any[] = [];
                accounts.forEach(acc => {
                    acc.savingStatements?.forEach((st: any) => {
                        allStatements.push({
                            ...st,
                            accountName: acc.name,
                            owner: acc.owner
                        });
                    });
                });
                return allStatements.sort((a, b) => new Date(b.date).getTime() - new Date(a.date).getTime());
            })
        );
    }

    saveStatement(statement: any): Observable<any> {
        const accountId = statement.accountId;

        if (statement.id && statement.id > 0) {
            return this.http.put(`${this.apiUrl}/${accountId}/statements/${statement.id}`, statement);
        } else {
            return this.http.post(`${this.apiUrl}/${accountId}/statements`, statement);
        }
    }

    updateAccount(account: any): Observable<any> {
        return this.http.put(`${this.apiUrl}/accounts/${account.id}`, account).pipe(
            tap(() => {
                // Mise à jour du signal local pour que toutes les pages (List et Statements) soient à jour
                this.accounts.update(currentAccounts =>
                    currentAccounts.map(a => a.id === account.id ? account : a)
                );
            })
        );
    }

}