import { Component, Input, SimpleChanges, OnChanges } from '@angular/core';
import { NgApexchartsModule } from 'ng-apexcharts';

@Component({
  selector: 'app-cc-macro-categories-monthly',
  standalone: true, // Assure-toi qu'il est standalone si besoin
  imports: [NgApexchartsModule],
  templateUrl: './cc-macro-categories-monthly.html',
  styleUrl: './cc-macro-categories-monthly.scss',
})
export class CcMacroCategoriesMonthly implements OnChanges {
  @Input() data: any[] = [];

  public chartOptions: any;

  constructor() {
    this.initChartOptions();
  }

  ngOnChanges(changes: SimpleChanges) {
    // On retire la vérification 'this.data' ici pour laisser updateChart gérer
    if (changes['data']) {
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
        stackType: "100%",
        toolbar: { show: false }
      },
      colors: ["#ef4444", "#3b82f6", "#22c55e", "#8b8b8b"],
      xaxis: { 
        categories: [],
        type: 'category' // Force le mode catégorie pour les strings
      },
      yaxis: {
        labels: { formatter: (val: number) => Math.round(val) + "%" }
      },
      dataLabels: {
        enabled: true,
        formatter: (val: number) => val > 0 ? Math.round(val) + "%" : ""
      },
      legend: { position: "top", horizontalAlign: "left" },
      tooltip: {
        y: { 
          formatter: (val: number) => {
            // Affiche 0 si c'est notre valeur de sécurité de 1€
            const displayVal = val <= 1 ? 0 : val;
            return displayVal.toLocaleString() + " €";
          }
        }
      }
    };
  }

  private updateChart() {
    // 1. Suppression du "if (this.data.length === 0) return;" 
    // On veut que les catégories vides s'affichent quand même !
    const categories = this.data.map(d => d.month);

    const series = [
      { name: "Obligatoire", data: this.data.map(d => d.obligatoire) },
      { name: "Loisir", data: this.data.map(d => d.loisir) },
      { name: "Investissement", data: this.data.map(d => d.invest) },
      { name: "Sans catégorie", data: this.data.map(d => d.inconnu) }
    ];

    // 2. CRUCIAL : On crée une nouvelle référence d'objet (Spread Operator)
    // C'est ce qui force le composant <apx-chart> à détecter le changement
    this.chartOptions = {
      ...this.chartOptions,
      series: series,
      xaxis: {
        ...this.chartOptions.xaxis,
        categories: categories
      }
    };
  }
}