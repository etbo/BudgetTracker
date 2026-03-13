import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { MatSnackBar } from '@angular/material/snack-bar';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ExportService {
  private apiUrl = `${environment.apiUrl}/export`;

  constructor(
    private http: HttpClient,
    private snackBar: MatSnackBar
  ) { }

  downloadDatabaseBackup() {
    this.http.get(`${environment.apiUrl}/export/database`, { responseType: 'blob' })
      .subscribe({
        next: (blob) => {
          const url = window.URL.createObjectURL(blob);
          const a = document.createElement('a');

          // Nom du fichier avec date du jour
          const now = new Date();
          const date = now.getFullYear() +
            ('0' + (now.getMonth() + 1)).slice(-2) +
            ('0' + now.getDate()).slice(-2) + "_" +
            ('0' + now.getHours()).slice(-2) + "h" +
            ('0' + now.getMinutes()).slice(-2);

          a.href = url;
          a.download = `BudgetTracker_Backup_${date}.zip`;

          document.body.appendChild(a);
          a.click();

          // Nettoyage de la mémoire et du DOM
          window.URL.revokeObjectURL(url);
          document.body.removeChild(a);
        },
        error: (error: HttpErrorResponse) => {
          this.handleExportError(error);
        }
      });
  }

  private async handleExportError(error: HttpErrorResponse) {
    let message = "Une erreur est survenue lors de l'export.";

    // L'API renvoie un Blob même pour les erreurs à cause du responseType: 'blob'
    // Il faut donc lire le texte à l'intérieur du blob pour avoir le message du serveur
    if (error.error instanceof Blob) {
      message = await error.error.text();
    }

    this.snackBar.open(message, 'Fermer', {
      duration: 3000,
      panelClass: ['error-snackbar']
    });
  }
}