import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';

import { filtersService, FilterState } from '../services/filters.service';
import { DateFilter } from '../date-filter/date-filter';
import { CcOperationsList } from '../cc-operations-list/cc-operations-list'; // Import du nouveau composant

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
    CcOperationsList // On importe le composant réutilisable
  ],
  templateUrl: './cc-operations.html',
  styleUrls: ['./cc-operations.scss']
})
export class CcOperations implements OnInit {
  searchString = '';
  
  // Ces propriétés servent uniquement à l'affichage des puces (chips)
  filterMissingCat = false;
  filterSuggestedCat = false;
  filterOnlyCheques = false;

  ngOnInit() {
    this.refreshLocalFilters();
    
    // On s'abonne aux changements pour mettre à jour l'état des puces
    window.addEventListener('filterChanged', () => {
      this.refreshLocalFilters();
    });
  }

  refreshLocalFilters() {
    const f = filtersService.getFilters();
    this.filterMissingCat = !!f.missingCat;
    this.filterSuggestedCat = !!f.suggestedCat;
    this.filterOnlyCheques = !!f.onlyCheques;
  }

  onFilterChanged(event: any) {
    filtersService.updateFilters({
      start: event.start,
      end: event.end,
      view: event.view
    });
  }

  toggleExtraFilter(type: 'missing' | 'suggested' | 'cheque') {
    const f = filtersService.getFilters();
    filtersService.updateFilters({
      missingCat: type === 'missing' ? !f.missingCat : f.missingCat,
      suggestedCat: type === 'suggested' ? !f.suggestedCat : f.suggestedCat,
      onlyCheques: type === 'cheque' ? !f.onlyCheques : f.onlyCheques
    });
  }

  onImport() {
    // Ta logique d'import ici
    console.log("Import demandé");
  }

  // Note : La recherche "QuickFilter" devra être gérée soit par une @Input 
  // dans CcOperationsList, soit via le service si tu veux qu'elle soit globale.
}