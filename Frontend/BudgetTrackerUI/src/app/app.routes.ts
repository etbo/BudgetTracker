import { Routes } from '@angular/router';
import { DashboardCcComponent } from './dashboard-cc/dashboard-cc';
import { ImportFileComponent } from './import-file/import-file';
import { PeaInputComponent } from './pea-input/pea-input';
import { RulesListComponent } from './rules-list/rules-list';

export const routes: Routes = [
  { path: '', component: DashboardCcComponent },
  { path: 'cc-evolution', component: DashboardCcComponent },
  { path: 'dashboardcomptecourant', component: DashboardCcComponent },
  
  // Routes temporaires (pointent vers le graph en attendant)
  { path: 'ListCategories', component: DashboardCcComponent },
  { path: 'ListRules', component: RulesListComponent },
  { path: 'operationseditor', component: DashboardCcComponent },
  { path: 'statsdb', component: DashboardCcComponent },
  { path: 'importfile', component: ImportFileComponent },
  { path: 'InputPea', component: PeaInputComponent },
  { path: 'peawallet', component: DashboardCcComponent },
  
  // Redirection si l'URL est inconnue
  { path: '**', redirectTo: '' }
];