import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { DailyBalance } from '../models/daily-balance';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class BalanceService {
  private apiUrl = `${environment.apiUrl}/ccdashboard`;

  constructor(private http: HttpClient) {}

  getEvolution() {
    return this.http.get<DailyBalance[]>(`${this.apiUrl}/evolution`);
  }
}