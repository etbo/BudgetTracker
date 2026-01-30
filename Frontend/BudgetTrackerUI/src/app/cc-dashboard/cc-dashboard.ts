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
import { MatCard, MatCardHeader, MatCardModule, MatCardSubtitle, MatCardTitle } from '@angular/material/card';
import { CcMacroCategoriesMonthly } from '../charts/cc-macro-categories-monthly/cc-macro-categories-monthly';
import { distinctUntilChanged } from 'rxjs';

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
    CcMacroCategoriesMonthly,
    MatButtonModule,
    MatIconModule,
    MatExpansionPanelTitle,
    MatExpansionPanelHeader,
    MatExpansionPanel,
    MatCardHeader,
    MatCardTitle,
    MatCard,
    MatAccordion,
    MatChipsModule,
    MatCardModule
  ],
  templateUrl: './cc-dashboard.html',
  styleUrl: './cc-dashboard.scss'
})
export class CcDashboard implements OnInit {

  public currentFilters = signal<FilterState>(filtersService.getFilters());

  public evolutionData = signal<DailyBalance[]>([]);
  public categoryData = signal<CategoryBalance[]>([]);
  public isLoading = signal(true);

  operations = signal<CcOperation[]>([]);

  // Variable pour piloter la grille au zoom
  public zoomFilters = signal<FilterState | undefined>(undefined);

  public onlyExpensesData = computed(() => {
    return this.categoryData()
      .filter(item => item.expenses < 0)
      .map(item => ({
        category: item.category,
        total: Math.abs(item.expenses)
      }));
  });

  public onlyRevenuesData = computed(() => {
    return this.categoryData()
      .filter(item => item.incomes > 0)
      .map(item => ({
        category: item.category,
        total: Math.abs(item.incomes)
      }));
  });

  public totalRevenues = computed(() => {
    return this.categoryData()
      // On additionne le champ 'incomes' de CHAQUE catégorie
      .reduce((acc, item) => acc + (item.incomes || 0), 0);
  });

  public totalExpenses = computed(() => {
    return this.categoryData()
      // On additionne le champ 'incomes' de CHAQUE catégorie
      .reduce((acc, item) => acc + (item.expenses || 0), 0);
  });

  public balance = computed(() => this.totalRevenues() + this.totalExpenses());

  public expensesForPie = computed(() => {
    return this.onlyExpensesData().map(d => ({
      label: d.category,
      value: d.total // Déjà en Math.abs() via onlyExpensesData
    }));
  });

  public revenuesForPie = computed(() => {
    return this.categoryData()
      .filter(d => d.incomes > 0)
      .map(d => ({
        label: d.category,
        value: d.incomes
      }));
  });

  constructor(
    private balanceService: BalanceService,
    private operationsService: OperationsService

  ) { }

  ngOnInit() {
    filtersService.filters$.pipe(
      // On ne déclenche que si l'objet JSON des filtres est différent du précédent
      distinctUntilChanged((prev, curr) => JSON.stringify(prev) === JSON.stringify(curr))
    ).subscribe((newFilters) => {
      this.currentFilters.set(newFilters);
      this.loadAllData();
    });

    // Force la synchro de l'URL UNE SEULE FOIS au démarrage
    const current = filtersService.getFilters();
    filtersService.updateFilters(current);
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

  public macroMonthlyData = computed(() => {
    // 1. On récupère les filtres pour connaître la plage de dates demandée
    const filters = this.currentFilters();
    if (!filters.start || !filters.end) return [];

    const startDate = new Date(filters.start);
    const endDate = new Date(filters.end);

    const groups: Record<string, any> = {};

    // 2. CRÉATION DE LA TIMELINE FORCÉE
    // On initialise chaque mois de la période avec 1€ en "inconnu"
    let cursor = new Date(startDate.getFullYear(), startDate.getMonth(), 1);
    const endLimit = new Date(endDate.getFullYear(), endDate.getMonth(), 1);

    while (cursor <= endLimit) {
      const monthKey = cursor.toLocaleString('fr-FR', { month: 'short', year: '2-digit' });
      groups[monthKey] = {
        month: monthKey,
        sortDate: new Date(cursor.getTime()),
        obligatoire: 0,
        loisir: 0,
        invest: 0,
        inconnu: 1 // <--- Ton idée : 1€ forcé pour que le mois existe visuellement
      };
      cursor.setMonth(cursor.getMonth() + 1);
    }

    // 3. REMPLISSAGE AVEC LES OPÉRATIONS RÉELLES
    const ops = this.operations().filter(o => o.montant < 0);

    ops.forEach(op => {
      const date = new Date(op.date);
      const monthKey = date.toLocaleString('fr-FR', { month: 'short', year: '2-digit' });

      if (groups[monthKey]) {
        // Si on a de la vraie donnée, on peut enlever le 1€ de sécurité 
        // ou simplement l'ajouter, à ce stade 1€ ne changera pas ton graph à 100%
        const amount = Math.abs(op.montant);
        const macro = op.macroCategory;

        if (macro === 'Obligatoire') groups[monthKey].obligatoire += amount;
        else if (macro === 'Loisir') groups[monthKey].loisir += amount;
        else if (macro === 'Investissement') groups[monthKey].invest += amount;
        else groups[monthKey].inconnu += amount;
      }
    });

    var result = Object.values(groups).sort((a, b) => a.sortDate.getTime() - b.sortDate.getTime());
    console.log('result = ', result)

    return result;
  });
}