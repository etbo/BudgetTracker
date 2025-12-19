import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-import-file',
  standalone: true,
  imports: [CommonModule, MatButtonModule, MatIconModule],
  templateUrl: './import-file.html'
})
export class ImportFileComponent {
  selectedFile: File | null = null;
  uploadStatus: string = '';

  constructor(private http: HttpClient) {}

  onFileSelected(event: any) {
    this.selectedFile = event.target.files[0];
  }

  onUpload() {
    if (!this.selectedFile) return;

    const formData = new FormData();
    formData.append('file', this.selectedFile);

    // L'URL de votre futur endpoint .NET
    this.http.post('http://localhost:5011/api/imports/upload', formData)
      .subscribe({
        next: () => this.uploadStatus = 'Fichier importé avec succès !',
        error: (err) => this.uploadStatus = 'Erreur lors de l\'import : ' + err.message
      });
  }
}