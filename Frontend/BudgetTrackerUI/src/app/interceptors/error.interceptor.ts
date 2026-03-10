import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { catchError, throwError } from 'rxjs';

let lastErrorTime = 0;
const ERROR_COOLDOWN_MS = 3000;

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const snackBar = inject(MatSnackBar);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      let errorMessage = 'Une erreur inattendue est survenue.';

      if (error.error instanceof ErrorEvent) {
        errorMessage = `Erreur réseau : ${error.error.message}`;
      } else {
        const backendMessage = error.error?.message || error.error || error.message;
        errorMessage = `[Erreur ${error.status}] Impossible de joindre le serveur.`;
        console.error(`Backend returned code ${error.status}, body was: `, backendMessage);
      }

      const now = Date.now();
      if (now - lastErrorTime > ERROR_COOLDOWN_MS) {
        lastErrorTime = now;
        
        snackBar.open(errorMessage, 'Fermer', {
          duration: 5000,
          panelClass: ['error-snackbar'],
          horizontalPosition: 'center',
          verticalPosition: 'bottom',
        });
      }

      // On relance l'erreur pour que les composants puissent (s'ils le veulent) la traiter spécifiquement
      return throwError(() => error);
    })
  );
};
