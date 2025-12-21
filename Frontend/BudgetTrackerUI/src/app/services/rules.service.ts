import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CategoryRule, Category } from '../models/category-rule.model';

@Injectable({ providedIn: 'root' })
export class RulesService {
  private apiUrl = 'http://localhost:5011/api/rules';

  constructor(private http: HttpClient) {}

  getRules() { return this.http.get<CategoryRule[]>(this.apiUrl); }
  getCategories() { return this.http.get<Category[]>(`${this.apiUrl}/categories`); }
  
  create(rule: Partial<CategoryRule>) { return this.http.post<CategoryRule>(this.apiUrl, rule); }
  update(rule: CategoryRule) { return this.http.put(`${this.apiUrl}/${rule.id}`, rule); }
}