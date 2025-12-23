import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { RulesService } from '../services/rules.service';
import { CategoryRule, Category } from '../models/category-rule.model';

@Component({
  selector: 'app-rules-list',
  standalone: true,
  imports: [
    CommonModule, FormsModule, MatTableModule, MatInputModule, 
    MatSelectModule, MatSlideToggleModule, MatDatepickerModule, MatIconModule, MatCardModule
  ],
  templateUrl: './rules-list.html'
})
export class RulesListComponent implements OnInit {
  rules = signal<CategoryRule[]>([]);
  categories = signal<Category[]>([]);
  
  displayedColumns = ['active', 'pattern', 'minAmount', 'maxAmount', 'minDate', 'maxDate', 'comment', 'arrow', 'category'];

  constructor(private rulesService: RulesService) {}

  ngOnInit() {
    this.rulesService.getCategories().subscribe(cats => this.categories.set(cats));
    this.loadRules();
  }

  loadRules() {
    this.rulesService.getRules().subscribe(data => this.rules.set(data));
  }

  save(rule: CategoryRule) {
    this.rulesService.update(rule).subscribe();
  }

  addRule() {
    const newRule: Partial<CategoryRule> = { isUsed: true, pattern: '', category: '' };
    this.rulesService.create(newRule).subscribe(saved => {
      this.rules.update(list => [...list, saved]);
    });
  }
}