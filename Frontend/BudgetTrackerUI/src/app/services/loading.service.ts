import { Injectable, signal } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class LoadingService {
  private activeRequests = 0;
  private timer: any = null;
  
  public isLoading = signal<boolean>(false);

  show() {
    this.activeRequests++;
    
    // Si on a déjà un timer en cours, on ne fait rien
    if (this.timer) return;

    // On attend 250ms avant de montrer le loader. 
    // Si la requête finit avant, on ne verra rien. (Évite le clignotement)
    this.timer = setTimeout(() => {
      if (this.activeRequests > 0) {
        this.isLoading.set(true);
      }
    }, 250);
  }

  hide() {
    this.activeRequests--;
    if (this.activeRequests <= 0) {
      this.activeRequests = 0;
      
      // On nettoie tout
      if (this.timer) {
        clearTimeout(this.timer);
        this.timer = null;
      }
      this.isLoading.set(false);
    }
  }
}
