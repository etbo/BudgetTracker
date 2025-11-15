public static class BanqueCsvParserFactory
{
    public static IBanqueParser GetParser(string fileContent)
    {

        if (fileContent.Contains("Date op�ration;Date valeur;libell�;D�bit;Cr�dit;") || fileContent.Contains("Date opération;Date valeur;libellé;Débit;Crédit;")) // ou analyser colonnes
        {
            return new FortuneoCsvParser();
        }

        if (fileContent.Contains("xxDate;")) // ou analyser colonnes
        {
            return new GsheetsCsvParser();
        }

        throw new Exception("Aucune correspondance avec un fichier de banque connu");
    }
}
