import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { NgApexchartsModule } from "ng-apexcharts";

@Component({
  selector: 'app-treemap-color',
  standalone: true,
  imports: [NgApexchartsModule],
  templateUrl: './treemap-color.html'
})
export class TreemapColor implements OnChanges {
  // Le parent donne directement les séries prêtes
  @Input() expenses: { category: string, total: number }[] = [];
  @Input() revenues: { category: string, total: number }[] = [];

  public chartOptions: any;

  ngOnChanges(changes: SimpleChanges) {
    // Si l'un des deux inputs change, on reconstruit les options
    if (changes['expenses'] || changes['revenues']) {
      this.initChart();
    }
  }

  private initChart() {
    const series = [];
    const colors = [];

    console.log("treemap revenues length:", this.revenues.length);
    if (this.revenues.length > 0) {
      series.push({
        name: 'Recettes',
        data: this.revenues.map(r => ({ x: r.category, y: r.total }))
      });
      colors.push('#22c55e');
    }
    
    console.log("treemap expenses length:", this.expenses.length);
    if (this.expenses.length > 0) {
      series.push({
        name: 'Dépenses',
        data: this.expenses.map(e => ({ x: e.category, y: e.total }))
      });
      colors.push('#ef4444');
    }

    this.chartOptions = {
      series: series,
      colors: colors,
      chart: { type: "treemap", height: 350, toolbar: { show: false } },
      plotOptions: { treemap: { distributed: false, enableShades: false } },
      dataLabels: { enabled: true },
      tooltip: {
        y: { formatter: (val: number) => val.toLocaleString() + " €" }
      }
    };
  }
}