import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { CcDailyBalance } from '../models/daily-balance.model';
import { environment } from '../../environments/environment';
import { Observable } from 'rxjs/internal/Observable';
import { CategoryBalance } from '../models/category-balance.model';
import { filtersService } from './filters.service';

@Injectable({ providedIn: 'root' })
export class BalanceService {
  private apiUrl = `${environment.apiUrl}/ccdashboard`;

  constructor(private http: HttpClient) { }

  getEvolution(): Observable<CcDailyBalance[]> {
    const filters = filtersService.getFilters();
    let params = new HttpParams();

    if (filters.start) params = params.set('start', filters.start);
    if (filters.end) params = params.set('end', filters.end);

    // On envoie une seule string : "Neutre,Investissement"
    if (filters.excludedCategories && filters.excludedCategories.length > 0) {
      params = params.set('excludedCategories', filters.excludedCategories.join(','));
    }

    return this.http.get<CcDailyBalance[]>(`${this.apiUrl}/evolution`, { params });
  }

  getAllCategoryBalances(): Observable<CategoryBalance[]> {
    const filters = filtersService.getFilters();
    let params = new HttpParams();

    if (filters.start) params = params.set('start', filters.start);
    if (filters.end) params = params.set('end', filters.end);
    if (filters.excludedCategories?.length) {
      params = params.set('excludedCategories', filters.excludedCategories.join(','));
    }

    return this.http.get<CategoryBalance[]>(`${this.apiUrl}/total-by-category`, { params });
  }
}