import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { CcOperation } from '../models/operation-cc.model';
import { environment } from '../../environments/environment';
import { FilterState } from './filters.service';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class OperationsService {
  private apiUrl = `${environment.apiUrl}/operations`;

  constructor(private http: HttpClient) { }

  getOperations(filters: FilterState): Observable<CcOperation[]> {
    let params = new HttpParams()
      .set('mode', filters.view || 'last')
      .set('missingCat', !!filters.missingCat)
      .set('suggestedCat', !!filters.suggestedCat)
      .set('onlyCheques', !!filters.onlyCheques);

    if (filters.start) {
      params = params.set('startDate', filters.start);
    }
    
    if (filters.end) {
      params = params.set('endDate', filters.end);
    }

    // AJOUT : On envoie les catégories exclues sous forme de string "Cat1,Cat2"
    if (filters.excludedCategories && filters.excludedCategories.length > 0) {
      params = params.set('excludedCategories', filters.excludedCategories.join(','));
    }

    return this.http.get<CcOperation[]>(this.apiUrl, { params });
  }

  updateOperation(op: CcOperation) {
    return this.http.put(`${this.apiUrl}/${op.id}`, op);
  }
}