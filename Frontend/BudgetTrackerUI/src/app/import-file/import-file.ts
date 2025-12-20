import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { HttpClient } from '@angular/common/http';

export interface FichierTraite {
  nomDuFichier: string;
  isSuccessful: boolean;
  msgErreur: string;
  nombreOperationsLus: number;
  nombreOperationsAjoutees: number;
  parser: string | null;
  dateMin: string | null;
  dateMax: string | null;
  tempsDeTraitementMs: number;
  dateTraitement: Date;
}

@Component({
  selector: 'app-import-file',
  standalone: true,
  imports: [CommonModule, MatButtonModule, MatIconModule, MatTableModule],
  templateUrl: './import-file.html',
  styleUrl: './import-file.scss'
})
export class ImportFileComponent {
  fichiersImports = signal<FichierTraite[]>([]);
  isUploading = signal(false);
  uploadStatus = signal('');

  displayedColumns: string[] = ['nom', 'resultat', 'banque', 'stats', 'dates', 'duree'];

  constructor(private http: HttpClient) { }

  onFileSelected(event: any) {
    const files: FileList = event.target.files;

    if (files && files.length > 0) {
      this.isUploading.set(true);

      // On transforme la FileList en tableau pour utiliser forEach
      Array.from(files).forEach((file, index) => {
        this.uploadFile(file);
      });

      // Reset de l'input pour permettre de reprendre les mêmes fichiers plus tard
      event.target.value = '';
    }
  }

  private filesInProgress = 0;

  private uploadFile(file: File) {
    this.filesInProgress++;
    this.isUploading.set(true);
    this.uploadStatus.set(`Import de ${this.filesInProgress} fichier(s) en cours...`);

    const formData = new FormData();
    formData.append('file', file);

    this.http.post<FichierTraite>('http://localhost:5011/api/imports/upload', formData)
      .subscribe({
        next: (result) => {
          result.dateTraitement = new Date();
          this.fichiersImports.update(actuels => [result, ...actuels]);
          this.checkFinalization();
        },
        error: (err) => {
          console.error('Erreur upload:', err);
          // On crée une ligne d'erreur pour le tableau même si l'API est injoignable
          const errorResult: FichierTraite = {
            nomDuFichier: file.name,
            isSuccessful: false,
            msgErreur: "Serveur injoignable",
            nombreOperationsLus: 0,
            nombreOperationsAjoutees: 0,
            parser: null,
            dateMin: null,
            dateMax: null,
            tempsDeTraitementMs: 0,
            dateTraitement: new Date()
          };
          this.fichiersImports.update(actuels => [errorResult, ...actuels]);
          this.checkFinalization();
        }
      });
  }

  private checkFinalization() {
    this.filesInProgress--;
    if (this.filesInProgress <= 0) {
      this.filesInProgress = 0;
      this.isUploading.set(false);
      this.uploadStatus.set('Tous les imports sont terminés');
    }
  }
}