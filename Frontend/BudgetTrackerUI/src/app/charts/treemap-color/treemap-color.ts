import { Component, Input, OnChanges, ViewChild, SimpleChanges } from '@angular/core';
import { NgApexchartsModule, ChartComponent } from "ng-apexcharts";
import { CategoryBalance } from '../../models/category-balance.model';

@Component({
  selector: 'app-treemap-color',
  standalone: true,
  imports: [NgApexchartsModule],
  templateUrl: './treemap-color.html',
  styleUrl: './treemap-color.scss',
})
export class TreemapColor implements OnChanges {
  // On utilise désormais "data" qui contient déjà les totaux par catégorie
  @Input() data: CategoryBalance[] = [];
  @ViewChild("chart") chart!: ChartComponent;

  public chartOptions: any = {
    series: [],
    chart: {
      type: "treemap",
      height: 350,
      toolbar: { show: false }
    },
    // Index 0: Recettes (Vert), Index 1: Dépenses (Rouge)
    colors: ['#22c55e', '#ef4444'],
    title: { text: "Flux financiers par catégorie" },
    dataLabels: {
      enabled: true,
      style: { fontSize: '12px', fontWeight: 'bold' }
    },
    plotOptions: {
      treemap: {
        enableShades: false,
        distributed: false // Important pour que chaque série garde sa couleur unie
      }
    },
    tooltip: {
      y: {
        formatter: (val: number, { seriesIndex, dataPointIndex, w }: any) => {
          // On récupère la valeur réelle (négative ou positive) stockée dans l'objet
          const originalValue = w.config.series[seriesIndex].data[dataPointIndex].actualValue;
          return originalValue.toLocaleString() + " €";
        }
      }
    }
  };

  ngOnChanges(changes: SimpleChanges) {
    // On surveille le changement de l'input "data"
    if (changes['data'] && this.data) {
      this.updateChart();
    }
  }

  private updateChart() {
    if (!this.data || this.data.length === 0) return;

    const revenuesData = this.data
      .filter(c => c.total > 0)
      .map(c => ({
        x: c.category,
        y: c.total,
        actualValue: c.total
      }));

    const expensesData = this.data
      .filter(c => c.total < 0)
      .map(c => ({
        x: c.category,
        y: Math.abs(c.total),
        actualValue: c.total
      }));

    // On construit les séries dynamiquement
    const finalSeries = [];

    if (revenuesData.length > 0) {
      finalSeries.push({ name: 'Recettes', data: revenuesData });
    }

    if (expensesData.length > 0) {
      finalSeries.push({ name: 'Dépenses', data: expensesData });
    }

    // Si on n'a rien à afficher, on arrête là
    if (finalSeries.length === 0) return;

    this.chartOptions.series = finalSeries;

    // Mise à jour manuelle des couleurs pour correspondre au nombre de séries présentes
    this.chartOptions.colors = [];
    if (revenuesData.length > 0) this.chartOptions.colors.push('#22c55e');
    if (expensesData.length > 0) this.chartOptions.colors.push('#ef4444');

    if (this.chart) {
      this.chart.updateOptions(this.chartOptions);
    }
  }
}