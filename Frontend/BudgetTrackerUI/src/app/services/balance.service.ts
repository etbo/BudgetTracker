import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { DailyBalance } from '../models/daily-balance';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class BalanceService {
  private apiUrl = `${environment.apiUrl}/evolution`;

  constructor(private http: HttpClient) {}

  getEvolution() {
    // Vérifiez bien le port (5011) ici !
    return this.http.get<DailyBalance[]>(this.apiUrl);
  }
}