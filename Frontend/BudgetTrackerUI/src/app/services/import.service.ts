import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment'; // Toujours importer l'environnement
import { FichierTraite } from '../models/fichier-traite.model';

@Injectable({ providedIn: 'root' })
export class ImportService {
    private apiUrl = `${environment.apiUrl}/imports`;

    constructor(private http: HttpClient) { }

    getHistory() {
        return this.http.get<FichierTraite[]>(this.apiUrl);
    }

    uploadFile(file: File) {
        const formData = new FormData();
        formData.append('file', file);
        return this.http.post<FichierTraite>(`${this.apiUrl}/upload`, formData);
    }
}