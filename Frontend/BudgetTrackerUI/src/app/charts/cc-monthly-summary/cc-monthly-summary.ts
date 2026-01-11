import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NgApexchartsModule } from 'ng-apexcharts';
import { CcOperation } from '../../models/operation-cc.model';

@Component({
  selector: 'app-cc-monthly-summary',
  standalone: true,
  imports: [CommonModule, NgApexchartsModule],
  templateUrl: './cc-monthly-summary.html'
})
export class CcMonthlySummary implements OnChanges {
  @Input() operations: CcOperation[] = [];

  public chartOptions: any;

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['operations'] && this.operations) {
      this.prepareChartData(this.operations);
    }
  }

  private formatMonth(monthStr: string): string {
    const [year, month] = monthStr.split('-');
    const date = new Date(parseInt(year), parseInt(month) - 1);
    return new Intl.DateTimeFormat('fr-FR', { month: 'short', year: '2-digit' })
      .format(date)
      .replace('.', '');
  }

  prepareChartData(operations: CcOperation[]) {
    if (!operations || operations.length === 0) return;

    // 1. Groupement par mois
    const summary = operations.reduce((acc, op) => {
      const month = op.date.substring(0, 7); // "YYYY-MM"

      // Initialisation du mois s'il n'existe pas
      if (!acc[month]) {
        acc[month] = { income: 0, expenses: 0 };
      }

      const val = op.montant;
      if (val > 0) {
        acc[month].income += val;
      } else {
        acc[month].expenses += Math.abs(val);
      }
      return acc;
    }, {} as Record<string, { income: number; expenses: number }>);

    // 2. Tri chronologique
    const sortedMonths = Object.keys(summary).sort();

    // 3. Configuration ApexCharts
    this.chartOptions = {
      series: [
        {
          name: "Recettes",
          type: "column",
          data: sortedMonths.map(m => Math.round(summary[m].income))
        },
        {
          name: "Dépenses",
          type: "column",
          data: sortedMonths.map(m => Math.round(summary[m].expenses))
        },
        {
          name: "Bilan Net",
          type: "line",
          data: sortedMonths.map(m => Math.round(summary[m].income - summary[m].expenses))
        }
      ],
      chart: {
        height: 350,
        type: "line",
        toolbar: { show: false },
        zoom: { enabled: false }
      },
      stroke: { width: [0, 0, 3], curve: "smooth" },
      colors: ["#22c55e", "#ef4444", "#3b82f6"], // Vert, Rouge, Bleu
      labels: sortedMonths.map(m => this.formatMonth(m)),
      plotOptions: {
        bar: { borderRadius: 4, columnWidth: "55%" }
      },
      yaxis: {
        labels: { formatter: (val: number) => Math.round(val).toLocaleString('fr-FR') + " €" }
      },
      tooltip: {
        shared: true,
        intersect: false,
        y: { formatter: (val: number) => val.toLocaleString('fr-FR') + " €" }
      },
      legend: { position: 'top' },
      dataLabels: {
        enabled: true,
        enabledOnSeries: [2], // 0: Recettes, 1: Dépenses, 2: Bilan
        //offsetY: -10, // Décale un peu vers le haut pour ne pas coller à la barre
        style: {
          fontSize: '12px',
          colors: ["#3b82f6"]
        },
        formatter: (val: number) => {
          // On n'affiche que si la valeur n'est pas 0 et on formate en €
          return val !== 0 ? Math.round(val).toLocaleString('fr-FR') + " €" : "";
        }
      },
    };
  }
}