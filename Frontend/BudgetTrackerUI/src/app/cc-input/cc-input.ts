import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { ImportService } from '../services/import.service';
import { FichierTraite } from '../models/fichier-traite.model';

@Component({
  selector: 'app-cc-input',
  standalone: true,
  imports: [CommonModule, MatButtonModule, MatIconModule, MatTableModule],
  templateUrl: './cc-input.html',
  styleUrl: './cc-input.scss'
})
export class CcInput {
  fichiersImports = signal<FichierTraite[]>([]);
  isUploading = signal(false);
  uploadStatus = signal('');
  private filesInProgress = 0;

  displayedColumns: string[] = ['nom', 'resultat', 'banque', 'stats', 'dates', 'duree'];

  constructor(private importService: ImportService) { }

  onFileSelected(event: any) {
    const files: FileList = event.target.files;
    if (files && files.length > 0) {
      this.isUploading.set(true);
      Array.from(files).forEach(file => this.uploadFile(file));
      event.target.value = ''; // Reset pour permettre de re-sélectionner le même fichier
    }
  }

  private uploadFile(file: File) {
    this.filesInProgress++;
    this.uploadStatus.set(`Import de ${this.filesInProgress} fichier(s) en cours...`);

    this.importService.uploadFile(file).subscribe({
      next: (result) => {
        result.dateTraitement = new Date();
        this.fichiersImports.update(actuels => [result, ...actuels]);
        this.checkFinalization();
      },
      error: (err) => {
        console.error('Erreur upload:', err);
        this.fichiersImports.update(actuels => [this.createErrorResult(file.name), ...actuels]);
        this.checkFinalization();
      }
    });
  }

  private createErrorResult(fileName: string): FichierTraite {
    return {
      nomDuFichier: fileName,
      isSuccessful: false,
      msgErreur: "Serveur injoignable ou erreur réseau",
      nombreOperationsLus: 0,
      nombreOperationsAjoutees: 0,
      parser: null,
      dateMin: null,
      dateMax: null,
      tempsDeTraitementMs: 0,
      dateTraitement: new Date()
    };
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