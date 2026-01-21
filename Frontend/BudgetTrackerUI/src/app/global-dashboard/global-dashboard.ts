import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NgApexchartsModule } from 'ng-apexcharts';
import { MatChipsModule } from '@angular/material/chips';
import { PatrimonyService } from '../services/patrimony.service';
import { MatRadioModule } from '@angular/material/radio';
import { FormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';

type ViewMode = 'accountType' | 'liquidity';

@Component({
  selector: 'app-global-dashboard',
  standalone: true,
  imports: [CommonModule, NgApexchartsModule, MatChipsModule, MatRadioModule, FormsModule, MatCardModule],
  templateUrl: './global-dashboard.html',
  styleUrl: './global-dashboard.scss',
})
export class GlobalDashboard implements OnInit {
  private patrimonyService = inject(PatrimonyService);

  // États
  public rawData = signal<any[]>([]);
  public viewMode = signal<ViewMode>('accountType');

  public lastPoint = computed(() => {
    const data = this.rawData();
    return data.length > 0 ? data[data.length - 1] : null;
  });

  // Calcul du total net actuel
  public currentTotal = computed(() => {
    const p = this.lastPoint();
    if (!p) return 0;
    return p.cash + p.savings + p.lifeInsurance + p.pea;
  });

  // Séries calculées dynamiquement selon le mode choisi
  public chartSeries = computed(() => {
    const data = this.rawData();
    if (data.length === 0) return [];

    if (this.viewMode() === 'accountType') {
      return [
        { name: "Liquidités (CC)", data: data.map(d => d.cash) },
        { name: "Épargne", data: data.map(d => d.savings) },
        { name: "Assurance Vie", data: data.map(d => d.lifeInsurance) },
        { name: "PEA", data: data.map(d => d.pea) }
      ];
    } else {
      // Mode Liquidité : Cash+Épargne vs AV+PEA
      return [
        { name: "Liquide", data: data.map(d => d.cash + d.savings) },
        { name: "Non Liquide", data: data.map(d => d.lifeInsurance + d.pea) }
      ];
    }
  });

  // Couleurs dynamiques selon le nombre de séries
  public chartColors = computed(() => {
    return this.viewMode() === 'accountType'
      ? ['#0077ff', '#FEB019', '#00E396', '#775DD0'] // 4 couleurs
      : ['#00E396', '#FF4560']; // 2 couleurs (Vert / Rouge)
  });

  public chartOptions: any = {
    chart: {
      type: "area",
      stacked: true,
      height: 500,
      zoom: { enabled: false },
      toolbar: { show: false }
    },
    dataLabels: { enabled: false },
    stroke: { curve: "smooth", width: 2 },
    fill: {
      type: "gradient",
      gradient: {
        shadeIntensity: 1,
        opacityFrom: 0.5,
        opacityTo: 0.1,
        stops: [0, 90, 100]
      }
    },
    legend: { position: 'top', horizontalAlign: 'right' },
    yaxis: {
      labels: {
        formatter: (v: number) => (v / 1000).toFixed(0) + ' k€'
      }
    },
    tooltip: {
      shared: true,
      intersect: false,
      custom: ({ series, seriesIndex, dataPointIndex, w }: any): string => {
        const date = w.globals.categoryLabels[dataPointIndex];
        const total = series.reduce((sum: number, s: number[]) => sum + s[dataPointIndex], 0);

        let html = `<div style="padding: 12px; line-height: 1.5; font-size: 12px; background: #fff; border: 1px solid #ccc; border-radius: 4px; box-shadow: 2px 2px 10px rgba(0,0,0,0.1)">`;
        html += `<div style="font-weight: bold; margin-bottom: 8px; border-bottom: 1px solid #eee;">${date}</div>`;

        w.config.series.forEach((s: any, idx: number) => {
          const val = series[idx][dataPointIndex];
          const color = w.config.colors[idx];
          html += `<div style="display: flex; justify-content: space-between; gap: 20px; margin-bottom: 3px;">
                    <span><span style="color:${color}">●</span> ${s.name}</span>
                    <span style="font-weight: 600;">${val.toLocaleString('fr-FR')} €</span>
                   </div>`;
        });

        html += `<div style="margin-top: 8px; padding-top: 8px; border-top: 2px solid #eee; display: flex; justify-content: space-between; font-weight: bold; color: #333;">
                  <span>Patrimoine Net</span>
                  <span>${total.toLocaleString('fr-FR')} €</span>
                 </div>`;
        html += `</div>`;
        return html;
      }
    }
  };

  public chartXAxis = computed(() => {
    return {
      type: "category" as const,
      categories: this.rawData().map(d => d.label),
      labels: {
        rotate: -45,
        style: { fontSize: '11px' }
      },
      tickAmount: 12
    };
  });

  ngOnInit() {
    this.patrimonyService.getGlobalHistory().subscribe(res => {
      this.rawData.set(res);
    });
  }
}