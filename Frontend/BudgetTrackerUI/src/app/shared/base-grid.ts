import { GridOptions } from 'ag-grid-community';

export class BaseGrid {
  /**
   * Génère les options de base. 
   * @param componentInstance Le 'this' du composant enfant
   * @param extraOptions Options spécifiques à la page qui écraseront ou s'ajouteront à la base
   */
  protected createGridOptions(componentInstance: any, extraOptions: GridOptions = {}): GridOptions {
    const defaultOptions: GridOptions = {
      context: { 
        componentParent: componentInstance 
      },
      stopEditingWhenCellsLoseFocus: true,
      undoRedoCellEditing: true,
      suppressNoRowsOverlay: false,
      // On peut ajouter ici des comportements globaux (ex: animation des lignes)
      animateRows: true,
    };

    // On fusionne les options par défaut avec les options spécifiques
    return { ...defaultOptions, ...extraOptions };
  }
}