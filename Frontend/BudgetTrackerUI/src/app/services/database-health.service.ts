import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface DatabaseHealthReport {
  missingMonths: AccountMissingMonths[];
  unknownCategories: UnknownCategory[];
}

export interface AccountMissingMonths {
  accountId: number;
  accountName: string;
  missingMonths: string[];
}

export interface UnknownCategory {
  categoryName: string;
  operationCount: number;
}

@Injectable({
  providedIn: 'root'
})
export class DatabaseHealthService {
  private apiUrl = `${environment.apiUrl}/DatabaseHealth`;

  constructor(private http: HttpClient) { }

  getHealthReport(): Observable<DatabaseHealthReport> {
    return this.http.get<DatabaseHealthReport>(this.apiUrl);
  }
}
