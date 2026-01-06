import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { NgApexchartsModule } from "ng-apexcharts";

@Component({
  selector: 'app-cc-evolution-chart',
  imports: [NgApexchartsModule],
  templateUrl: './cc-evolution-chart.html',
  styleUrl: './cc-evolution-chart.scss',
})
export class CcEvolutionChart implements OnChanges {

  @Input() data: any[] = [];
  public chartOptions: any;

  // Cette fonction se déclenche dès que [data] change côté parent
  ngOnChanges(changes: SimpleChanges) {
    if (changes['data'] && this.data.length > 0) {
      this.setupChart();
    }
  }

  private setupChart() {
    this.chartOptions = {
      series: [{
        name: "Solde (€)",
        data: this.data.map(d => ({
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
      dataLabels: {
        enabled: false
      },
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
};
