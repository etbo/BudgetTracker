import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { ImportService } from '../services/import.service';
import { FichierTraite } from '../models/fichier-traite.model';

import { AgGridModule } from 'ag-grid-angular';
import { ColDef, ModuleRegistry, AllCommunityModule, GridReadyEvent, GridApi, ValueFormatterParams } from 'ag-grid-community';
import { customDateFormatter } from '../shared/utils/grid-utils';
ModuleRegistry.registerModules([AllCommunityModule]);

@Component({
  selector: 'app-cc-input',
  standalone: true,
  // Remplacer MatTableModule par AgGridModule
  imports: [CommonModule, MatButtonModule, MatIconModule, AgGridModule],
  templateUrl: './cc-input.html',
  styleUrl: './cc-input.scss'
})
export class CcInput {
  fichiersImports = signal<FichierTraite[]>([]);
  isUploading = signal(false);
  uploadStatus = signal('');
  private filesInProgress = 0;
  private gridApi!: GridApi;

  // Définition des colonnes AG Grid
  columnDefs: ColDef[] = [
    { headerName: 'Fichier', field: 'nomDuFichier', flex: 1.5 },
    {
      headerName: 'Résultat',
      field: 'isSuccessful',
      flex: 1,
      cellRenderer: (p: any) => {
        const color = p.value ? 'green' : 'red';
        const label = p.value ? 'Succès' : (p.data.msgErreur || 'Erreur');
        return `<span style="color: ${color}; font-weight: 500;">${label}</span>`;
      }
    },
    { headerName: 'Banque', field: 'parser', flex: 1 },
    {
      headerName: 'Opérations (Lues/Ajoutées)',
      valueGetter: (p) => `${p.data.nombreOperationsAjoutees} / ${p.data.nombreOperationsLus}`,
      flex: 1
    },
    {
      headerName: 'Période',
      valueGetter: (p) => {
        if (!p.data.dateMin || !p.data.dateMax) return '-';

        // On appelle manuellement ton formateur pour chaque date
        const start = customDateFormatter({ value: p.data.dateMin } as ValueFormatterParams);
        const end = customDateFormatter({ value: p.data.dateMax } as ValueFormatterParams);

        return `${start} → ${end}`;
      },
      flex: 1.2
    },
    {
      headerName: 'Traitement',
      field: 'tempsDeTraitementMs',
      valueFormatter: (p) => `${p.value?.toFixed(0)} ms`,
      width: 120
    }
  ];

  defaultColDef: ColDef = {
    sortable: true,
    filter: true,
    resizable: true
  };

  constructor(private importService: ImportService) { }

  onGridReady(params: GridReadyEvent) {
    this.gridApi = params.api;
  }

  onFileSelected(event: any) {
    const files: FileList = event.target.files;
    if (files && files.length > 0) {
      this.isUploading.set(true);
      Array.from(files).forEach(file => this.uploadFile(file));
      event.target.value = '';
    }
  }

  private uploadFile(file: File) {
    this.filesInProgress++;
    this.uploadStatus.set(`LOADING Import de ${this.filesInProgress} fichier(s) en cours...`);

    this.importService.uploadFile(file).subscribe({
      next: (result) => {
        result.dateTraitement = new Date();
        this.fichiersImports.update(actuels => [result, ...actuels]);
        this.checkFinalization();
      },
      error: (err) => {
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