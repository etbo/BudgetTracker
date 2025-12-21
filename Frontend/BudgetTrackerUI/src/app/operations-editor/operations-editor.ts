import { Component, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatSelectModule } from '@angular/material/select';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { OperationsService } from '../services/operations.service';
import { RulesService } from '../services/rules.service';
import { OperationCC } from '../models/operation-cc.model';
import { Category } from '../models/category-rule.model';

@Component({
  selector: 'app-operations-editor',
  standalone: true,
  imports: [CommonModule, FormsModule, MatTableModule, MatSelectModule, MatInputModule, MatButtonModule, MatIconModule, MatProgressSpinnerModule],
  templateUrl: './operations-editor.html'
})
export class OperationsEditorComponent implements OnInit {
  // Signaux d'état
  resultatOperations = signal<OperationCC[]>([]);
  categories = signal<Category[]>([]);
  isLoading = signal(false);
  filterType = signal('C'); // Par défaut : dernier import
  searchString = signal('');
  textResult = signal('');

  // Filtrage local (Recherche)
  filteredOperations = computed(() => {
    const search = this.searchString().toLowerCase();
    const ops = this.resultatOperations();
    if (!search) return ops;
    return ops.filter(o => o.description?.toLowerCase().includes(search));
  });

  displayedColumns = ['date', 'description', 'montant', 'categorie', 'action', 'commentaire', 'banque'];

  constructor(private opService: OperationsService, private rulesService: RulesService) {}

  ngOnInit() {
    this.rulesService.getCategories().subscribe(cats => this.categories.set(cats));
    this.refresh();
  }

  refresh() {
    this.isLoading.set(true);
    this.opService.getOperations(this.filterType()).subscribe({
      next: (data) => {
        this.resultatOperations.set(data);
        this.textResult.set(`${data.length} opérations trouvées`);
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  onCategoryChanged(newCat: string, op: OperationCC) {
    op.categorie = newCat;
    op.isModified = true;
    // On peut soit sauver auto, soit attendre le clic sur disquette
  }

  save(op: OperationCC) {
    this.opService.updateOperation(op).subscribe(() => {
      op.isModified = false;
    });
  }
}