import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BalanceService } from '../services/balance.service';
import { CcEvolutionChart } from '../charts/cc-evolution-chart/cc-evolution-chart';
import { PieChart } from '../charts/pie-chart/pie-chart';
import { DailyBalance } from '../models/daily-balance.model';
import { CategoryBalance } from '../models/category-balance.model';
import { DateFilter } from '../date-filter/date-filter';
import { CcOperationsList } from '../cc-operations-list/cc-operations-list';
import { FilterState, filtersService } from '../services/filters.service'; // Import nécessaire
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-cc-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    CcEvolutionChart,
    PieChart,
    DateFilter,
    CcOperationsList,
    MatButtonModule,
    MatIconModule
  ],
  templateUrl: './cc-dashboard.html',
  styleUrl: './cc-dashboard.scss'
})
export class CcDashboard implements OnInit {

  public evolutionData = signal<DailyBalance[]>([]);
  public categoryData = signal<CategoryBalance[]>([]);
  public isLoading = signal(true);

  // Variable pour piloter la grille au zoom
  public zoomFilters = signal<FilterState | undefined>(undefined);

  constructor(private balanceService: BalanceService) { }

  ngOnInit() {
    this.loadAllData();

    // On écoute aussi les changements de filtres globaux (puces du haut)
    // pour réinitialiser le zoom si l'utilisateur change de période globale
    window.addEventListener('filterChanged', () => {
      this.resetZoom();
      this.loadAllData();
    });
  }

  private loadAllData() {
    this.isLoading.set(true);
    const filters = filtersService.getFilters();

    // On définit des dates par défaut si filters.start ou end sont undefined
    // Par exemple, une plage très large ou la date du jour
    const startDate = filters.start || '1900-01-01';
    const endDate = filters.end || '2099-12-31';

    this.balanceService.getEvolution().subscribe(data => {
      // Le filtre est maintenant sécurisé
      const filteredData = data.filter(item => {
        return item.date >= startDate && item.date <= endDate;
      });

      this.evolutionData.set(filteredData);
      this.isLoading.set(false);
    });

    // Pour l'API, on passe aussi les dates sécurisées
    this.balanceService.getExpensesByCategory(startDate, endDate).subscribe(res => {
      this.categoryData.set(res);
    });
  }

  onPeriodZoomed(event: { min: number, max: number }) {
    // Si min et max sont à 0, c'est un reset du zoom
    if (event.min === 0 && event.max === 0) {
      this.resetZoom();
      return;
    }

    // Formatage en YYYY-MM-DD pour le service et la grille
    const startStr = new Date(event.min).toISOString().split('T')[0];
    const endStr = new Date(event.max).toISOString().split('T')[0];

    // 1. Mise à jour du Pie Chart
    this.balanceService.getExpensesByCategory(startStr, endStr).subscribe(data => {
      this.categoryData.set(data);
    });

    // 2. Mise à jour de la liste des opérations via l'Input
    this.zoomFilters.set({
      ...filtersService.getFilters(),
      start: startStr,
      end: endStr,
      view: 'custom'
    });
  }

  resetZoom() {
    this.zoomFilters.set(undefined);
    const filters = filtersService.getFilters();
    this.balanceService.getExpensesByCategory(filters.start, filters.end).subscribe(data => {
      this.categoryData.set(data);
    });
  }

  onGlobalFilterChanged(event: { start: string, end: string, view: string }) {
    // 1. On met à jour le service global (ce qui change l'URL et avertit les autres composants)
    filtersService.updateFilters({
      start: event.start,
      end: event.end,
      view: event.view
    });

    // 2. On réinitialise le zoom pour éviter les conflits
    this.resetZoom();

    // 3. On recharge les données des graphiques pour cette nouvelle période globale
    this.loadAllData();
  }
}