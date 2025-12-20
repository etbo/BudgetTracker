import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatInputModule } from '@angular/material/input';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatButtonModule } from '@angular/material/button';
import { PeaService } from '../services/pea.service';
import { OperationPea } from '../models/operation-pea.model';

@Component({
  selector: 'app-pea-input',
  standalone: true,
  imports: [CommonModule, FormsModule, MatTableModule, MatInputModule, MatDatepickerModule, MatButtonModule],
  templateUrl: './pea-input.html'
})
export class PeaInputComponent implements OnInit {
  operations = signal<OperationPea[]>([]);
  displayedColumns = ['date', 'titulaire', 'code', 'quantite', 'brut', 'net', 'frais'];

  constructor(private peaService: PeaService) { }

  ngOnInit() {
    this.loadOperations();
  }

  loadOperations() {
    this.peaService.getAll().subscribe(data => {
      console.log("Données chargées :", data);
      this.operations.set(data);
    });
  }

  addOperation() {
    this.peaService.create({}).subscribe(newOp => {
      this.operations.update(list => [...list, newOp]);
    });
  }

  save(op: OperationPea) {
    this.peaService.update(op).subscribe({
      next: () => console.log('Sauvegardé'),
      error: () => alert('Erreur lors de la sauvegarde')
    });
  }

  calculateFrais(op: OperationPea): string {
    if (op.montantNet > 0 && op.montantNet > op.montantBrutUnitaire) {
      const pourcentage = ((op.montantNet / op.montantBrutUnitaire) - 1) * 100;
      return pourcentage.toFixed(2) + ' %';
    }
    return '-';
  }
}