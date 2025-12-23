import { Routes } from '@angular/router';
import { DashboardCcComponent } from './dashboard-cc/dashboard-cc';
import { ImportFileComponent } from './import-file/import-file';
import { PeaInputComponent } from './pea-input/pea-input';
import { OperationsEditorComponent } from './operations-editor/operations-editor';
import { CategoryListComponent } from './category-list/category-list';
import { PeaWalletComponent } from './pea-wallet/pea-wallet';
import { CategorizationGridComponent } from './categorization-grid/categorization-grid';

export const routes: Routes = [
  { path: '', component: DashboardCcComponent },
  { path: 'cc-evolution', component: DashboardCcComponent },
  { path: 'dashboardcomptecourant', component: DashboardCcComponent },
  
  // Routes temporaires (pointent vers le graph en attendant)
  { path: 'ListCategories', component: CategoryListComponent },
  { path: 'ListRules', component: CategorizationGridComponent },
  { path: 'operationseditor', component: OperationsEditorComponent },
  { path: 'statsdb', component: CategorizationGridComponent },
  { path: 'importfile', component: ImportFileComponent },
  { path: 'InputPea', component: PeaInputComponent },
  { path: 'peawallet', component: PeaWalletComponent },
  
  // Redirection si l'URL est inconnue
  { path: '**', redirectTo: '' }
];