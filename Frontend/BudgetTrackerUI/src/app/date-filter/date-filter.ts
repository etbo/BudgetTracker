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
    CommonModule, 
    FormsModule, 
    MatDatepickerModule, 
    MatIconModule, 
    MatFormFieldModule, 
    MatInputModule,
    MatChipsModule
  ],
  templateUrl: './date-filter.html',
  styleUrls: ['./date-filter.scss']
})
export class DateFilter implements OnInit {
  // Paramètres de configuration selon la page
  @Input() mode: 'detailed' | 'quick' = 'detailed';
  @Input() showLastImportOption: boolean = false;

  @Output() filterChanged = new EventEmitter<{ start: string, end: string }>();
  @Output() importClicked = new EventEmitter<void>();

  startDate: Date | null = null;
  endDate: Date | null = null;
  currentMode: string = 'all';

  ngOnInit() {
    // Initialisation depuis le service de filtres (URL)
    const current = filtersService.getFilters();
    if (current.start) {
      this.startDate = new Date(current.start);
      this.currentMode = 'custom'; // Si on a des dates précises, on active le mode custom
    }
    if (current.end) this.endDate = new Date(current.end);
  }

  onChipChange(event: any) {
    this.currentMode = event.value;

    if (this.currentMode === 'custom') {
      return; // On attend que l'utilisateur saisisse les dates dans le calendrier
    }

    if (this.currentMode === 'last') {
      // Logique spécifique pour le dernier import (via le service ou un flag)
      this.filterChanged.emit({ start: 'last', end: 'last' });
      return;
    }

    this.updateDates();
  }

  updateDates() {
    const now = new Date();
    let start = new Date();

    switch (this.currentMode) {
      case 'last3':
        start.setMonth(now.getMonth() - 3);
        break;
      case 'last6':
        start.setMonth(now.getMonth() - 6);
        break;
      case 'last12':
        start.setMonth(now.getMonth() - 12);
        break;
      case 'all':
        start = new Date(2000, 0, 1);
        break;
      case 'custom':
        if (this.startDate && this.endDate) {
           this.filterChanged.emit({
             start: this.formatDate(this.startDate),
             end: this.formatDate(this.endDate)
           });
        }
        return;
    }

    this.filterChanged.emit({
      start: this.formatDate(start),
      end: this.formatDate(now)
    });
  }

  // Émet les dates personnalisées quand elles changent
  emitCustom() {
    if (this.startDate && this.endDate) {
      this.updateDates();
    }
  }

  private formatDate(date: Date): string {
    const year = date.getFullYear();
    const month = (date.getMonth() + 1).toString().padStart(2, '0');
    const day = date.getDate().toString().padStart(2, '0');
    return `${year}-${month}-${day}`;
  }

  reset() {
    this.startDate = null;
    this.endDate = null;
    this.currentMode = 'all';
    filtersService.reset();
    this.updateDates();
  }
}