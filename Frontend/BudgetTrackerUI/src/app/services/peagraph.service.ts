import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CumulPea } from '../models/cumul-pea.model';
import { environment } from '../../environments/environment';

export interface UpdateResult {
  ticker: string;
  status: 'Success' | 'Failed' | 'NotAttempted';
  message: string;
}

@Injectable({ providedIn: 'root' })
export class PeaGraphService {
  private apiUrl = `${environment.apiUrl}/PeaGraph`;

  constructor(private http: HttpClient) {}

  // Déclenche la mise à jour de tous les prix sur le serveur
  updatePrices(): Observable<UpdateResult[]> {
    return this.http.get<UpdateResult[]>(`${this.apiUrl}/update-prices`);
  }

  // --- Méthode pour la page graphique (PeaWallet) ---
  getCalculerCumul(): Observable<CumulPea[]> {
    return this.http.get<CumulPea[]>(`${this.apiUrl}/cumul`);
  }
}