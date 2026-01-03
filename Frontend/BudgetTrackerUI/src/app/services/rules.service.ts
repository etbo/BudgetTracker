import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CcCategoryRule, CcCategory } from '../models/category-rule.model';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class RulesService {
  private apiUrl = 'http://localhost:5011/api/rules';

  constructor(private http: HttpClient) { }

  getRules() { return this.http.get<CcCategoryRule[]>(this.apiUrl); }
  getCcCategories() { return this.http.get<CcCategory[]>(`${this.apiUrl}/CcCategories`); }

  create(rule: Partial<CcCategoryRule>) { return this.http.post<CcCategoryRule>(this.apiUrl, rule); }
  update(rule: CcCategoryRule) { return this.http.put(`${this.apiUrl}/${rule.id}`, rule); }

  delete(id: number | string): Observable<void> {
    // On ne rajoute PAS "/rules/" ici si c'est déjà dans l'apiUrl
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}