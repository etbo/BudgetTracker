import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CcOperation } from '../models/operation-cc.model';

@Injectable({ providedIn: 'root' })
export class OperationsService {
  private apiUrl = 'http://localhost:5011/api/operations';

  constructor(private http: HttpClient) {}

  getOperations(filterType: string) {
    return this.http.get<CcOperation[]>(`${this.apiUrl}?filterType=${filterType}`);
  }

  updateOperation(op: CcOperation) {
    return this.http.put(`${this.apiUrl}/${op.id}`, op);
  }
}