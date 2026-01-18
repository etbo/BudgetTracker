import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AccountStatus } from '../models/account-status.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  private apiUrl = `${environment.apiUrl}/accountstatus`;

  private http = inject(HttpClient);

  getAccountsStatus(): Observable<AccountStatus[]> {
    return this.http.get<AccountStatus[]>(`${this.apiUrl}/status`);
  }
}