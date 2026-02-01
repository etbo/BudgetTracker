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
import { FilterState, filtersService } from '../services/filters.service';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { CcOperation } from '../models/operation-cc.model';
import { OperationsService } from '../services/operations.service';
import { MatAccordion, MatExpansionPanel, MatExpansionPanelHeader, MatExpansionPanelTitle } from '@angular/material/expansion';
import { MatCard, MatCardHeader, MatCardModule, MatCardSubtitle, MatCardTitle } from '@angular/material/card';
import { CcMacroCategoriesMonthly } from '../charts/cc-macro-categories-monthly/cc-macro-categories-monthly';
import { distinctUntilChanged } from 'rxjs';

@Component({
  selector: 'app-cc-dashboard',
  standalone: true,
  imports: [
    CommonModule, CcEvolutionChart, PieChart, TreemapColor, CcMonthlySummary,
    DateFilter, CcOperationsList, CcMacroCategoriesMonthly, MatButtonModule,
    MatIconModule, MatExpansionPanelTitle, MatExpansionPanelHeader,
    MatExpansionPanel, MatCardHeader, MatCardTitle, MatCard, MatAccordion,
    MatChipsModule, MatCardModule
  ],
  templateUrl: './cc-dashboard.html',
  styleUrl: './cc-dashboard.scss'
})
export class CcDashboard implements OnInit {

  // --- 1. Signaux de filtrage Local (Dashboard uniquement) ---
  public ignoreNeutre = signal(localStorage.getItem('dash_ignore_neutre') !== 'false');
  public ignoreInvest = signal(localStorage.getItem('dash_ignore_invest') !== 'false');

  // --- 2. Signaux de données ---
  public currentFilters = signal<FilterState>(filtersService.getFilters());
  public evolutionData = signal<DailyBalance[]>([]);
  public categoryData = signal<CategoryBalance[]>([]);
  public isLoading = signal(true);
  public operations = signal<CcOperation[]>([]);
  public zoomFilters = signal<FilterState | undefined>(undefined);

  // --- 3. Propriétés calculées (Computed) ---
  public onlyExpensesData = computed(() => {
    return this.categoryData()
      .filter(item => item.expenses < 0)
      .map(item => ({ category: item.category, total: Math.abs(item.expenses) }));
  });

  public onlyRevenuesData = computed(() => {
    return this.categoryData()
      .filter(item => item.incomes > 0)
      .map(item => ({ category: item.category, total: Math.abs(item.incomes) }));
  });

  public totalRevenues = computed(() => this.categoryData().reduce((acc, item) => acc + (item.incomes || 0), 0));
  public totalExpenses = computed(() => this.categoryData().reduce((acc, item) => acc + (item.expenses || 0), 0));
  public balance = computed(() => this.totalRevenues() + this.totalExpenses());

  public expensesForPie = computed(() => this.onlyExpensesData().map(d => ({ label: d.category, value: d.total })));
  public revenuesForPie = computed(() => this.categoryData().filter(d => d.incomes > 0).map(d => ({ label: d.category, value: d.incomes })));

  constructor(
    private balanceService: BalanceService,
    private operationsService: OperationsService
  ) { }

  ngOnInit() {
    // Écoute les changements globaux (URL, Filtres de période)
    filtersService.filters$.pipe(
      distinctUntilChanged((prev, curr) => JSON.stringify(prev) === JSON.stringify(curr))
    ).subscribe((newFilters) => {
      this.currentFilters.set(newFilters);
      this.loadAllData();
    });

    // Synchro initiale
    const current = filtersService.getFilters();
    filtersService.updateFilters(current);
  }

  /**
   * Cœur de la logique : Charge toutes les données en appliquant 
   * les filtres globaux + les filtres locaux du Dashboard.
   */
  public loadAllData() {
    this.isLoading.set(true);
    const fullFilters = filtersService.getFilters();

    // Préparation des exclusions Dashboard
    const localExclusions: string[] = [];
    if (this.ignoreNeutre()) localExclusions.push('Neutre');
    if (this.ignoreInvest()) localExclusions.push('Investissement', 'Virement interne');

    const dashboardFilters: FilterState = {
      ...fullFilters,
      onlyCheques: false,   // Ignoré sur Dashboard
      suggestedCat: false,  // Ignoré sur Dashboard
      missingCat: false,    // Ignoré sur Dashboard
      excludedCategories: [
        ...(fullFilters.excludedCategories || []),
        ...localExclusions
      ]
    };

    // 1. Appel Evolution (Graphique linéaire)
    this.balanceService.getEvolution().subscribe(data => {
      this.evolutionData.set(data);
      this.isLoading.set(false);
    });

    // 2. Appel Balances par Category (Pie Chart / Treemap)
    this.balanceService.getAllCategoryBalances().subscribe(data => {
      // Filtrage manuel des catégories ignorées localement
      const filtered = data.filter(c => !localExclusions.includes(c.category));
      this.categoryData.set(filtered);
    });

    // 3. Appel Opérations (Tableau Zoom et Macro Graphes)
    this.operationsService.getOperations(dashboardFilters).subscribe(data => {
      this.operations.set(data);
    });

    // 4. Update des filtres pour le tableau de zoom
    this.zoomFilters.set({ ...fullFilters });
  }

  // --- 4. Actions Locales (Chips Dashboard) ---

  toggleDashNeutre() {
    const newVal = !this.ignoreNeutre();
    this.ignoreNeutre.set(newVal);
    localStorage.setItem('dash_ignore_neutre', newVal.toString());
    this.loadAllData();
  }

  toggleDashInvest() {
    const newVal = !this.ignoreInvest();
    this.ignoreInvest.set(newVal);
    localStorage.setItem('dash_ignore_invest', newVal.toString());
    this.loadAllData();
  }

  // --- 5. Actions Globales (Navigation / Zoom) ---

  onPeriodZoomed(event: { min: number, max: number }) {
    if (event.min === 0 && event.max === 0) {
      this.resetZoom();
      return;
    }
    filtersService.updateFilters({
      ...filtersService.getFilters(),
      start: new Date(event.min).toISOString().split('T')[0],
      end: new Date(event.max).toISOString().split('T')[0],
      view: 'custom'
    });
  }

  resetZoom() {
    this.zoomFilters.set(undefined);
    this.loadAllData();
  }

  onGlobalFilterChanged(event: { start: string, end: string, view: string }) {
    filtersService.updateFilters({
      ...filtersService.getFilters(),
      start: event.start,
      end: event.end,
      view: event.view
    });
    this.resetZoom();
  }

  // --- 6. Helpers et Computed pour Graphiques complexes ---

  public macroMonthlyData = computed(() => {
    const filters = this.currentFilters();
    if (!filters.start || !filters.end) return [];

    const startDate = new Date(filters.start);
    const endDate = new Date(filters.end);
    const groups: Record<string, any> = {};

    let cursor = new Date(startDate.getFullYear(), startDate.getMonth(), 1);
    const endLimit = new Date(endDate.getFullYear(), endDate.getMonth(), 1);

    while (cursor <= endLimit) {
      const monthKey = cursor.toLocaleString('fr-FR', { month: 'short', year: '2-digit' });
      groups[monthKey] = {
        month: monthKey,
        sortDate: new Date(cursor.getTime()),
        obligatoire: 0, loisir: 0, invest: 0, inconnu: 1
      };
      cursor.setMonth(cursor.getMonth() + 1);
    }

    const ops = this.operations().filter(o => o.amount < 0);
    ops.forEach(op => {
      const date = new Date(op.date);
      const monthKey = date.toLocaleString('fr-FR', { month: 'short', year: '2-digit' });

      if (groups[monthKey]) {
        const amount = Math.abs(op.amount);
        const macro = op.macroCategory;
        if (macro === 'Obligatoire') groups[monthKey].obligatoire += amount;
        else if (macro === 'Loisir') groups[monthKey].loisir += amount;
        else if (macro === 'Investissement') groups[monthKey].invest += amount;
        else groups[monthKey].inconnu += amount;
      }
    });

    return Object.values(groups).sort((a, b) => a.sortDate.getTime() - b.sortDate.getTime());
  });
}