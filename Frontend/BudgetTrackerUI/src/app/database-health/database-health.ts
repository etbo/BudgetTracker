import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { DatabaseHealthService, DatabaseHealthReport } from '../services/database-health.service';

@Component({
  selector: 'app-database-health',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatTableModule,
    MatIconModule,
    MatListModule,
    MatProgressBarModule
  ],
  templateUrl: './database-health.html',
  styleUrl: './database-health.scss'
})
export class DatabaseHealth implements OnInit {
  report: DatabaseHealthReport | null = null;
  loading = true;
  error: string | null = null;

  constructor(
    private healthService: DatabaseHealthService,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this.fetchReport();
  }

  fetchReport(): void {
    this.loading = true;
    this.error = null;
    this.healthService.getHealthReport().subscribe({
      next: (data) => {
        this.report = data;
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        this.error = "Erreur lors de la récupération du rapport de santé.";
        this.loading = false;
        this.cdr.detectChanges();
        console.error(err);
      }
    });
  }
}
