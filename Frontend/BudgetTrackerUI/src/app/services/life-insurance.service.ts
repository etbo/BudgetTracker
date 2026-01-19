import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class LifeInsuranceService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/LifeInsurance`;

  /**
   * Récupère la liste des comptes/contrats pour le sélecteur
   */
  getAccounts(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/accounts`);
  }

  /**
   * Récupère les dernières valeurs pour pré-remplir la saisie
   */
  getPrepareSaisie(accountId: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/last-values/${accountId}`);
  }

  /**
   * Récupère l'historique complet (format plat)
   */
  getHistory(accountId: number = 0): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/history/${accountId}`);
  }

  /**
   * Enregistre les nouveaux relevés
   */
  saveSaisie(statements: any[]): Observable<any> {
    return this.http.post(`${this.apiUrl}/save-statement`, statements);
  }

  updateStatement(id: number, data: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/update-statement/${id}`, data);
  }
}