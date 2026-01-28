import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NgApexchartsModule } from 'ng-apexcharts';
import { MatChipsModule } from '@angular/material/chips';
import { MatRadioModule } from '@angular/material/radio';
import { FormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { PatrimonyService } from '../services/patrimony.service';
import { ThemeService } from '../services/theme.service';

type ViewMode = 'accountType' | 'liquidity';

@Component({
  selector: 'app-global-dashboard',
  standalone: true,
  imports: [CommonModule, NgApexchartsModule, MatChipsModule, MatRadioModule, FormsModule, MatCardModule],
  templateUrl: './global-dashboard.html',
  styleUrl: './global-dashboard.scss',
})
export class GlobalDashboard implements OnInit {
  private themeService = inject(ThemeService);
  private patrimonyService = inject(PatrimonyService);

  // --- ÉTATS ---
  public rawData = signal<any[]>([]);
  public viewMode = signal<ViewMode>('accountType');

  // --- CALCULS DU PATRIMOINE ---
  public lastPoint = computed(() => {
    const data = this.rawData();
    return data.length > 0 ? data[data.length - 1] : null;
  });

  public currentTotal = computed(() => {
    const p = this.lastPoint();
    return p ? (p.cash + p.savings + p.lifeInsurance + p.pea) : 0;
  });

  // --- SÉRIES DU GRAPHIQUE ---
  public chartSeries = computed(() => {
    const data = this.rawData();
    if (data.length === 0) return [];

    if (this.viewMode() === 'accountType') {
      return [
        { name: "Liquidités (CC)", data: data.map(d => d.cash) },
        { name: "Épargne", data: data.map(d => d.savings) },
        { name: "PEA", data: data.map(d => d.pea) },
        { name: "Assurance Vie", data: data.map(d => d.lifeInsurance) }
      ];
    } else {
      return [
        { name: "Liquide", data: data.map(d => d.cash + d.savings) },
        { name: "Non Liquide", data: data.map(d => d.lifeInsurance + d.pea) }
      ];
    }
  });

  // --- COULEURS SYNCHRONISÉES (THEME SERVICE) ---
  public chartColors = computed(() => {
    if (this.viewMode() === 'accountType') {
      return [
        this.themeService.getAccountColor('cc'),
        this.themeService.getAccountColor('savings'),
        this.themeService.getAccountColor('pea'),
        this.themeService.getAccountColor('life-insurrance'),
        this.themeService.getVariableColor('--color-autre')
      ];
    }
    return [
      this.themeService.getVariableColor('--color-success'),
      this.themeService.getVariableColor('--color-danger')
    ];
  });

  // --- AXE X ---
  public chartXAxis = computed(() => {
    return {
      type: "category" as const,
      categories: this.rawData().map(d => d.label),
      labels: { rotate: -45, style: { fontSize: '11px' } },
      tickAmount: 12
    };
  });

  // --- CONFIGURATION APEXCHARTS ---
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
      type: "solid",
      gradient: {
        shadeIntensity: 1,
        opacityFrom: 0.5,
        opacityTo: 0.1,
        stops: [0, 90, 100]
      }
    },
    legend: { position: 'top', horizontalAlign: 'right' },
    yaxis: {
      labels: { formatter: (v: number) => (v / 1000).toFixed(0) + ' k€' }
    },
    tooltip: {
      shared: true,
      intersect: false,
      custom: ({ series, seriesIndex, dataPointIndex, w }: any): string => {
        const date = w.globals.categoryLabels[dataPointIndex];
        const total = series.reduce((sum: number, s: number[]) => sum + s[dataPointIndex], 0);

        let html = `<div style="padding: 12px; font-size: 12px; background: #fff; border: 1px solid #ccc; border-radius: 4px;">`;
        html += `<div style="font-weight: bold; margin-bottom: 8px;">${date}</div>`;

        w.config.series.forEach((s: any, idx: number) => {
          const val = series[idx][dataPointIndex];
          const color = w.config.colors[idx];
          const formattedVal = Math.round(val).toLocaleString('fr-FR');
          html += `<div style="display: flex; justify-content: space-between; gap: 20px;">
                    <span><span style="color:${color}">●</span> ${s.name}</span>
                    <span style="font-weight: 600;">${formattedVal} €</span>
                   </div>`;
        });

        const formattedVal = Math.round(total).toLocaleString('fr-FR');

        html += `<div style="margin-top: 8px; border-top: 1px solid #eee; font-weight: bold; padding-top: 5px; text-align: center;">
            Total : ${formattedVal} €
         </div></div>`;
        return html;
      }
    }
  };

  ngOnInit() {
    // Restauration du mode de vue uniquement
    const savedView = localStorage.getItem('dashboard_view_mode') as ViewMode;
    if (savedView) this.viewMode.set(savedView);

    this.patrimonyService.getGlobalHistory().subscribe(res => {
      this.rawData.set(res);
    });
  }

  updateViewMode(mode: ViewMode) {
    this.viewMode.set(mode);
    localStorage.setItem('dashboard_view_mode', mode);
  }

  // Utilitaire pour les bordures et textes des cartes dans le HTML
  getAccountColorVar(name: string) {
    return this.themeService.getAccountColorVar(name);
  }
}