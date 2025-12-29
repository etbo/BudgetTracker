import { Routes } from '@angular/router';
import { CcDashboard } from './cc-dashboard/cc-dashboard';
import { CcInput } from './cc-input/cc-input';
import { PeaInputComponent } from './pea-input/pea-input';
import { CcOperations } from './cc-operations/cc-operations';
import { CcCategories } from './cc-categories/cc-categories';
import { PeaDashboard } from './pea-dashboard/pea-dashboard';
import { CcRules } from './cc-rules/cc-rules';

export const routes: Routes = [
  { path: '', component: CcDashboard },

  // Comptes courant : cc
  { path: 'cc-dashboard', component: CcDashboard },
  { path: 'cc-categories', component: CcCategories },
  { path: 'cc-rules', component: CcRules },
  { path: 'cc-operations', component: CcOperations },
  { path: 'cc-input', component: CcInput },

  // pea
  { path: 'pea-input', component: PeaInputComponent },
  { path: 'pea-dashboard', component: PeaDashboard },
  
  // Redirection si l'URL est inconnue
  { path: '**', redirectTo: '' }
];