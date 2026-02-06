import { Component, signal, OnInit, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips'; // N'oublie pas l'import
import { ImportService } from '../services/import.service';
import { FichierTraite } from '../models/fichier-traite.model';

import { AgGridModule } from 'ag-grid-angular';
import { ColDef, ModuleRegistry, AllCommunityModule, GridReadyEvent, GridApi, ValueFormatterParams } from 'ag-grid-community';
import { customDateFormatter } from '../shared/utils/grid-utils';

ModuleRegistry.registerModules([AllCommunityModule]);

@Component({
  selector: 'app-cc-input',
  standalone: true,
  imports: [CommonModule, MatButtonModule, MatIconModule, MatChipsModule, AgGridModule],
  templateUrl: './cc-input.html',
  styleUrl: './cc-input.scss'
})
export class CcInput implements OnInit {
  isUploading = signal(false);
  uploadStatus = signal('');

  // 1. Initialisation du filtre (Mode persisté)
  activeFilter = signal<string>(localStorage.getItem('importFilter') || 'all');

  // 2. Les données brutes (Signal pour la réactivité)
  allRowData = signal<FichierTraite[]>([]);

  columnDefs: ColDef[] = [
    { headerName: 'Fichier', field: 'nomDuFichier', flex: 1.5 },
    {
      headerName: 'Résultat',
      field: 'isSuccessful',
      flex: 1,
      cellRenderer: (p: any) => {
        const color = p.value ? '#4caf50' : '#f44336';
        const label = p.value ? 'Succès' : (p.data.msgErreur || 'Erreur');
        return `<span style="color: ${color}; font-weight: 600;">${label}</span>`;
      }
    },
    { headerName: 'Banque', field: 'parser', flex: 1 },
    {
      headerName: 'Opérations',
      valueGetter: (p) => `${p.data.nombreOperationsAjoutees} / ${p.data.nombreOperationsLus}`,
      flex: 1
    },
    {
      headerName: 'Période',
      flex: 1.2,
      valueGetter: (p) => {
        if (!p.data.dateMin || !p.data.dateMax) return '-';
        const start = customDateFormatter({ value: p.data.dateMin } as ValueFormatterParams);
        const end = customDateFormatter({ value: p.data.dateMax } as ValueFormatterParams);
        return `${start} → ${end}`;
      }
    },
    {
      headerName: 'Temps',
      field: 'processingTimeMs',
      valueFormatter: (p) => p.value ? `${p.value.toFixed(0)} ms` : '0 ms',
      width: 100
    },
    {
      headerName: 'Date import',
      field: 'dateImport',
      width: 250,
      valueFormatter: (p) => {
        if (!p.value) return '';
        const d = new Date(p.value);

        // Extraction des composants
        const date = d.toISOString().split('T')[0]; // 2026-01-23
        const hours = d.getHours().toString().padStart(2, '0');
        const mins = d.getMinutes().toString().padStart(2, '0');
        const secs = d.getSeconds().toString().padStart(2, '0');
        const ms = d.getMilliseconds().toString().padStart(3, '0');

        return `${date} à ${hours}:${mins}:${secs}.${ms}`;
      }
    },
  ];

  defaultColDef: ColDef = { sortable: true, filter: true, resizable: true };

  constructor(private importService: ImportService) { }

  ngOnInit() {
    this.loadHistory();
  }

  loadHistory() {
    this.importService.getHistory().subscribe({
      next: (data) => this.allRowData.set(data),
      error: (err) => console.error(err)
    });
  }

  setFilter(mode: string) {
    if (mode) {
      this.activeFilter.set(mode);
      localStorage.setItem('importFilter', mode);
    }
  }

  onGridReady(params: GridReadyEvent) { }

  onFileSelected(event: any) {
    const files: FileList = event.target.files;
    if (files && files.length > 0) {
      this.isUploading.set(true);
      const filesArray = Array.from(files);
      let completed = 0;

      filesArray.forEach(file => {
        this.importService.uploadFile(file).subscribe({
          next: (result) => {
            // Utilisation du signal allRowData
            this.allRowData.update(prev => [result, ...prev]);
          },
          error: (err) => {
            this.allRowData.update(prev => [this.createErrorResult(file.name), ...prev]);
          },
          complete: () => {
            completed++;
            if (completed === filesArray.length) {
              this.isUploading.set(false);
              this.uploadStatus.set('Terminé');
            }
          }
        });
      });
      event.target.value = '';
    }
  }

  private createErrorResult(fileName: string): FichierTraite {
    return {
      nomDuFichier: fileName,
      isSuccessful: false,
      msgErreur: "Erreur réseau",
      nombreOperationsLus: 0,
      nombreOperationsAjoutees: 0,
      parser: null,
      dateMin: null,
      dateMax: null,
      ProcessingTimeMs: 0
    };
  }
}