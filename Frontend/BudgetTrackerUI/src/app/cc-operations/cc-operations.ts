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

  // Stats (Inchangées)
  totalOperations = computed(() => this.operations().length);
  missingCategoriesCount = computed(() => this.operations().filter(op => !op.categorie || op.isSuggested).length);
  suggestedCategoriesCount = computed(() => this.operations().filter(op => op.isSuggested).length);

  constructor(private operationsService: OperationsService) { }

  ngOnInit() {
    // Écoute les changements de filtres (URL, Puces, Dates)
    this.filterSub = filtersService.filters$.pipe(
      distinctUntilChanged((prev, curr) => JSON.stringify(prev) === JSON.stringify(curr))
    ).subscribe(() => {
      this.refreshLocalFilters(); // Met à jour les variables filterXXXX pour l'UI
      this.loadOperations();      // RELANCE l'appel API (indispensable)
    });
  }

  loadOperations() {
    // Note : getOperations() utilise filtersService.getFilters() en interne
    this.operationsService.getOperations().subscribe(data => {
      this.operations.set(data);
      this.zoomFilters.set({ ...filtersService.getFilters() });
    });
  }

  refreshLocalFilters() {
    const f = filtersService.getFilters();
    this.filterMissingCat = !!f.missingCat;
    this.filterSuggestedCat = !!f.suggestedCat;
    this.filterOnlyCheques = !!f.onlyCheques;
  }

  onFilterChanged(event: any) {
    // Cette méthode met à jour le service global, 
    // ce qui déclenche automatiquement le .subscribe() du ngOnInit
    filtersService.updateFilters({
      ...filtersService.getFilters(),
      start: event.start,
      end: event.end,
      view: event.view
    });
  }

  onListRefresh() {
    this.loadOperations();
  }

  toggleExtraFilter(type: 'missing' | 'suggested' | 'cheque') {
    const f = filtersService.getFilters();
    
    // On met à jour le service global. 
    // L'abonnement dans ngOnInit s'occupera du loadOperations()
    filtersService.updateFilters({
      ...f,
      missingCat: type === 'missing' ? !f.missingCat : f.missingCat,
      suggestedCat: type === 'suggested' ? !f.suggestedCat : f.suggestedCat,
      onlyCheques: type === 'cheque' ? !f.onlyCheques : f.onlyCheques
    });
  }

  ngOnDestroy() {
    this.filterSub?.unsubscribe();
  }
}