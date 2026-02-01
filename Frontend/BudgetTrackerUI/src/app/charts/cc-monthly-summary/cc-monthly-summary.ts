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
  @Input() startDate?: string; // Nouvelle entrée
  @Input() endDate?: string;   // Nouvelle entrée

  public chartOptions: any;

  ngOnChanges(changes: SimpleChanges): void {
    this.prepareChartData(this.operations);
  }

  private formatMonth(monthStr: string): string {
    const [year, month] = monthStr.split('-');
    const date = new Date(parseInt(year), parseInt(month) - 1);
    return new Intl.DateTimeFormat('fr-FR', { month: 'short', year: '2-digit' })
      .format(date)
      .replace('.', '');
  }

  prepareChartData(operations: CcOperation[]) {
    if ((!operations || operations.length === 0) && (!this.startDate || !this.endDate)) {
      this.chartOptions = null;
      return;
    }

    const summary: Record<string, { income: number; expenses: number }> = {};

    // --- 1. DÉTERMINATION DE LA PLAGE DE DATES ---
    let startKey: string;
    let endKey: string;

    if (this.startDate && this.endDate) {
      // Mode "LastX" ou "Custom" : on utilise les inputs
      startKey = this.startDate.substring(0, 7);
      endKey = this.endDate.substring(0, 7);
    } else {
      // Mode "ALL" : on cherche les bornes dans les données
      const dates = operations.map(o => o.date).sort();
      startKey = dates[0].substring(0, 7);
      endKey = dates[dates.length - 1].substring(0, 7);
    }

    // --- 2. INITIALISATION DE LA TIMELINE ---
    let [startYear, startMonth] = startKey.split('-').map(Number);
    let [endYear, endMonth] = endKey.split('-').map(Number);

    let cursor = new Date(startYear, startMonth - 1, 1);
    const endLimit = new Date(endYear, endMonth - 1, 1);

    while (cursor <= endLimit) {
      const key = `${cursor.getFullYear()}-${String(cursor.getMonth() + 1).padStart(2, '0')}`;
      summary[key] = { income: 0, expenses: 0 };
      cursor.setMonth(cursor.getMonth() + 1);
    }

    // --- 3. REMPLISSAGE ---
    operations.forEach(op => {
      const month = op.date.substring(0, 7);
      if (summary[month]) {
        const val = op.amount;
        if (val > 0) summary[month].income += val;
        else summary[month].expenses += Math.abs(val);
      }
    });

    const sortedMonths = Object.keys(summary).sort();

    // --- 4. CONFIGURATION APEXCHARTS ---

    // --- 3. CONFIGURATION APEXCHARTS ---
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
      colors: ["#22c55e", "#ef4444", "#3b82f6"],
      labels: sortedMonths.map(m => this.formatMonth(m)), // Crucial : ce sont nos catégories
      xaxis: {
        type: 'category',
        categories: sortedMonths.map(m => this.formatMonth(m)),
        labels: {
          rotate: -45,         // Incline les labels pour gagner de la place
          rotateAlways: false, // Ne les incline que si ça ne passe pas à plat
          hideOverlappingLabels: true, // Cache les labels qui se chevauchent vraiment
          trim: false,
          style: {
            fontSize: '10px'   // On réduit un peu la taille pour les vues denses (24/36 mois)
          }
        },
        tickAmount: sortedMonths.length > 12 ? 12 : undefined // Affiche max 12 ticks sur l'axe pour éviter le fouillis
      },
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
        enabledOnSeries: [2],
        style: { fontSize: '12px', colors: ["#3b82f6"] },
        formatter: (val: number) => {
          return val !== 0 ? Math.round(val).toLocaleString('fr-FR') + " €" : "";
        }
      },
    };
  }
}