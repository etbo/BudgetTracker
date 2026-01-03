export interface FilterState {
    start: string;
    end: string;
}

export class FiltersService {
    private defaultState: FilterState = {
        start: '',
        end: ''
    };

    constructor() {
        // Au chargement du service, on synchronise l'état avec l'URL
        this.syncFromUrl();
    }

    /**
     * Récupère l'état actuel des filtres depuis l'URL
     */
    getFilters(): FilterState {
        const params = new URLSearchParams(window.location.search);
        return {
            start: params.get('start') || this.defaultState.start,
            end: params.get('end') || this.defaultState.end
        };
    }

    /**
     * Met à jour les filtres et l'URL
     */
    updateFilters(partialState: Partial<FilterState>): void {
        const currentState = this.getFilters();
        const newState = { ...currentState, ...partialState };

        const params = new URLSearchParams();

        if (newState.start) params.set('start', newState.start);
        if (newState.end) params.set('end', newState.end);

        // Mise à jour de l'URL sans recharger la page
        const newUrl = `${window.location.pathname}?${params.toString()}`;
        window.history.pushState(null, '', newUrl);

        // On prévient le reste de l'application que les filtres ont changé
        window.dispatchEvent(new CustomEvent('filterChanged', { detail: newState }));
    }

    /**
     * Réinitialise tous les filtres
     */
    reset(): void {
        window.history.pushState(null, '', window.location.pathname);
        window.dispatchEvent(new CustomEvent('filterChanged', { detail: this.defaultState }));
        
        // On force le rafraîchissement des éléments HTML si nécessaire
        this.syncFromUrl();
    }

    /**
     * Synchronise manuellement les éléments HTML de la Navbar avec l'URL
     */
    syncFromUrl(): void {
        const state = this.getFilters();
        
        const startEl = document.getElementById('startDate') as HTMLInputElement;
        const endEl = document.getElementById('endDate') as HTMLInputElement;

        if (startEl) startEl.value = state.start;
        if (endEl) endEl.value = state.end;
    }
}

// On exporte une instance unique (Singleton) pour toute l'app
export const filtersService = new FiltersService();