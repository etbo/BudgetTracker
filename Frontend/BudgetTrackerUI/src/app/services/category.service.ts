import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { CcCategory } from "../models/category-rule.model";
import { Observable } from "rxjs";
import { environment } from "../../environments/environment";

@Injectable({ providedIn: 'root' })
export class CategoryService {
  private apiUrl = `${environment.apiUrl}/CcCategories`;

  constructor(private http: HttpClient) {}

  getAll() { return this.http.get<CcCategory[]>(this.apiUrl); }
  
  update(cat: CcCategory) { 
    return this.http.put(`${this.apiUrl}/${cat.id}`, cat); 
  }

  create(cat: Partial<CcCategory>) {
    return this.http.post<CcCategory>(this.apiUrl, cat);
  }

  delete(id: number | string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}