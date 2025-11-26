public static class CsvParserFactory
{
    public static IBanqueParser? GetParser(string fileContent)
    {

        if (fileContent.StartsWith("Date op�ration;Date valeur;libell�;D�bit;Cr�dit;") || fileContent.Contains("Date opération;Date valeur;libellé;Débit;Crédit;")) // ou analyser colonnes
        {
            return new FortuneoCsvParser();
        }

        if (fileContent.StartsWith("Date;")) // ou analyser colonnes
        {
            return new GsheetsCsvParser();
        }

        if (fileContent.StartsWith("Entry Date,Transaction Type,Transaction Description,Transaction Amount,Statement_Balance,flow")) // ou analyser colonnes
        {
            return new MaybankCsvParser();
        }

        return null;
    }
}
