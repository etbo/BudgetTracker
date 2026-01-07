import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { CcOperation } from '../models/operation-cc.model';
import { environment } from '../../environments/environment';
import { FilterState } from './filters.service';

@Injectable({ providedIn: 'root' })
export class OperationsService {
  private apiUrl = `${environment.apiUrl}/operations`;

  constructor(private http: HttpClient) { }

  getOperations(filters: FilterState) {
    let params = new HttpParams()
      .set('mode', filters.view || 'last') // On mappe view vers mode
      .set('missingCat', !!filters.missingCat)
      .set('suggestedCat', !!filters.suggestedCat)
      .set('onlyCheques', !!filters.onlyCheques);

    if (filters.start) params = params.set('startDate', filters.start);
    if (filters.end) params = params.set('endDate', filters.end);

    return this.http.get<CcOperation[]>(this.apiUrl, { params });
  }

  updateOperation(op: CcOperation) {
    return this.http.put(`${this.apiUrl}/${op.id}`, op);
  }
}