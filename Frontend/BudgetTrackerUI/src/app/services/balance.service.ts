import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
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

  getExpensesByCategory(startDate?: string, endDate?: string): Observable<CategoryBalance[]> {
    let params = new HttpParams();

    console.log('startDate:', startDate);
    console.log('endDate:', endDate);

    if (startDate && endDate) {
      params = params.set('start', startDate).set('end', endDate);
    }

    return this.http.get<CategoryBalance[]>(`${this.apiUrl}/expenses-by-category`, { params });
  }
}