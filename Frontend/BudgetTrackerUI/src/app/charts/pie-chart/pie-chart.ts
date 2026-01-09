import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { NgApexchartsModule } from "ng-apexcharts";
import { CategoryBalance } from '../../models/category-balance.model';

@Component({
  selector: 'app-pie-chart',
  imports: [NgApexchartsModule],
  templateUrl: './pie-chart.html',
  styleUrl: './pie-chart.scss',
})
export class PieChart implements OnChanges {

  @Input() data: CategoryBalance[] = [];
  @Input() chartTitle: string = '';
  public chartOptions: any;

  ngOnChanges(changes: SimpleChanges) {
    console.log("this.data.length", this.data.length);
    if (changes['data'] && this.data && this.data.length > 0) {
      const validData = this.data.filter(d => d.total > 0);
      if (validData.length > 0) {
        this.setupChart();
      }
    }
  }

  private setupChart() {
    this.chartOptions = {
      series: this.data.map(d => d.total),
      labels: this.data.map(d => d.category),
      chart: {
        type: "donut",
        height: 350,
        sparkline: { enabled: true } // Aide à épurer pour le style semi-donut
      },
      legend: {
        show: true,
        position: 'bottom'
      },
      plotOptions: {
        pie: {
          startAngle: -90, // Commence à gauche
          endAngle: 90,    // S'arrête à droite (arc de 180°)
          offsetY: 10,
          donut: {
            size: '70%',
            labels: {
              show: true,
              total: {
                show: true,
                label: 'Total',
                formatter: () => {
                  const total = this.data.reduce((a, b) => a + b.total, 0);
                  return total.toLocaleString('fr-FR') + " €";
                }
              }
            }
          }
        }
      },
      grid: {
        padding: { bottom: -80 } // Réduit l'espace vide sous le demi-cercle
      },
      title: { text: this.chartTitle, align: 'center' },
      colors: ["#594ae2", "#ff4081", "#3f51b5", "#00bcd4", "#4caf50"]
    };
  }
}
