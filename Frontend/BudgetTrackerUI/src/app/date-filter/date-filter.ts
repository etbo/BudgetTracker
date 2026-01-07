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
  // Configuration d'affichage restante
  @Input() showLastImportOption: boolean = false;
  @Input() showImport: boolean = false;

  // L'événement envoie maintenant explicitement la "view"
  @Output() filterChanged = new EventEmitter<{ start: string, end: string, view: string }>();
  @Output() importClicked = new EventEmitter<void>();

  startDate: Date | null = null;
  endDate: Date | null = null;
  currentView: string = 'all'; // Renommé pour plus de clarté

  ngOnInit() {
    const current = filtersService.getFilters();

    // Initialisation depuis l'URL au chargement
    this.currentView = current.view || (this.showLastImportOption ? 'last' : 'all');

    if (current.start) this.startDate = new Date(current.start);
    if (current.end) this.endDate = new Date(current.end);

    // Si on refresh sur un mode calculé (ex: last6), on s'assure que les dates sont prêtes
    if (['last3', 'last6', 'last12'].includes(this.currentView) && !current.start) {
      this.onViewChange(this.currentView);
    }
  }

  onViewChange(viewName: string) {

    if (this.currentView === viewName && viewName !== 'custom') {
      return;
    }

    this.currentView = viewName;

    let startStr = '';
    let endStr = '';

    if (viewName === 'last') {
      this.startDate = null;
      this.endDate = null;
      startStr = 'last';
      endStr = 'last';
    }
    else if (viewName === 'all') {
      this.startDate = null;
      this.endDate = null;
    }
    else if (viewName !== 'custom') {
      // Modes calculés : last3, last6, last12
      startStr = this.calculateStartDate(viewName);
      console.log('startStr', startStr);
      endStr = this.formatDate(new Date());
      this.startDate = new Date(startStr);
      this.endDate = new Date(endStr);
    }

    this.filterChanged.emit({ start: startStr, end: endStr, view: viewName });
  }

  private calculateStartDate(viewName: string): string {
    const now = new Date();
    let start = new Date();
    if (viewName === 'last1') start.setMonth(now.getMonth() - 1);
    if (viewName === 'last3') start.setMonth(now.getMonth() - 3);
    if (viewName === 'last6') start.setMonth(now.getMonth() - 6);
    if (viewName === 'last12') start.setMonth(now.getMonth() - 12);
    return this.formatDate(start);
  }

  emitCustom() {
    // On vérifie que les deux dates existent
    if (this.startDate && this.endDate) {

      // On vérifie que les dates sont des objets valides et ont une année cohérente
      const start = new Date(this.startDate);
      const end = new Date(this.endDate);

      // On vérifie que l'année est complète
      const isYearValid = (d: Date) => d.getFullYear() > 1000 && d.getFullYear() < 3000;

      if (!isNaN(start.getTime()) && !isNaN(end.getTime()) && isYearValid(start) && isYearValid(end)) {
        this.currentView = 'custom';
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

    // On s'assure d'avoir un objet Date valide
    const d = (date instanceof Date) ? date : new Date(date);

    // Sécurité si la conversion échoue
    if (isNaN(d.getTime())) return '';

    const year = d.getFullYear(); // 2022
    // On ajoute un zéro devant si le mois ou le jour est < 10
    const month = (d.getMonth() + 1).toString().padStart(2, '0');
    const day = d.getDate().toString().padStart(2, '0');

    // Retourne impérativement YYYY-MM-DD
    return `${year}-${month}-${day}`;
  }
}