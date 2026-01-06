import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BalanceService } from '../services/balance.service';
import { CcEvolutionChart } from '../charts/cc-evolution-chart/cc-evolution-chart';
import { PieChart } from '../charts/pie-chart/pie-chart';
import { DailyBalance } from '../models/daily-balance.model';
import { CategoryBalance } from '../models/category-balance.model';
import { DateFilter } from '../date-filter/date-filter';

@Component({
  selector: 'app-cc-dashboard',
  standalone: true,
  imports: [CommonModule, CcEvolutionChart, PieChart, DateFilter],
  templateUrl: './cc-dashboard.html',
  styleUrl: './cc-dashboard.scss'
})
export class CcDashboard implements OnInit {

  public evolutionData = signal<DailyBalance[]>([]);
  public categoryData = signal<CategoryBalance[]>([]);
  public isLoading = signal(true);

  constructor(private balanceService: BalanceService) { }

  ngOnInit() {
    this.isLoading.set(true);
    // Un seul appel pour tout le monde ou des appels groupés
    this.balanceService.getEvolution().subscribe(data => {
      this.evolutionData.set(data);
      this.isLoading.set(false);
    });

    this.balanceService.getExpensesByCategory().subscribe(res => {
      this.categoryData.set(res);
    });
  }

  onPeriodZoomed(event: { min: number, max: number }) {
    // Si min et max sont à 0, c'est un reset
    if (event.min === 0 && event.max === 0) {
      this.balanceService.getExpensesByCategory().subscribe(data => {
        this.categoryData.set(data);
      });
      return;
    }

    // Sinon, on fait le filtrage normal
    const start = new Date(event.min).toISOString();
    const end = new Date(event.max).toISOString();

    this.balanceService.getExpensesByCategory(start, end).subscribe(data => {
      this.categoryData.set(data);
    });
  }

}