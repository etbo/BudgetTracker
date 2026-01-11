import { Component, computed, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BalanceService } from '../services/balance.service';
import { CcEvolutionChart } from '../charts/cc-evolution-chart/cc-evolution-chart';
import { PieChart } from '../charts/pie-chart/pie-chart';
import { TreemapColor } from '../charts/treemap-color/treemap-color';
import { CcMonthlySummary } from '../charts/cc-monthly-summary/cc-monthly-summary';
import { DailyBalance } from '../models/daily-balance.model';
import { CategoryBalance } from '../models/category-balance.model';
import { DateFilter } from '../date-filter/date-filter';
import { CcOperationsList } from '../cc-operations-list/cc-operations-list';
import { FilterState, filtersService } from '../services/filters.service'; // Import nécessaire
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { CcOperation } from '../models/operation-cc.model';
import { OperationsService } from '../services/operations.service';
import { MatAccordion, MatExpansionPanel, MatExpansionPanelHeader, MatExpansionPanelTitle } from '@angular/material/expansion';
import { MatCard, MatCardHeader, MatCardSubtitle, MatCardTitle } from '@angular/material/card';

@Component({
  selector: 'app-cc-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    CcEvolutionChart,
    PieChart,
    TreemapColor,
    CcMonthlySummary,
    DateFilter,
    CcOperationsList,
    MatButtonModule,
    MatIconModule,
    MatExpansionPanelTitle,
    MatExpansionPanelHeader,
    MatExpansionPanel,
    MatCardSubtitle,
    MatCardHeader,
    MatCardTitle,
    MatCard,
    MatAccordion,
    MatChipsModule
  ],
  templateUrl: './cc-dashboard.html',
  styleUrl: './cc-dashboard.scss'
})
export class CcDashboard implements OnInit {

  public evolutionData = signal<DailyBalance[]>([]);
  public categoryData = signal<CategoryBalance[]>([]);
  public isLoading = signal(true);

  operations = signal<CcOperation[]>([]);

  // Variable pour piloter la grille au zoom
  public zoomFilters = signal<FilterState | undefined>(undefined);

  public onlyExpensesData = computed(() => {
    return this.categoryData()
      .filter(item => item.total < 0) // 1. On ne garde que les dépenses
      .map(item => ({
        ...item,                   // 2. On garde toutes les propriétés (category, etc.)
        total: Math.abs(item.total) // 3. On remplace le total par sa valeur absolue
      }));
  });

  // Calcul pour total revenus, total dépenses et la balance : 
  public totalRevenues = computed(() => {
    return this.categoryData()
      .filter(item => item.total > 0)
      .reduce((acc, item) => acc + item.total, 0);
  });

  public totalExpenses = computed(() => {
    return Math.abs(this.categoryData()
      .filter(item => item.total < 0)
      .reduce((acc, item) => acc + item.total, 0));
  });

  public balance = computed(() => this.totalRevenues() - this.totalExpenses());

  public onlyRevenuesData = computed(() => {
    return this.categoryData().filter(item => item.total > 0);
  });

  constructor(
    private balanceService: BalanceService,
    private operationsService: OperationsService

  ) { }

  ngOnInit() {
    // Charge les données initiales
    this.loadAllData();

  }

  private refreshFromGlobal() {
    // Une version légère qui ne fait que rafraîchir sans redéclencher d'événements
    this.loadAllData();
  }

  public loadAllData() {
    this.isLoading.set(true);

    // 1. Evolution (Déjà OK)
    this.balanceService.getEvolution().subscribe(data => {
      this.evolutionData.set(data);
      this.isLoading.set(false);
    });

    // 2. Valeurs par catégories (backend)
    this.balanceService.getAllCategoryBalances().subscribe(data => {
      this.categoryData.set(data);
    });

    this.operationsService.getOperations().subscribe(data => {
      this.operations.set(data);
    });

    // 3. Update les filtres pour le tableau
    this.zoomFilters.set({ ...filtersService.getFilters() });
  }

  onPeriodZoomed(event: { min: number, max: number }) {
    if (event.min === 0 && event.max === 0) {
      this.resetZoom();
      return;
    }

    const startStr = new Date(event.min).toISOString().split('T')[0];
    const endStr = new Date(event.max).toISOString().split('T')[0];

    // 1. On met à jour le service global pour que le prochain appel service.getExpensesByCategory() 
    // utilise ces nouvelles dates automatiquement
    filtersService.updateFilters({
      ...filtersService.getFilters(),
      start: startStr,
      end: endStr,
      view: 'custom'
    });

    // 2. On appelle simplement loadAllData() qui va rafraîchir le Pie Chart et l'Evolution
    // avec les filtres que nous venons de mettre à jour.
    this.loadAllData();
  }

  resetZoom() {
    // On remet la vue par défaut (ex: last6) via le service
    filtersService.updateFilters({
      ...filtersService.getFilters(),
      view: 'last6', // Ou ta valeur par défaut
      // Note: Le service calculera automatiquement les dates start/end pour last6
    });

    this.zoomFilters.set(undefined);
    this.loadAllData();
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

  isCategoryExcluded(cat: string): boolean {
    return filtersService.getFilters().excludedCategories?.includes(cat) ?? false;
  }

  toggleCategoryExclusion(cat: string) {
    const currentFilters = filtersService.getFilters();
    let excluded = [...(currentFilters.excludedCategories || [])];

    if (excluded.includes(cat)) {
      excluded = excluded.filter(c => c !== cat);
    } else {
      excluded.push(cat);
    }

    // On crée un nouvel objet complet
    const newFilters: FilterState = {
      ...currentFilters,
      excludedCategories: excluded
    };

    // On met à jour le service
    filtersService.updateFilters(newFilters);

    // On recharge
    this.loadAllData();
  }
}