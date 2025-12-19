import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { DailyBalance } from '../models/daily-balance';

@Injectable({ providedIn: 'root' })
export class BalanceService {
  constructor(private http: HttpClient) {}

  getEvolution() {
    // Vérifiez bien le port (5011) ici !
    return this.http.get<DailyBalance[]>('http://localhost:5011/api/reports/evolution');
  }
}