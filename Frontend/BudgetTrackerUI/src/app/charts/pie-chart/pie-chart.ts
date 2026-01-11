import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { NgApexchartsModule } from "ng-apexcharts";

@Component({
  selector: 'app-pie-chart',
  standalone: true, // Ajoute standalone si nécessaire
  imports: [NgApexchartsModule],
  templateUrl: './pie-chart.html',
  styleUrl: './pie-chart.scss',
})
export class PieChart implements OnChanges {
  // On accepte une structure simple et déjà filtrée par le parent
  @Input() data: { label: string, value: number }[] = [];
  @Input() chartTitle: string = '';

  public chartOptions: any;

  ngOnChanges(changes: SimpleChanges) {
    if (changes['data'] && this.data) {
      this.setupChart();
    }
  }

  private setupChart() {
    // Si pas de données, on vide les options pour ne pas afficher un graph erroné
    if (this.data.length === 0) {
      this.chartOptions = null;
      return;
    }

    this.chartOptions = {
      series: this.data.map(d => d.value),
      labels: this.data.map(d => d.label),
      chart: {
        type: "donut",
        height: 350,
        sparkline: { enabled: true }
      },
      legend: { show: true, position: 'bottom' },
      plotOptions: {
        pie: {
          startAngle: -90,
          endAngle: 90,
          offsetY: 10,
          donut: {
            size: '70%',
            labels: {
              show: true,
              total: {
                show: true,
                label: 'Total',
                formatter: () => {
                  const total = this.data.reduce((a, b) => a + b.value, 0);
                  return Math.round(total).toLocaleString('fr-FR') + " €";
                }
              }
            }
          }
        }
      },
      grid: { padding: { bottom: -80 } },
      title: { text: this.chartTitle, align: 'center' },
      // Couleurs adaptées (Apex gérera la répétition si + de 5 catégories)
      colors: ["#594ae2", "#ff4081", "#3f51b5", "#00bcd4", "#4caf50"]
    };
  }
}