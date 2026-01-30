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

  // ... (reste du code identique)

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
