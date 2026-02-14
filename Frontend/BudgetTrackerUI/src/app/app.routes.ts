import { Routes } from '@angular/router';
import { CcDashboard } from './cc-dashboard/cc-dashboard';
import { CcInput } from './cc-input/cc-input';
import { PeaOperations } from './pea-operations/pea-operations';
import { CcOperations } from './cc-operations/cc-operations';
import { CcCategories } from './cc-categories/cc-categories';
import { PeaDashboard } from './pea-dashboard/pea-dashboard';
import { CcRules } from './cc-rules/cc-rules';
import { SavingsStatement } from './savings-statement/savings-statement';
import { LifeInsuranceStatement } from './life-insurance-statement/life-insurance-statement';
import { GlobalDashboard } from './global-dashboard/global-dashboard';
import { AccountList } from './account-list/account-list';

export const routes: Routes = [
  // { path: '', component: Home },

  // Global dashboard
  { path: '', component: GlobalDashboard },

  // All accounts
  { path: 'account-list', component: AccountList },

  // Comptes courant : cc
  { path: 'cc-dashboard', component: CcDashboard },
  { path: 'cc-categories', component: CcCategories },
  { path: 'cc-rules', component: CcRules },
  { path: 'cc-operations', component: CcOperations },
  { path: 'cc-input', component: CcInput },

  // pea
  { path: 'pea-input', component: PeaOperations },
  { path: 'pea-dashboard', component: PeaDashboard },

  // savings
  { path: 'savings-statement', component: SavingsStatement },
  
  // Assurance vie
  { path: 'li-statement', component: LifeInsuranceStatement },
  
  // Redirection si l'URL est inconnue
  { path: '**', redirectTo: '' }
];