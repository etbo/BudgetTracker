import { Component, computed, OnDestroy, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';

import { OperationsService } from '../services/operations.service';
import { filtersService, FilterState } from '../services/filters.service';
import { DateFilter } from '../date-filter/date-filter';
import { CcOperationsList } from '../cc-operations-list/cc-operations-list';
import { CcOperation } from '../models/operation-cc.model';

import { distinctUntilChanged, Subscription } from 'rxjs';

@Component({
  selector: 'app-cc-operations',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatCardModule,
    MatInputModule,
    MatIconModule,
    MatChipsModule,
    DateFilter,
    CcOperationsList
  ],
  templateUrl: './cc-operations.html',
  styleUrls: ['./cc-operations.scss']
})
export class CcOperations implements OnInit, OnDestroy {

  private filterSub?: Subscription;

  operations = signal<CcOperation[]>([]);
  public zoomFilters = signal<FilterState | undefined>(undefined);
  parentSearchString: string = '';

  // État visuel des puces (chips)
  filterMissingCat = false;
  filterSuggestedCat = false;
  filterOnlyCheques = false;

  // Stats
  totalOperations = computed(() => this.operations().length);
  
  missingCategoriesCount = computed(() => 
    this.operations().filter(op => !op.categorie || op.isModified).length
  );

  suggestedCategoriesCount = computed(() => 
    this.operations().filter(op => op.isModified).length
  );

  constructor(private operationsService: OperationsService) { }

  ngOnInit() {
    // 1. Mise à jour de l'UI uniquement lors des changements de filtres globaux
    // On ne rappelle PAS loadOperations ici pour éviter la boucle infinie avec le service
    this.filterSub = filtersService.filters$.pipe(
      distinctUntilChanged((prev, curr) => JSON.stringify(prev) === JSON.stringify(curr))
    ).subscribe(() => {
      this.refreshLocalFilters();
    });

    // 2. Premier chargement manuel au lancement de la page
    this.loadOperations();
  }

  loadOperations() {
    this.operationsService.getOperations().subscribe(data => {
      this.operations.set(data);
      // Synchronisation du signal local pour informer la liste AG Grid
      this.zoomFilters.set({ ...filtersService.getFilters() });
    });
  }

  refreshLocalFilters() {
    const f = filtersService.getFilters();
    this.filterMissingCat = !!f.missingCat;
    this.filterSuggestedCat = !!f.suggestedCat;
    this.filterOnlyCheques = !!f.onlyCheques;
  }

  // Déclenché par le composant DateFilter
  onFilterChanged(event: any) {
    filtersService.updateFilters({
      start: event.start,
      end: event.end,
      view: event.view
    });
    this.loadOperations(); 
  }

  // Déclenché par le composant CcOperationsList (ex: après une sauvegarde ou suppression)
  onListRefresh() {
    this.loadOperations();
  }

  // Gestion des clics sur les puces de filtrage
  toggleExtraFilter(type: 'missing' | 'suggested' | 'cheque') {
    const f = filtersService.getFilters();
    
    filtersService.updateFilters({
      ...f,
      missingCat: type === 'missing' ? !f.missingCat : f.missingCat,
      suggestedCat: type === 'suggested' ? !f.suggestedCat : f.suggestedCat,
      onlyCheques: type === 'cheque' ? !f.onlyCheques : f.onlyCheques
    });

    // On déclenche manuellement le chargement suite à l'action utilisateur
    this.loadOperations();
  }

  ngOnDestroy() {
    if (this.filterSub) {
      this.filterSub.unsubscribe();
    }
  }
}