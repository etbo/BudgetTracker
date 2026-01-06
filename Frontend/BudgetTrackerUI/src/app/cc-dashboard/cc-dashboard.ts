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

  constructor(private balanceService: BalanceService) { }

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
    // On s'assure que les données sont triées par date
    const sortedData = [...data].sort((a, b) => new Date(a.date).getTime() - new Date(b.date).getTime());

    this.chartOptions = {
      series: [{
        name: "Solde (€)",
        data: sortedData.map(d => ({
          x: new Date(d.date).getTime(),
          y: Number(d.cumulatedBalance.toFixed(2))
        }))
      }],
      chart: {
        type: "area",
        height: 600,
        zoom: { type: 'x', enabled: true, autoScaleYaxis: true },
        toolbar: { show: true }
      },
      // FORCE LE MASQUAGE ICI
      dataLabels: {
        enabled: false
      },
      // Supprime les petits cercles sur chaque point qui saturent le graph
      markers: {
        size: 0,
        hover: { size: 5 }
      },
      xaxis: {
        type: 'datetime',
        labels: {
          datetimeUTC: false,
          format: 'dd/MM/yyyy' // Format plus court pour l'axe
        },
        tickAmount: 8 // Limite le nombre de dates affichées sur l'axe X
      },
      yaxis: {
        labels: {
          formatter: (val: number) => Math.round(val).toLocaleString('fr-FR') + " €"
        }
      },
      stroke: { width: 2 },
      colors: ["#594ae2"]
    };
  }
}