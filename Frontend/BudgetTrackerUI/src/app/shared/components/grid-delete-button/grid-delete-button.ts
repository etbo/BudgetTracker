import { Component } from '@angular/core';
import { ICellRendererAngularComp } from 'ag-grid-angular';
import { ICellRendererParams } from 'ag-grid-community';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-grid-delete-button',
  imports: [CommonModule, MatButtonModule, MatIconModule],
  standalone: true,
  templateUrl: './grid-delete-button.html',
  styleUrl: './grid-delete-button.scss',
})
export class GridDeleteButton implements ICellRendererAngularComp {
  params!: ICellRendererParams;
  showButton: boolean = true;

  agInit(params: ICellRendererParams): void {
    this.params = params;
    // Gestion spécifique pour l'assurance vie (header uniquement)
    if (params.data?.hasOwnProperty('isHeader')) {
      this.showButton = params.data.isHeader;
    }
  }

  refresh(params: ICellRendererParams): boolean {
    return false;
  }

  onDelete(event: MouseEvent) {
    event.stopPropagation(); // Évite de déclencher le clic sur la cellule/ligne
    
    // On récupère le nom de la méthode passé dans les paramètres de la colonne
    const methodName = (this.params as any).methodName || 'delete';
    const parent = this.params.context.componentParent;

    if (parent && typeof parent[methodName] === 'function') {
      parent[methodName](this.params.data);
    } else {
      console.warn(`La méthode ${methodName} n'existe pas sur le composant parent.`);
    }
  }
}