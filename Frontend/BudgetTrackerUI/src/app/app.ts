import { Component, inject, OnInit } from '@angular/core'; // OnInit et signal ne sont plus nécessaires ici
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

import { MatSidenavModule } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatFormFieldModule } from '@angular/material/form-field';

import { NavMenuComponent } from './nav-menu/nav-menu';

import { filtersService } from './services/filters.service';
import { ExportService } from './services/export.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { PeaGraphService } from './services/peagraph.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    NavMenuComponent,
    MatSidenavModule,
    MatToolbarModule,
    MatButtonModule,
    MatIconModule,
    MatFormFieldModule,
    MatDatepickerModule
  ],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class AppComponent implements OnInit{
  // On ne garde que la logique de l'interface (UI)
  public isDrawerOpen = true;
  public isDarkMode = false;

  private peaGraphService = inject(PeaGraphService);
  private snackBar = inject(MatSnackBar);

  constructor(private exportService: ExportService) {}

  ngOnInit() {
    // Se lancera une seule fois au démarrage de l'app
    this.peaGraphService.updatePricesIfNeeded().subscribe(results => {
      if (results && results.length > 0) {
        this.snackBar.open(`✅ Mise à jour des valeurs PEA (${results.length} titres)`, 'OK', { 
          duration: 3000 
        });
      }
    });
  }

  toggleDrawer() {
    this.isDrawerOpen = !this.isDrawerOpen;
  }

  onFilterChange(startValue: any, endValue: any) {
    // Le picker renvoie souvent des objets Date ou des chaînes selon la config
    // On ne déclenche la mise à jour que si on a les deux bornes
    if (startValue && endValue) {
      const start = new Date(startValue);
      const end = new Date(endValue);

      // Formatage YYYY-MM-DD
      const formattedStart = start.toISOString().split('T')[0];
      const formattedEnd = end.toISOString().split('T')[0];

      filtersService.updateFilters({
        start: formattedStart,
        end: formattedEnd
      });
    }
  }

  resetFilters() {
    filtersService.reset();
  }

  onExportDatabase() {
    this.exportService.downloadDatabaseBackup();
  }
}
