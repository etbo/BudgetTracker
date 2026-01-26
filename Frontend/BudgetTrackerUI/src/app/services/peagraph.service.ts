import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of, tap } from 'rxjs';
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

  private isUpdated = false;

  constructor(private http: HttpClient) {}

  // Déclenche la mise à jour de tous les prix sur le serveur
  updatePrices(): Observable<UpdateResult[]> {
    return this.http.get<UpdateResult[]>(`${this.apiUrl}/update-prices`);
  }

  // --- Méthode pour la page graphique (PeaWallet) ---
  getCalculerCumul(): Observable<CumulPea[]> {
    return this.http.get<CumulPea[]>(`${this.apiUrl}/cumul`);
  }

  updatePricesIfNeeded(): Observable<any[]> {
    if (this.isUpdated) {
      console.log('Prix déjà à jour pour cette session.');
      return of([]); // On renvoie un tableau vide pour ne pas déclencher de snackbar inutile
    }

    return this.http.get<any[]>(`${this.apiUrl}/update-prices`).pipe(
      tap(() => this.isUpdated = true) // On marque comme fait
    );
  }

}