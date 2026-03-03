import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { DatabaseSelectorService } from '../services/database-selector.service'; // Ajuste le chemin vers ton service

export const dbSelectorInterceptor: HttpInterceptorFn = (req, next) => {
  const dbService = inject(DatabaseSelectorService);
  const currentDb = dbService.currentDb(); 

  const clonedRequest = req.clone({
    setHeaders: {
      'X-Database-Selection': currentDb // On simplifie le nom ici
    }
  });

  return next(clonedRequest);
};