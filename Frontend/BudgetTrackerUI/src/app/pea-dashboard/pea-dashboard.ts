import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { PeaGraphService } from '../services/peagraph.service';
import { NgApexchartsModule } from "ng-apexcharts";
import { MatCardModule } from '@angular/material/card';

@Component({
  selector: 'app-pea-dashboard',
  standalone: true,
  imports: [CommonModule, MatSnackBarModule, NgApexchartsModule, MatCardModule],
  templateUrl: './pea-dashboard.html'
})
export class PeaDashboard implements OnInit {
  // --- Services ---
  private peaGraphService = inject(PeaGraphService);

  // --- États ---
  public listeCumulsPea = signal<any[]>([]);
  public chartSeries = signal<any[]>([]);
  public isLoading = signal(true);
  public chartOptions: any;

  constructor() {
    this.initChartOptions();
  }

  ngOnInit() {
    // On appelle updatePricesIfNeeded() : 
    // - Si déjà fait (AppComponent), ça charge les données direct.
    // - Si pas fait, ça met à jour puis charge les données.
    this.peaGraphService.updatePricesIfNeeded().subscribe(() => {
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

    this.chartSeries.set([
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
    ]);
  }

  private initChartOptions() {
    this.chartOptions = {
      chart: { 
        type: "line", 
        height: 600, 
        animations: { enabled: true },
        toolbar: { show: false }
      },
      stroke: { width: 3, curve: 'smooth' },
      xaxis: { type: "datetime" },
      yaxis: { labels: { formatter: (val: number) => val.toLocaleString('fr-FR') + " €" } },
      tooltip: { x: { format: "dd MMM yyyy" }, shared: true }
    };
  }
}