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
export const amountParser = (params: ValueParserParams): number => {
  if (params.newValue === null || params.newValue === '') return 0;
  // Remplace la virgule par un point et supprime les espaces
  const cleaned = params.newValue.toString().replace(',', '.').replace(/\s/g, '');
  return parseFloat(cleaned);
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
  const d = new Date(params.newValue);
  if (isNaN(d.getTime())) return false;
  const year = d.getFullYear();
  const month = String(d.getMonth() + 1).padStart(2, '0');
  const day = String(d.getDate()).padStart(2, '0');
  params.data[params.colDef.field!] = `${year}-${month}-${day}`;
  return true;
};