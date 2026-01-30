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
  // 1. Cas "All" : On nettoie explicitement les dates
  if (state.view === 'all') {
    return {
      ...state,
      start: undefined,
      end: undefined
    };
  }

  // 2. Cas "LastX" : On calcule les dates dynamiquement
  if (state.view?.startsWith('last') && state.view !== 'last') {
    const monthsToSubtract = parseInt(state.view.replace('last', ''), 10);

    if (!isNaN(monthsToSubtract)) {
      const now = new Date();
      const currentYear = now.getFullYear();
      const currentMonth = now.getMonth();

      // Fin = dernier jour du mois précédent
      const endOfRange = new Date(currentYear, currentMonth, 0);
      // Début = 1er jour du mois X mois en arrière
      const startOfRange = new Date(currentYear, currentMonth - monthsToSubtract, 1);

      const formatDate = (d: Date) => {
        return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')}`;
      };

      return {
        ...state,
        start: formatDate(startOfRange),
        end: formatDate(endOfRange)
      };
    }
  }

  // 3. Cas par défaut (Custom, undefined, ou 'last' sans chiffre) : On ne touche à rien
  return state;
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
    const current = computeDates({ ...this.getFilters(), ...newState });
    const params = new URLSearchParams();

    Object.entries(current).forEach(([key, value]) => {
      if (value === undefined || value === null || value === false) return;
      if (Array.isArray(value)) {
        if (value.length > 0) params.set(key, value.join(','));
      } else {
        params.set(key, value.toString());
      }
    });

    const queryString = params.toString();

    // --- CORRECTION ICI ---
    // On garde le pathname actuel (ex: /cc-dashboard) et on ajoute les params
    const newPath = window.location.pathname + (queryString ? '?' + queryString : '');

    window.history.pushState(null, '', newPath);
    localStorage.setItem(STORAGE_KEY, JSON.stringify(current));

    filtersSubject.next(current);
  },

  reset() {
    localStorage.removeItem(STORAGE_KEY);
    this.updateFilters({ view: 'last6', start: undefined, end: undefined, excludedCategories: [] });
  }
};
