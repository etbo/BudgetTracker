import { Injectable } from '@angular/core';

@Injectable({
    providedIn: 'root'
})
export class ThemeService {

    constructor() { }

    /**
     * Récupère une couleur définie dans styles.scss (:root)
     */
    getVariableColor(varName: string): string {
        // On s'assure que le nom commence bien par --
        const name = varName.startsWith('--') ? varName : `--${varName}`;
        return getComputedStyle(document.documentElement).getPropertyValue(name).trim();
    }

    /**
     * Logique centrale pour attribuer une variable CSS selon le nom du compte
     */
    getAccountColorVar(accountName: string): string {
        const name = accountName.toLowerCase();

        if (name.includes('cc')) return '--color-account-cc';
        if (name.includes('savings')) return '--color-account-savings';
        if (name.includes('pea')) return '--color-account-pea';
        if (name.includes('life-insurrance')) return '--color-account-life-insurrance';
        if (name.includes('ldd')) return '--color-success';

        return '--color-autre'; // Variable par défaut à ajouter dans ton styles.scss
    }

    /**
     * Retourne la couleur réelle (HEX/RGB) pour les outils type ApexCharts
     */
    getAccountColor(accountName: string): string {
        const varName = this.getAccountColorVar(accountName);
        return this.getVariableColor(varName);
    }

}