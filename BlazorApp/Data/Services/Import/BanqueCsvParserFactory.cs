public static class BanqueCsvParserFactory
{
    public static IBanqueCsvParser GetParser(string fileContent)
    {

        if (fileContent.Contains("Date op�ration;Date valeur;libell�;D�bit;Cr�dit;") || fileContent.Contains("Date opération;Date valeur;libellé;Débit;Crédit;")) // ou analyser colonnes
        {
            return new FortuneoCsvParser();
        }

        if (fileContent.Contains("xxDate;")) // ou analyser colonnes
        {
            return new GsheetsCsvParser();
        }

        if (fileContent.Contains("T�l�chargement du ") || fileContent.Contains("Téléchargement du "))
        {
            return new CreditAgricoleCsvParser();
        }

        throw new Exception("Aucune correspondance avec un fichier de banque connu");
    }
}
