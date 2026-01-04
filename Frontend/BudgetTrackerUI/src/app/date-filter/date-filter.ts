import { Component, OnInit } from '@angular/core';
import { filtersService, FilterState } from '../services/filters.service';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-date-filter',
  standalone: true,
  imports: [MatDatepickerModule, MatIconModule, MatFormFieldModule, FormsModule],
  templateUrl: './date-filter.html',
  styleUrls: ['./date-filter.scss']
})
export class DateFilter implements OnInit {
  startDate: Date | null = null;
  endDate: Date | null = null;

  ngOnInit() {
    // On initialise le calendrier avec ce qu'il y a dans l'URL
    const current = filtersService.getFilters();
    if (current.start) this.startDate = new Date(current.start);
    if (current.end) this.endDate = new Date(current.end);
  }

  onDateChange() {
    if (this.startDate && this.endDate) {
      filtersService.updateFilters({
        start: this.formatDate(this.startDate),
        end: this.formatDate(this.endDate)
      });
    }
  }

  private formatDate(date: Date): string {
    const year = date.getFullYear();
    // getMonth() commence à 0, donc on ajoute 1
    const month = (date.getMonth() + 1).toString().padStart(2, '0');
    const day = date.getDate().toString().padStart(2, '0');

    return `${year}-${month}-${day}`;
  }

  reset() {
    this.startDate = null;
    this.endDate = null;
    filtersService.reset();
  }
}