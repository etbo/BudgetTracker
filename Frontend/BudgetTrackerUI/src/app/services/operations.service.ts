import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { CcOperation } from '../models/operation-cc.model';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class OperationsService {
  private apiUrl = `${environment.apiUrl}/operations`;

  constructor(private http: HttpClient) { }

  getOperations(filters: any) {
    let params = new HttpParams().set('mode', filters.view || 'last');

    if (filters.missingCat) params = params.set('missingCat', 'true');
    if (filters.onlyCheques) params = params.set('onlyCheques', 'true');
    if (filters.suggestedCat) params = params.set('suggestedCat', 'true');

    if (filters.view === 'custom' && filters.start && filters.end) {
      params = params.set('startDate', filters.start).set('endDate', filters.end);
    }

    return this.http.get<CcOperation[]>(`${environment.apiUrl}/operations`, { params });
  }

  updateOperation(op: CcOperation) {
    return this.http.put(`${this.apiUrl}/${op.id}`, op);
  }
}