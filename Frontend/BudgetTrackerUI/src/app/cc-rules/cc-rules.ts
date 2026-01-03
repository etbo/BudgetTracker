import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ICellRendererAngularComp, ICellEditorAngularComp } from 'ag-grid-angular';

// AG Grid
import { AgGridModule } from 'ag-grid-angular';
import {
  ModuleRegistry,
  AllCommunityModule,
  ValueFormatterParams,
  ValueSetterParams,
  ColDef
} from 'ag-grid-community';

// Angular Material (Uniquement pour le bouton de suppression et les icônes)
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatSlideToggleModule, MatSlideToggleChange } from '@angular/material/slide-toggle';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';

import { CcCategory, CcCategoryRule } from '../models/category-rule.model';
import { RulesService } from '../services/rules.service';

import { currencyFormatter, amountParser, localDateSetter, customDateFormatter } from '../shared/utils/grid-utils';

ModuleRegistry.registerModules([AllCommunityModule]);

/* =========================================================
    RENDERER : Bouton Supprimer (Hover & Warn)
   ========================================================= */

@Component({
  selector: 'delete-button-renderer',
  standalone: true,
  imports: [MatButtonModule, MatIconModule],
  template: `
    <div class="delete-container">
      <button mat-icon-button (click)="onDelete()" class="delete-btn">
        <mat-icon color="warn">delete_outline</mat-icon>
      </button>
    </div>
  `,
  styles: [`
    .delete-container { display: flex; justify-content: center; align-items: center; height: 100%; }
    .delete-btn { opacity: 0; transition: opacity 0.2s ease-in-out; }
  `]
})
export class DeleteButtonRenderer implements ICellRendererAngularComp {
  params: any;
  agInit(params: any): void { this.params = params; }
  refresh(): boolean { return true; }
  onDelete() {
    this.params.context.componentParent.deleteRule(this.params.data);
  }
}

/* =========================================================
    COMPOSANT PRINCIPAL
   ========================================================= */

@Component({
  selector: 'app-cc-rules',
  standalone: true,
  imports: [
    CommonModule, FormsModule, AgGridModule, MatCardModule,
    MatIconModule, MatButtonModule
  ],
  templateUrl: './cc-rules.html',
  styleUrls: ['./cc-rules.scss']
})
export class CcRules implements OnInit {
  rules = signal<CcCategoryRule[]>([]);
  CcCategories = signal<CcCategory[]>([]);

  gridContext = { componentParent: this };
  defaultColDef = { resizable: true, sortable: true, filter: true };

  columnDefs: ColDef[] = [
    {
      headerName: 'Active',
      field: 'isUsed',
      width: 90,
      cellDataType: 'boolean',
      editable: true,
      cellStyle: { 'display': 'flex', 'justify-content': 'center' }
    },
    { headerName: 'Contient...', field: 'pattern', editable: true, flex: 2 },
    {
      headerName: 'Min',
      field: 'minAmount',
      editable: true,
      width: 110,
      valueFormatter: currencyFormatter,
      valueParser: amountParser,
      cellStyle: { 'text-align': 'right' }
    },
    {
      headerName: 'Max',
      field: 'maxAmount',
      editable: true,
      width: 110,
      valueFormatter: currencyFormatter,
      valueParser: amountParser,
      cellStyle: { 'text-align': 'right' }
    },
    {
      headerName: 'Début',
      field: 'minDate',
      editable: true,
      cellEditor: 'agDateCellEditor',
      cellDataType: false, // Désactive la détection automatique de type
      width: 140,
      valueParser: params => params.newValue,
      valueFormatter: customDateFormatter,
      valueSetter: localDateSetter
    },
    {
      headerName: 'Fin',
      field: 'maxDate',
      editable: true,
      cellEditor: 'agDateCellEditor',
      width: 140,
      valueFormatter: customDateFormatter,
      valueSetter: localDateSetter
    },
    { headerName: 'Comment', field: 'comment', editable: true, flex: 1 },
    {
      headerName: 'Catégorie cible',
      field: 'category',
      editable: true,
      cellEditor: 'agSelectCellEditor',
      cellEditorParams: { values: [] },
      width: 160,
      cellStyle: {
        'background-color': '#e3f2fd', // Bleu très clair pour se démarquer
        'font-weight': 'bold',
        'border-left': '2px solid #1976d2', // Ligne bleue pour marquer la séparation
        'color': '#1565c0'
      }
    },
    {
      headerName: 'X',
      width: 50,
      sortable: false,
      filter: false,
      cellRenderer: DeleteButtonRenderer,
      cellStyle: {
        'background-color': '#e3f2fd'
      }
    }
  ];

  constructor(private rulesService: RulesService) { }

  ngOnInit() {
    this.rulesService.getCcCategories().subscribe(cats => {
      this.CcCategories.set(cats);
      this.updateCategoryColumnParams(cats);
    });
    this.loadRules();
  }

  loadRules() {
    this.rulesService.getRules().subscribe(data => this.rules.set(data));
  }

  updateCategoryColumnParams(cats: CcCategory[]) {
    const categoryCol = this.columnDefs.find(col => col.field === 'category');
    if (categoryCol) {
      categoryCol.cellEditorParams = { values: cats.map(c => c.name) };
    }
  }

  onCellValueChanged(event: any) {
    this.rulesService.update(event.data).subscribe();
  }

  addRule() {
    const newRule: Partial<CcCategoryRule> = { isUsed: true, pattern: '', category: '', minAmount: 0, maxAmount: 0 };
    this.rulesService.create(newRule).subscribe(saved => {
      this.rules.update(list => [...list, saved]);
    });
  }

  deleteRule(rule: CcCategoryRule) {
    if (!rule.id) {
      this.rules.update(list => list.filter(r => r !== rule));
      return;
    }
    if (confirm('Voulez-vous vraiment supprimer cette règle ?')) {
      this.rulesService.delete(rule.id).subscribe(() => {
        this.rules.update(list => list.filter(r => r.id !== rule.id));
      });
    }
  }
}