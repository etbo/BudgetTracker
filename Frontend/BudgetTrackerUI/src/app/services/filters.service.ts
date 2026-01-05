// src/app/services/filters.service.ts

export interface FilterState {
  start?: string;
  end?: string;
  view?: string;
  missingCat?: boolean;
  suggestedCat?: boolean;
  onlyCheques?: boolean;
}

export const filtersService = {
  // 1. Lire l'URL et transformer en objet propre
  getFilters(): FilterState {
    const params = new URLSearchParams(window.location.search);
    const view = params.get('view');

    return {
      start: params.get('start') || undefined,
      end: params.get('end') || undefined,
      view: view || 'last',
      missingCat: params.get('missingCat') === 'true',
      suggestedCat: params.get('suggestedCat') === 'true',
      onlyCheques: params.get('onlyCheques') === 'true'
    };
  },

  // 2. Mettre à jour l'URL intelligemment
  updateFilters(newState: FilterState) {
    const params = new URLSearchParams(window.location.search);
    
    // On récupère l'état actuel et on fusionne avec les changements
    const current = { ...this.getFilters(), ...newState };

    // --- LOGIQUE DE NETTOYAGE ---
    // Si on n'est pas en mode 'custom', on force la suppression des dates
    if (current.view !== 'custom') {
      current.start = undefined;
      current.end = undefined;
    }

    // On applique tout l'objet à l'URL
    Object.entries(current).forEach(([key, value]) => {
      if (value === true) {
        // Pour les booléens (Chips)
        params.set(key, 'true');
      } else if (value && typeof value === 'string') {
        // Pour les strings (view, dates)
        params.set(key, value);
      } else {
        // Si c'est false, undefined ou null -> On dégage de l'URL
        params.delete(key);
      }
    });

    // Mise à jour de la barre d'adresse
    const newUrl = window.location.pathname + '?' + params.toString();
    window.history.pushState(null, '', newUrl);

    // Notification globale pour que les composants se rafraîchissent
    window.dispatchEvent(new CustomEvent('filterChanged', { detail: current }));
  },

  reset() {
    window.history.pushState(null, '', window.location.pathname);
    window.dispatchEvent(new CustomEvent('filterChanged', { detail: {} }));
  }
};