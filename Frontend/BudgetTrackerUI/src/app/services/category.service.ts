import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Category } from "../models/category-rule.model";

@Injectable({ providedIn: 'root' })
export class CategoryService {
  private apiUrl = 'http://localhost:5011/api/categories';

  constructor(private http: HttpClient) {}

  getAll() { return this.http.get<Category[]>(this.apiUrl); }
  
  update(cat: Category) { 
    return this.http.put(`${this.apiUrl}/${cat.id}`, cat); 
  }

  create(cat: Partial<Category>) {
    return this.http.post<Category>(this.apiUrl, cat);
  }
}