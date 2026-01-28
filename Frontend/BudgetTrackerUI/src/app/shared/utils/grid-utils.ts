import { ValueFormatterParams, ValueParserParams, ValueSetterParams } from 'ag-grid-community';

/**
 * Formatteur pour les devises (Ex: 12.5 -> 12,50 €)
 */
export const currencyFormatter = (params: ValueFormatterParams): string => {
  if (params.value === null || params.value === undefined) return '';
  return new Intl.NumberFormat('fr-FR', {
    style: 'currency',
    currency: 'EUR',
  }).format(params.value);
};

/**
 * Parseur pour convertir la saisie texte en nombre
 * Gère les virgules et les espaces
 */
export const amountParser = (params: any): number | null => {
  // Si vide ou null, on renvoie null pour la DB
  if (params.newValue === null || params.newValue === undefined || params.newValue === '') {
    return null;
  }

  const cleaned = params.newValue.toString().replace(',', '.').replace(/\s/g, '');
  const parsed = parseFloat(cleaned);

  return isNaN(parsed) ? null : parsed;
};

export const customDateFormatter = (params: ValueFormatterParams): string => {
  if (!params.value) return '';
  const date = new Date(params.value);
  if (isNaN(date.getTime())) return params.value;
  return new Intl.DateTimeFormat('fr-FR', {
    day: 'numeric',
    month: 'short',
    year: 'numeric'
  }).format(date).replace('.', '');
};

export const localDateSetter = (params: ValueSetterParams): boolean => {
  if (!params.newValue) {
    params.data[params.colDef.field!] = null;
    return true;
  }

  let datePart: string = '';

  if (params.newValue instanceof Date) {
    // Si l'éditeur renvoie un objet Date, on extrait YYYY-MM-DD
    const d = params.newValue;
    const year = d.getFullYear();
    const month = String(d.getMonth() + 1).padStart(2, '0');
    const day = String(d.getDate()).padStart(2, '0');
    datePart = `${year}-${month}-${day}`;
  } else {
    // Si c'est déjà une chaîne, on ne garde que les 10 premiers caractères
    datePart = String(params.newValue).substring(0, 10);
  }

  // On force l'heure à midi (format ISO 8601 compréhensible par .NET)
  params.data[params.colDef.field!] = `${datePart}T12:00:00`;

  return true;
};

export const formatDateFr = (value: any): string => {
  if (!value) return '';
  const date = new Date(value);
  if (isNaN(date.getTime())) return value;
  
  return new Intl.DateTimeFormat('fr-FR', {
    day: 'numeric',
    month: 'short',
    year: 'numeric'
  }).format(date).replace('.', '');
};