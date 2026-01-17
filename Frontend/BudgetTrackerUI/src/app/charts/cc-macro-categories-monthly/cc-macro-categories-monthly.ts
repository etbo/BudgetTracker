import { Component, Input, SimpleChanges } from '@angular/core';
import { NgApexchartsModule } from 'ng-apexcharts';

@Component({
  selector: 'app-cc-macro-categories-monthly',
  imports: [NgApexchartsModule],
  templateUrl: './cc-macro-categories-monthly.html',
  styleUrl: './cc-macro-categories-monthly.scss',
})
export class CcMacroCategoriesMonthly {
  @Input() data: any[] = [];

  public chartOptions: any;

  constructor() {
    this.initChartOptions();
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['data'] && this.data) {
      this.updateChart();
    }
  }

  private initChartOptions() {
    this.chartOptions = {
      series: [],
      chart: {
        type: "bar",
        height: 350,
        stacked: true,
        stackType: "100%", // Force le total à 100%
        toolbar: { show: false }
      },
      colors: ["#ef4444", "#3b82f6", "#22c55e", "#8b8b8b"], // Rouge (Obligatoire), Bleu (Loisir), Vert (Invest)
      xaxis: { categories: [] },
      yaxis: {
        labels: { formatter: (val: number) => val + "%" }
      },
      dataLabels: {
        enabled: true,
        formatter: (val: number) => Math.round(val) + "%"
      },
      legend: { position: "top", horizontalAlign: "left" },
      tooltip: {
        y: { formatter: (val: number) => val.toLocaleString() + " €" }
      }
    };
  }

  private updateChart() {
    if (this.data.length === 0) return;

    // Extraction des labels (mois)
    const categories = this.data.map(d => d.month);

    // Construction des séries
    this.chartOptions.series = [
      { name: "Obligatoire", data: this.data.map(d => d.obligatoire) },
      { name: "Loisir", data: this.data.map(d => d.loisir) },
      { name: "Investissement", data: this.data.map(d => d.invest) },
      { name: "Sans catégorie", data: this.data.map(d => d.inconnu) }
    ];

    this.chartOptions.xaxis = { categories: categories };
  }
}
