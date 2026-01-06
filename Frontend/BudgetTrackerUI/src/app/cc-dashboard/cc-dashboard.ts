import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BalanceService } from '../services/balance.service';
import { CcEvolutionChart } from '../charts/cc-evolution-chart/cc-evolution-chart';
import { DailyBalance } from '../models/daily-balance';

@Component({
  selector: 'app-cc-dashboard',
  standalone: true,
  imports: [CommonModule, CcEvolutionChart],
  templateUrl: './cc-dashboard.html',
  styleUrl: './cc-dashboard.scss'
})
export class CcDashboard implements OnInit {

  public evolutionData = signal<DailyBalance[]>([]);
  public isLoading = signal(true);

  constructor(private balanceService: BalanceService) { }

  ngOnInit() {
    this.isLoading.set(true);
    // Un seul appel pour tout le monde ou des appels groupés
    this.balanceService.getEvolution().subscribe(data => {
      this.evolutionData.set(data);
      this.isLoading.set(false);
    });
  }

}