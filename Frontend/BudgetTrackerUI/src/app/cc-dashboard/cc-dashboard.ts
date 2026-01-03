import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NgApexchartsModule } from "ng-apexcharts";
import { BalanceService } from '../services/balance.service';

@Component({
  selector: 'app-cc-dashboard',
  standalone: true,
  imports: [CommonModule, NgApexchartsModule],
  templateUrl: './cc-dashboard.html',
  styleUrl: './cc-dashboard.scss'
})
export class CcDashboard implements OnInit {
  public chartOptions: any;
  public isLoading = signal(true);

  constructor(private balanceService: BalanceService) {}

  ngOnInit() {
    this.balanceService.getEvolution().subscribe({
      next: (data) => {
        this.setupChart(data);
        this.isLoading.set(false);
      },
      error: (err) => {
        console.error("Erreur API:", err);
        this.isLoading.set(false);
      }
    });
  }

  setupChart(data: any[]) {
    this.chartOptions = {
      series: [{
        name: "Solde cumulé",
        data: data.map(d => d.cumulatedBalance)
      }],
      chart: { 
        type: "line", 
        height: 450,
        toolbar: { show: true },
        zoom: { enabled: true }
      },
      xaxis: {
        type: 'category',
        CcCategories: data.map(d => new Date(d.date).toLocaleDateString('fr-FR')),
        title: { text: 'Date' }
      },
      yaxis: {
        labels: {
          formatter: (val: number) => val.toFixed(2) + " €"
        },
        title: { text: 'Montant' }
      },
      stroke: { curve: "smooth", width: 3 },
      colors: ["#594ae2"], // Le violet de votre thème MudBlazor
      title: { 
        text: "Évolution du compte courant", 
        align: "center" 
      },
      grid: {
        row: {
          colors: ["#f3f3f3", "transparent"],
          opacity: 0.5
        }
      }
    };
  }
}