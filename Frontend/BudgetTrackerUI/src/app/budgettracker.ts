import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class BudgettrackerService {
  // Vérifiez bien le port (5000, 7000, etc.) dans votre terminal .NET
  private apiUrl = 'http://localhost:5011/weatherforecast'; 

  constructor(private http: HttpClient) { }

  getForecasts(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl);
  }
}