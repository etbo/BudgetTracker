import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { CategoryService } from '../services/category.service';
import { Category } from '../models/category-rule.model';

@Component({
  selector: 'app-category-list',
  standalone: true,
  imports: [CommonModule, FormsModule, MatTableModule, MatInputModule, MatButtonModule],
  templateUrl: './category-list.html'
})
export class CategoryListComponent implements OnInit {
  categories = signal<Category[]>([]);
  displayedColumns = ['name', 'type'];

  constructor(private catService: CategoryService) {}

  ngOnInit() {
    this.load();
  }

  load() {
    this.catService.getAll().subscribe(data => this.categories.set(data));
  }

  save(cat: Category) {
    this.catService.update(cat).subscribe(() => {
      console.log('Catégorie et dépendances mises à jour');
    });
  }

  addCategory() {
    this.catService.create({ name: 'Nouvelle catégorie', type: 'Dépense' }).subscribe(newCat => {
      this.categories.update(list => [...list, newCat]);
    });
  }
}