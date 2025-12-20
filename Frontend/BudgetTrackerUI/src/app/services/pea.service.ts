import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { OperationPea } from '../models/operation-pea.model';

@Injectable({ providedIn: 'root' })
export class PeaService {
  private apiUrl = 'http://localhost:5011/api/pea';

  constructor(private http: HttpClient) {}

  getAll() {
    return this.http.get<OperationPea[]>(this.apiUrl);
  }

  create(op: Partial<OperationPea>) {
    return this.http.post<OperationPea>(this.apiUrl, op);
  }

  update(op: OperationPea) {
    return this.http.put(`${this.apiUrl}/${op.id}`, op);
  }
}