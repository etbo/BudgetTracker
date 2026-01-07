// src/app/services/filters.service.ts

export interface FilterState {
  start?: string;
  end?: string;
  view?: string;
  excludedCategories?: string[];
  missingCat?: boolean;
  suggestedCat?: boolean;
  onlyCheques?: boolean;
}

export const filtersService = {
  // 1. Lire l'URL et transformer en objet propre
  getFilters(): FilterState {
    const params = new URLSearchParams(window.location.search);
    const view = params.get('view');
    const excludedRaw = params.get('excludedCategories');

    return {
      start: params.get('start') || undefined,
      end: params.get('end') || undefined,
      view: view || 'last',
      excludedCategories: excludedRaw ? excludedRaw.split(',') : [],
      missingCat: params.get('missingCat') === 'true',
      suggestedCat: params.get('suggestedCat') === 'true',
      onlyCheques: params.get('onlyCheques') === 'true'
    };
  },

  // 2. Mettre à jour l'URL intelligemment
  updateFilters(newState: FilterState) {
    const params = new URLSearchParams(window.location.search);
    const current = { ...this.getFilters(), ...newState };

    if (current.view === 'last' || current.view === 'all') {
      current.start = undefined;
      current.end = undefined;
    }

    Object.entries(current).forEach(([key, value]) => {
      if (value === true) {
        params.set(key, 'true');
      } else if (typeof value === 'string' && value) {
        params.set(key, value);
      } else if (Array.isArray(value) && value.length > 0) {
        // --- AJOUT ICI : Gestion du tableau ---
        params.set(key, value.join(','));
      } else {
        params.delete(key);
      }
    });

    const newPath = params.toString() ? '?' + params.toString() : '';
    window.history.pushState(null, '', window.location.pathname + newPath);
    window.dispatchEvent(new CustomEvent('filterChanged', { detail: current }));
  },

  reset() {
    window.history.pushState(null, '', window.location.pathname);
    window.dispatchEvent(new CustomEvent('filterChanged', { detail: { view: 'last' } }));
  }
};