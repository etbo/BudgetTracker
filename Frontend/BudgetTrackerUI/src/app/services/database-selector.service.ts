import { Injectable, signal } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class DatabaseSelectorService {
  // On initialise le signal avec la valeur stockée ou 'Prod'
  private initialDb = localStorage.getItem('selected_db') || 'Prod';
  currentDb = signal<string>(this.initialDb);

  setDatabase(db: string) {
    localStorage.setItem('selected_db', db); // On sauvegarde physiquement
    this.currentDb.set(db); // On met à jour le signal
    
    // Très important : recharger pour que l'interceptor 
    // et tous les composants repartent sur la nouvelle base
    window.location.reload(); 
  }
}