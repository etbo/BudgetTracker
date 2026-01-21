import { Component, OnInit, signal, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { PeaService } from '../services/pea.service';
import { PeaGraphService } from '../services/peagraph.service';
import { NgApexchartsModule, ChartComponent } from "ng-apexcharts";
import { MatCardModule } from '@angular/material/card';

@Component({
  selector: 'app-pea-dashboard',
  standalone: true,
  imports: [CommonModule, MatSnackBarModule, NgApexchartsModule, MatCardModule],
  templateUrl: './pea-dashboard.html'
})
export class PeaDashboard implements OnInit {
  listeCumulsPea = signal<any[]>([]);
  chartSeries = signal<any[]>([]);
  isLoading = signal(true);

  // Configuration ApexCharts
  public chartOptions: any;

  constructor(
    private peaService: PeaService, 
    private peaGraphService: PeaGraphService,
    private snackBar: MatSnackBar
  ) {
    this.initChartOptions();
  }

  ngOnInit() {
    this.updatePricesAndLoadData();
  }

  private updatePricesAndLoadData() {
    // 1. Déclencher la mise à jour des prix
    this.peaGraphService.updatePrices().subscribe(results => {
      results.forEach(res => {
        this.snackBar.open(`${res.ticker}: ${res.message}`, 'Fermer', { 
            duration: 3000, 
            panelClass: res.status === 'Success' ? 'bg-success' : 'bg-error' 
        });
      });

      // 2. Charger les données de cumul
      this.loadCumulData();
    });
  }

  private loadCumulData() {
    this.peaGraphService.getCalculerCumul().subscribe(data => {
      this.listeCumulsPea.set(data);
      this.processChartData(data);
      this.isLoading.set(false);
    });
  }

  private processChartData(data: any[]) {
    const sortedData = [...data].sort((a, b) => new Date(a.date).getTime() - new Date(b.date).getTime());

    const series = [
      {
        name: "Gains",
        data: sortedData.map(d => ({ x: new Date(d.date).getTime(), y: Math.round(d.valeurTotale - d.achatCumules) }))
      },
      {
        name: "Cumul Achat",
        data: sortedData.map(d => ({ x: new Date(d.date).getTime(), y: Math.round(d.achatCumules) }))
      },
      {
        name: "Valeur Totale",
        data: sortedData.map(d => ({ x: new Date(d.date).getTime(), y: Math.round(d.valeurTotale) }))
      }
    ];

    this.chartSeries.set(series);
  }

  private initChartOptions() {
    this.chartOptions = {
      chart: { type: "line", height: 600, animations: { enabled: true } },
      stroke: { width: 2, curve: 'smooth' },
      xaxis: { type: "datetime" },
      yaxis: { labels: { formatter: (val: number) => val.toFixed(2) + " €" } },
      tooltip: { x: { format: "dd MMM yyyy" } }
    };
  }
}