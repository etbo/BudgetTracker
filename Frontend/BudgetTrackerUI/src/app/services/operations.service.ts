import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { CcOperation } from '../models/operation-cc.model';
import { environment } from '../../environments/environment';
import { filtersService, FilterState } from './filters.service';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class OperationsService {
  private apiUrl = `${environment.apiUrl}/operations`;

  constructor(private http: HttpClient) { }

  /**
   * Récupère les opérations.
   * @param customFilters Si fourni, utilise ces filtres au lieu de ceux du service global.
   */
  getOperations(customFilters?: FilterState): Observable<CcOperation[]> {
    // Si customFilters est fourni, on l'utilise, sinon on prend les filtres globaux
    const filters = customFilters || filtersService.getFilters();

    let params = new HttpParams()
      .set('mode', filters.view || 'last')
      .set('missingCat', (!!filters.missingCat).toString())
      .set('suggestedCat', (!!filters.suggestedCat).toString())
      .set('onlyCheques', (!!filters.onlyCheques).toString());

    if (filters.start) {
      params = params.set('startDate', filters.start);
    }

    if (filters.end) {
      params = params.set('endDate', filters.end);
    }

    if (filters.excludedCategories && filters.excludedCategories.length > 0) {
      params = params.set('excludedCategories', filters.excludedCategories.join(','));
    }

    return this.http.get<CcOperation[]>(this.apiUrl, { params });
  }

  updateOperation(op: CcOperation) {
    // Note : On envoie le DTO (qui contient Amount, Label, etc. selon ton mapping C#)
    return this.http.put(`${this.apiUrl}/${op.id}`, op);
  }

  getSuggestion(op: CcOperation): Observable<{ categorie: string, isSuggested: boolean }> {
    return this.http.post<{ categorie: string, isSuggested: boolean }>(
      `${this.apiUrl}/suggest`,
      op
    );
  }
}