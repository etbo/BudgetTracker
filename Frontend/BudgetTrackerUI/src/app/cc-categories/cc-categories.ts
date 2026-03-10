import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { AgGridModule, ICellRendererAngularComp } from 'ag-grid-angular';
import {
  ModuleRegistry,
  AllCommunityModule,
  GridApi,
  GridReadyEvent,
  CellValueChangedEvent,
  GridOptions
} from 'ag-grid-community';

import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';

import { CategoryService } from '../services/category.service';
import { CcCategory } from '../models/category-rule.model';
import { GridDeleteButton } from '../shared/components/grid-delete-button/grid-delete-button';

ModuleRegistry.registerModules([AllCommunityModule]);

@Component({
  selector: 'app-cc-Categories',
  standalone: true,
  imports: [CommonModule, FormsModule, AgGridModule, MatButtonModule, MatIconModule, MatCardModule],
  templateUrl: './cc-Categories.html',
  styleUrl: './cc-Categories.scss',
})
export class CcCategories implements OnInit {
  private gridApi!: GridApi;
  CcCategories = signal<CcCategory[]>([]);

  public gridOptions: GridOptions = {
    context: { componentParent: this },
    stopEditingWhenCellsLoseFocus: true,
    // sauvegarde aussi si on appuie sur Entrée :
    undoRedoCellEditing: true
  };

  columnDefs: any[] = [
    {
      headerName: 'Nom',
      field: 'name',
      flex: 1,
      editable: true
    },
    {
      headerName: 'Macro catégorie',
      field: 'macroCategory',
      flex: 1,
      editable: true
    },
    {
      headerName: '',
      field: 'delete',
      width: 60,
      cellRenderer: GridDeleteButton,
      cellRendererParams: { methodName: 'deleteCategory' } // Nom de ta fonction
    }
  ];

  defaultColDef = { 
    sortable: true, 
    filter: true, 
    resizable: true,
    filterParams: {
      buttons: ['clear']
    }
  };

  constructor(private catService: CategoryService) { }

  ngOnInit() {
    this.load();
  }

  onGridReady(params: GridReadyEvent) {
    this.gridApi = params.api;
  }

  load() {
    this.catService.getAll().subscribe(data => this.CcCategories.set(data));
  }

  onCellValueChanged(event: CellValueChangedEvent) {
    if (event.oldValue !== event.newValue) {
      this.catService.update(event.data).subscribe(() => {
        this.CcCategories.update(list => [...list]);
      });
    }
  }

  addCategory() {
    const newCatData = { name: 'Nouvelle catégorie', type: 'Dépense' };
    this.catService.create(newCatData).subscribe(newCat => {
      this.CcCategories.update(list => [...list, newCat]);
    });
  }

  deleteCategory(cat: CcCategory) {
    if(confirm(`Voulez-vous vraiment supprimer la catégorie "${cat.name}" ?`)) {
      this.catService.delete(cat.id).subscribe({
        next: () => {
          // Mise à jour immuable du signal pour retirer l'élément
          this.CcCategories.update(list => list.filter(c => c.id !== cat.id));
        },
        error: (err) => console.error("Erreur suppression", err)
      });
    }
  }

  getRowId = (params: any) => params.data.id?.toString();
}