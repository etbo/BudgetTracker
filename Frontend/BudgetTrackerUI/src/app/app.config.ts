import { ApplicationConfig, LOCALE_ID } from '@angular/core';
import { provideNativeDateAdapter } from '@angular/material/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { provideHttpClient, withInterceptors } from '@angular/common/http';

import { registerLocaleData } from '@angular/common';
import localeFr from '@angular/common/locales/fr';
import { errorInterceptor } from './interceptors/error.interceptor';
import { loadingInterceptor } from './interceptors/loading.interceptor';
import { dbSelectorInterceptor } from './interceptors/db-selector.interceptor';
registerLocaleData(localeFr);

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes),
    provideHttpClient(
      withInterceptors([dbSelectorInterceptor, errorInterceptor, loadingInterceptor])
    ),
    provideNativeDateAdapter(),
    { provide: LOCALE_ID, useValue: 'fr-FR' }
  ]
};
