import { Injectable, signal, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map, tap } from 'rxjs';
import { environment } from '../../environments/environment';
import { SavingAccount, SavingStatement } from '../models/saving-account.model';

@Injectable({
  providedIn: 'root'
})
export class SavingsService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/SavingAccounts`;

  accounts = signal<SavingAccount[]>([]);

  constructor() { }

  loadAccounts(): void {
    this.http.get<SavingAccount[]>(this.apiUrl).subscribe(data => {
      this.accounts.set(data);
    });
  }

  createAccount(account: SavingAccount): Observable<SavingAccount> {
    return this.http.post<SavingAccount>(this.apiUrl, account).pipe(
      tap(() => this.loadAccounts())
    );
  }

  updateAccount(account: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/${account.id}`, account).pipe(
      tap(() => {
        this.accounts.update(current =>
          current.map(a => a.id === account.id ? { ...a, ...account } : a)
        );
      })
    );
  }

  deleteAccount(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`).pipe(
      tap(() => this.loadAccounts())
    );
  }

  getFlattenedStatements(): Observable<any[]> {
    return this.http.get<SavingAccount[]>(this.apiUrl).pipe(
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
        return allStatements.sort((a, b) => 
          new Date(b.date).getTime() - new Date(a.date).getTime()
        );
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

  deleteStatement(statementId: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/statements/${statementId}`);
  }
}