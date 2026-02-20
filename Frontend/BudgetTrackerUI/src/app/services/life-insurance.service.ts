import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class LifeInsuranceService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/LifeInsurance`;

  getAccounts(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/accounts`);
  }

  getPrepareSaisie(accountId: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/last-values/${accountId}`);
  }

  getHistory(accountId: number = 0): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/history/${accountId}`);
  }

  saveSaisie(payload: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/save-statement`, payload);
  }

  updateStatement(id: number, data: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/update-statement/${id}`, data);
  }

  // --- NOUVELLES MÉTHODES POUR LE GROUPAGE ---

  deleteStatementGroup(groupKey: string): Observable<any> {
    // encodeURIComponent transforme les "/" et les espaces en caractères sûrs pour l'URL
    const safeKey = encodeURIComponent(groupKey);
    return this.http.delete(`${this.apiUrl}/history/group/${safeKey}`);
  }

  updateStatementGroupDate(payload: { groupKey: string, newDate: string }): Observable<any> {
    // Ici pas besoin de encodeURIComponent car on passe la clé dans le BODY (JSON) 
    // et non dans l'URL. C'est le JSON qui sera parsé par .NET.
    return this.http.put(`${this.apiUrl}/history/group/date`, payload);
  }

  // ------------------------------------------

  createAccount(account: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/accounts`, account);
  }

  updateAccount(account: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/accounts/${account.id}`, account);
  }
}