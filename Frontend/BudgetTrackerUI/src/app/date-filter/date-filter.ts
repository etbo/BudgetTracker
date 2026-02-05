import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatChipsModule } from '@angular/material/chips';
import { filtersService } from '../services/filters.service';

@Component({
  selector: 'app-date-filter',
  standalone: true,
  imports: [
    CommonModule, FormsModule, MatDatepickerModule,
    MatIconModule, MatFormFieldModule, MatInputModule, MatChipsModule
  ],
  templateUrl: './date-filter.html',
  styleUrls: ['./date-filter.scss']
})
export class DateFilter implements OnInit {
  @Input() showLastImportOption: boolean = false;
  @Input() showImport: boolean = false;

  @Output() filterChanged = new EventEmitter<{ start: string, end: string, view: string }>();
  @Output() importClicked = new EventEmitter<void>();

  public availableYears: number[] = [2026, 2025, 2024, 2023, 2022, 2021, 2020, 2019, 2018, 2017];
  public selectedYears: number[] = [];
  public currentView: string = 'all';

  public startDate: Date | null = null;
  public endDate: Date | null = null;

  ngOnInit() {
    const current = filtersService.getFilters();
    let initialView = current.view || (this.showLastImportOption ? 'last' : 'last12');

    // Sécurité : fallback si "last" est demandé mais non autorisé
    if (!this.showLastImportOption && initialView === 'last') {
      initialView = 'last12';
    }

    this.currentView = initialView;

    // Restauration des années sélectionnées si la vue est une liste d'années (ex: "2023,2024")
    if (this.currentView.includes(',') || /^\d{4}$/.test(this.currentView)) {
      this.selectedYears = this.currentView.split(',').map(Number);
    }

    if (current.start) this.startDate = new Date(current.start);
    if (current.end) this.endDate = new Date(current.end);

    // Si refresh sur un mode relatif sans dates calculées, on force le calcul
    if (['last3', 'last6', 'last12', 'last24', 'last36'].includes(this.currentView) && !current.start) {
      setTimeout(() => this.onViewChange(this.currentView), 0);
    }
  }

  /**
   * Gère le changement vers une vue relative (Chips du haut)
   */
  onViewChange(view: string) {
    if (this.currentView === view && this.selectedYears.length === 0) return;

    this.currentView = view;
    this.selectedYears = [];

    // Async pour la fluidité
    setTimeout(() => {
      filtersService.updateFilters({ view: view as any });
    }, 0);
  }

  /**
   * Vérifie si une année doit être affichée comme sélectionnée
   */
  isYearSelected(year: number): boolean {
    return this.selectedYears.includes(year);
  }

  /**
 * Une année est "bloquée" si elle est sélectionnée 
 * ET qu'elle n'est ni le début ni la fin du range.
 */
  isYearDisabled(year: number): boolean {
    if (this.selectedYears.length <= 2) return false;

    const min = Math.min(...this.selectedYears);
    const max = Math.max(...this.selectedYears);

    // On disable si l'année est strictement entre le min et le max
    return this.selectedYears.includes(year) && year > min && year < max;
  }

  onYearToggle(year: number) {
    if (this.isYearDisabled(year)) return;

    const isCurrentlySelected = this.selectedYears.includes(year);

    if (!isCurrentlySelected) {
      // AJOUT : On prend les extrêmes avec la nouvelle année
      const all = [...this.selectedYears, year];
      this.rebuildRange(Math.min(...all), Math.max(...all));
    } else {
      // DÉSÉLECTION D'UNE EXTRÉMITÉ : 
      // On enlève l'année et on recalcule le range sur ce qu'il reste
      const remaining = this.selectedYears.filter(y => y !== year);

      if (remaining.length > 0) {
        this.rebuildRange(Math.min(...remaining), Math.max(...remaining));
      } else {
        // Si on a vraiment tout décoché
        this.onViewChange('last6');
        return;
      }
    }

    // Application commune
    this.currentView = this.selectedYears.join(',');
    this.applyYearFilter();
  }

  /**
   * Utilitaire pour reconstruire un tableau continu d'années
   */
  private rebuildRange(min: number, max: number) {
    const range = [];
    for (let y = min; y <= max; y++) {
      range.push(y);
    }
    this.selectedYears = range;
  }

  /**
   * Applique le filtre basé sur la sélection d'années
   */
  private applyYearFilter() {
    const min = Math.min(...this.selectedYears);
    const max = Math.max(...this.selectedYears);

    this.currentView = this.selectedYears.join(',');

    filtersService.updateFilters({
      view: this.currentView as any,
      start: `${min}-01-01`,
      end: `${max}-12-31`
    });
  }

  /**
   * Gestion du mode Custom (Datepickers)
   */
  emitCustom() {
    if (this.startDate && this.endDate) {
      const start = new Date(this.startDate);
      const end = new Date(this.endDate);
      const isYearValid = (d: Date) => d.getFullYear() > 1000 && d.getFullYear() < 3000;

      if (!isNaN(start.getTime()) && !isNaN(end.getTime()) && isYearValid(start) && isYearValid(end)) {
        this.currentView = 'custom';
        this.selectedYears = []; // Désélectionne les années

        this.filterChanged.emit({
          start: this.formatDate(start),
          end: this.formatDate(end),
          view: 'custom'
        });
      }
    }
  }

  private formatDate(date: any): string {
    if (!date) return '';
    const d = (date instanceof Date) ? date : new Date(date);
    if (isNaN(d.getTime())) return '';

    const year = d.getFullYear();
    const month = (d.getMonth() + 1).toString().padStart(2, '0');
    const day = d.getDate().toString().padStart(2, '0');
    return `${year}-${month}-${day}`;
  }
}