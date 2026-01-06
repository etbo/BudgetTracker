import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { DailyBalance } from '../models/daily-balance.model';
import { environment } from '../../environments/environment';
import { Observable } from 'rxjs/internal/Observable';
import { CategoryBalance } from '../models/category-balance.model';

@Injectable({ providedIn: 'root' })
export class BalanceService {
  private apiUrl = `${environment.apiUrl}/ccdashboard`;

  constructor(private http: HttpClient) { }

  getEvolution() {
    return this.http.get<DailyBalance[]>(`${this.apiUrl}/evolution`);
  }

  getExpensesByCategory(): Observable<CategoryBalance[]> {
  return this.http.get<CategoryBalance[]>(`${this.apiUrl}/expenses-by-category`);
}
}