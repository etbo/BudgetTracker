import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { PeaOperation } from '../models/operation-pea.model';
import { CumulPea } from '../models/cumul-pea.model'; // <--- Ajoute cet import
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class PeaService {
  private apiUrl = `${environment.apiUrl}/pea`;

  constructor(private http: HttpClient) { }

  // --- Méthodes pour la page d'édition (PeaInput) ---
  getAll() {
    return this.http.get<PeaOperation[]>(this.apiUrl);
  }

  create(op: Partial<PeaOperation>) {
    return this.http.post<PeaOperation>(this.apiUrl, op);
  }

  update(op: PeaOperation) {
    return this.http.put(`${this.apiUrl}/${op.id}`, op);
  }

  delete(id: number | string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}