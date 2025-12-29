import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { AgGridModule, ICellRendererAngularComp } from 'ag-grid-angular';
import { 
  ModuleRegistry, 
  AllCommunityModule, 
  GridApi, 
  GridReadyEvent,
  CellValueChangedEvent
} from 'ag-grid-community';

import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';

import { CategoryService } from '../services/category.service';
import { Category } from '../models/category-rule.model';

ModuleRegistry.registerModules([AllCommunityModule]);

/* --- Renderer pour le bouton supprimer --- */
@Component({
  standalone: true,
  imports: [CommonModule, MatButtonModule, MatIconModule, MatIconModule],
  template: `
    <div class="flex justify-center items-center h-full">
      <button mat-icon-button color="warn" (click)="onDelete()">
        <mat-icon>delete</mat-icon>
      </button>
    </div>
  `
})
export class DeleteButtonRenderer implements ICellRendererAngularComp {
  params: any;
  agInit(params: any): void { this.params = params; }
  refresh(): boolean { return false; }

  onDelete() {
    // On appelle la méthode de suppression du parent via le contexte
    this.params.context.componentParent.deleteCategory(this.params.data);
  }
}

@Component({
  selector: 'app-cc-categories',
  standalone: true,
  imports: [CommonModule, FormsModule, AgGridModule, MatButtonModule, MatIconModule, MatCardModule],
  templateUrl: './cc-categories.html',
  styleUrl: './cc-categories.scss',
})
export class CcCategories implements OnInit {
  private gridApi!: GridApi;
  categories = signal<Category[]>([]);
  gridContext = { componentParent: this };

  columnDefs: any[] = [
    { 
      headerName: 'Nom', 
      field: 'name', 
      flex: 1, 
      editable: true
    },
    { 
      headerName: 'Type', 
      field: 'type', 
      flex: 1, 
      editable: true 
    },
    {
      headerName: '',
      width: 70,
      cellRenderer: DeleteButtonRenderer,
      sortable: false,
      filter: false
    }
  ];

  defaultColDef = { sortable: true, filter: true, resizable: true };

  constructor(private catService: CategoryService) {}

  ngOnInit() {
    this.load();
  }

  onGridReady(params: GridReadyEvent) {
    this.gridApi = params.api;
  }

  load() {
    this.catService.getAll().subscribe(data => this.categories.set(data));
  }

  onCellValueChanged(event: CellValueChangedEvent) {
    if (event.oldValue !== event.newValue) {
      this.catService.update(event.data).subscribe(() => {
        this.categories.update(list => [...list]);
      });
    }
  }

  addCategory() {
    const newCatData = { name: 'Nouvelle catégorie', type: 'Dépense' };
    this.catService.create(newCatData).subscribe(newCat => {
      this.categories.update(list => [...list, newCat]);
    });
  }

  deleteCategory(cat: Category) {
    if (confirm(`Voulez-vous vraiment supprimer la catégorie "${cat.name}" ?`)) {
      this.catService.delete(cat.id).subscribe({
        next: () => {
          // Mise à jour immuable du signal pour retirer l'élément
          this.categories.update(list => list.filter(c => c.id !== cat.id));
        },
        error: (err) => console.error("Erreur suppression", err)
      });
    }
  }

  getRowId = (params: any) => params.data.id?.toString();
}