import { BehaviorSubject } from 'rxjs';

export interface FilterState {
  start?: string;
  end?: string;
  view?: string;
  excludedCategories?: string[];
  missingCat?: boolean;
  suggestedCat?: boolean;
  onlyCheques?: boolean;
}

const STORAGE_KEY = 'budget_tracker_filters';

/**
 * Calcule les dates si nécessaire
 */
function computeDates(state: FilterState): FilterState {
  const now = new Date();
  const today = now.toISOString().split('T')[0];
  let start = state.start;
  let end = state.end;

  if (state.view === 'last6') {
    const d = new Date();
    d.setMonth(d.getMonth() - 6);
    start = d.toISOString().split('T')[0];
    end = today;
  } else if (state.view === 'last12') {
    const d = new Date();
    d.setFullYear(d.getFullYear() - 1);
    start = d.toISOString().split('T')[0];
    end = today;
  } else if (state.view === 'last24') {
    const d = new Date();
    d.setFullYear(d.getFullYear() - 2);
    start = d.toISOString().split('T')[0];
    end = today;
  }

  return { ...state, start, end };
}

function getInitialState(): FilterState {
  const params = new URLSearchParams(window.location.search);
  
  // Si URL vide, on check le storage
  if (params.toString() === '' && localStorage.getItem(STORAGE_KEY)) {
    return computeDates(JSON.parse(localStorage.getItem(STORAGE_KEY)!));
  }

  return computeDates({
    start: params.get('start') || undefined,
    end: params.get('end') || undefined,
    view: params.get('view') || 'last6',
    excludedCategories: params.get('excludedCategories')?.split(',').filter(x => x) || [],
    missingCat: params.get('missingCat') === 'true',
    suggestedCat: params.get('suggestedCat') === 'true',
    onlyCheques: params.get('onlyCheques') === 'true'
  });
}

const filtersSubject = new BehaviorSubject<FilterState>(getInitialState());

export const filtersService = {
  filters$: filtersSubject.asObservable(),

  getFilters(): FilterState {
    return filtersSubject.value;
  },

  updateFilters(newState: Partial<FilterState>) {
    // 1. Fusionner l'ancien état avec le nouveau
    const current = computeDates({ ...this.getFilters(), ...newState });
    console.log('Filtres mis à jour (Objet) :', current);

    // 2. Construire les paramètres de l'URL
    const params = new URLSearchParams();
    
    Object.entries(current).forEach(([key, value]) => {
      // Sécurité : on n'ajoute que si la valeur existe
      if (value === undefined || value === null || value === false) return;

      if (Array.isArray(value)) {
        if (value.length > 0) params.set(key, value.join(','));
      } else {
        params.set(key, value.toString());
      }
    });

    const queryString = params.toString();
    const newPath = queryString ? '?' + queryString : window.location.pathname;
    
    console.log('Tentative écriture URL :', newPath);

    // 3. Mise à jour physique de l'URL et du Storage
    window.history.pushState(null, '', newPath);
    localStorage.setItem(STORAGE_KEY, JSON.stringify(current));

    // 4. Notification des composants
    filtersSubject.next(current);
  },

  reset() {
    localStorage.removeItem(STORAGE_KEY);
    this.updateFilters({ view: 'last6', start: undefined, end: undefined, excludedCategories: [] });
  }
};
