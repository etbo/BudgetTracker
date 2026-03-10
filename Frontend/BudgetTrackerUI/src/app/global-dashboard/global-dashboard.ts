import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NgApexchartsModule } from 'ng-apexcharts';
import { MatChipsModule } from '@angular/material/chips';
import { MatRadioModule } from '@angular/material/radio';
import { FormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { PatrimonyService, PatrimonySummaryDto, GlobalHistoryPoint } from '../services/patrimony.service';
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
  public rawData = signal<GlobalHistoryPoint[]>([]);
  public currentSummary = signal<PatrimonySummaryDto | null>(null);
  public viewMode = signal<ViewMode>('accountType');

  // --- SÉRIES DU GRAPHIQUE ---
  public chartSeries = computed(() => {
    const data = this.rawData();
    if (data.length === 0) return [];

    let series: any[] = [];
    if (this.viewMode() === 'accountType') {
      series = [
        { name: "Liquidités (CC)", data: data.map(d => d.cash) },
        { name: "Épargne", data: data.map(d => d.savings) },
        { name: "PEA", data: data.map(d => d.pea) },
        { name: "Assurance Vie", data: data.map(d => d.lifeInsurance) }
      ];
    } else {
      series = [
        { name: "Liquide", data: data.map(d => d.cash + d.savings) },
        { name: "Non Liquide", data: data.map(d => d.lifeInsurance + d.pea) }
      ];
    }

    // On ne garde que les séries qui ont au moins une valeur > 0 sur la période
    // Cela évite d'empiler des "lignes à zéro" qui créent des artéfacts visuels (traits sombres)
    return series.filter(s => s.data.some((v: number) => v > 0));
  });

  // --- COULEURS SYNCHRONISÉES (THEME SERVICE) ---
  public chartColors = computed(() => {
    const data = this.rawData();
    if (data.length === 0) return [];

    let allColors: string[] = [];
    let seriesActive: boolean[] = [];

    if (this.viewMode() === 'accountType') {
      allColors = [
        this.themeService.getAccountColor('cc'),
        this.themeService.getAccountColor('savings'),
        this.themeService.getAccountColor('pea'),
        this.themeService.getAccountColor('life-insurrance')
      ];
      seriesActive = [
        data.some(d => d.cash > 0),
        data.some(d => d.savings > 0),
        data.some(d => d.pea > 0),
        data.some(d => d.lifeInsurance > 0)
      ];
    } else {
      allColors = [
        this.themeService.getVariableColor('--color-success'),
        this.themeService.getVariableColor('--color-danger')
      ];
      seriesActive = [
        data.some(d => (d.cash + d.savings) > 0),
        data.some(d => (d.lifeInsurance + d.pea) > 0)
      ];
    }

    // On ne garde que les couleurs des séries qui ont des données
    return allColors.filter((_, idx) => seriesActive[idx]);
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
      height: '100%',
      zoom: { enabled: false },
      toolbar: { show: false },
      animations: {
        enabled: true,
        easing: 'easeinout',
        speed: 800,
        animateGradually: {
            enabled: false // IMPRESSIONNANT : On désactive la montée couche par couche qui créait les artéfacts
        },
        dynamicAnimation: {
            enabled: true,
            speed: 350
        }
      }
    },
    dataLabels: { enabled: false },
    stroke: { curve: "smooth", width: 2 },
    fill: {
      type: "solid",
      opacity: 0.8 // Couleurs solides et bien visibles
    },
    legend: { 
      show: true,
      position: 'top', 
      horizontalAlign: 'right',
      showForSingleSeries: true, // IMPORTANT : Sinon la légende disparait s'il n'y a qu'une série (ex: que du CC)
      offsetY: -10 // Remonte un peu pour laisser de la place
    },
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

    this.patrimonyService.getCurrentSummary().subscribe(res => {
      this.currentSummary.set(res);
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

  getGlobalColorVar() {
    return this.themeService.getGlobalColorVar();
  }
}