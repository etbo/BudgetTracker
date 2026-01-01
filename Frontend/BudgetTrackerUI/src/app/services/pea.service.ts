import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { OperationPea } from '../models/operation-pea.model';
import { CumulPea } from '../models/cumul-pea.model'; // <--- Ajoute cet import
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class PeaService {
  private apiUrl = 'http://localhost:5011/api/pea';

  constructor(private http: HttpClient) { }

  // --- Méthodes pour la page d'édition (PeaInput) ---
  getAll() {
    return this.http.get<OperationPea[]>(this.apiUrl);
  }

  create(op: Partial<OperationPea>) {
    return this.http.post<OperationPea>(this.apiUrl, op);
  }

  update(op: OperationPea) {
    return this.http.put(`${this.apiUrl}/${op.id}`, op);
  }

  delete(id: number | string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}