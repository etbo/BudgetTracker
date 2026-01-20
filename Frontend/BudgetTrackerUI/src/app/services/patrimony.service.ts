import { HttpClient } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { environment } from "../../environments/environment";

export interface GlobalHistoryPoint {
  label: string;         // "01/2024"
  date: string;          // ISO Date
  cash: number;          // Flux cumulés
  savings: number;       // Propagation solde
  lifeInsurance: number; // Propagation solde
  pea: number;           // Quantité x Prix
}

@Injectable({
  providedIn: 'root'
})
export class PatrimonyService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/GlobalPatrimony`;

  getGlobalHistory(): Observable<GlobalHistoryPoint[]> {
    return this.http.get<GlobalHistoryPoint[]>(`${this.apiUrl}/history`);
  }
}