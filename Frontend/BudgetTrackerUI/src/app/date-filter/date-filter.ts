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

    console.log("showLastImportOption = ", this.showLastImportOption);

    let initialView = current.view;

    // Si on arrive avec "last" mais que l'option est désactivée pour ce composant
    if (!this.showLastImportOption && initialView === 'last') {
      initialView = 'last6'; // On force sur last6 par exemple
    }

    this.currentView = initialView || (this.showLastImportOption ? 'last' : 'last6');

    if (current.start) this.startDate = new Date(current.start);
    if (current.end) this.endDate = new Date(current.end);

    // Si on refresh sur un mode calculé (ex: last6), on s'assure que les dates sont prêtes
    if (['last3', 'last6', 'last12'].includes(this.currentView) && !current.start) {
      setTimeout(() => {
        this.onViewChange(this.currentView, true);
      }, 0);
    }
  }

  onViewChange(viewName: string, force: boolean = false) {

    // On ne bloque que si ce n'est pas forcé
    if (!force && this.currentView === viewName && viewName !== 'custom') {
      return;
    }

    this.currentView = viewName;
    console.log("Initialisation URL forcée pour :", viewName);

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
      const now = new Date();

      // FIN : Le dernier jour du mois PRÉCÉDENT
      // En mettant le jour à 0 sur le mois actuel, JS revient au dernier jour du mois d'avant
      const lastDayLastMonth = new Date(now.getFullYear(), now.getMonth(), 0);
      endStr = this.formatDate(lastDayLastMonth);

      // DÉBUT : Le 1er jour du mois il y a X mois
      const monthsToGoBack = parseInt(viewName.replace('last', ''));
      const firstDayStartMonth = new Date(now.getFullYear(), now.getMonth() - monthsToGoBack, 1);
      startStr = this.formatDate(firstDayStartMonth);

      this.startDate = firstDayStartMonth;
      this.endDate = lastDayLastMonth;
    }

    this.filterChanged.emit({ start: startStr, end: endStr, view: viewName });
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