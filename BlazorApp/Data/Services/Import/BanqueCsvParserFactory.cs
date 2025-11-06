public static class BanqueCsvParserFactory
{
    public static IBanqueCsvParser GetParser(string fileContent)
    {

        if (fileContent.Contains("Date op�ration;Date valeur;libell�;D�bit;Cr�dit;")) // ou analyser colonnes
        {
            Console.WriteLine("Correspondance trouvée : Fortuneo");
            return new FortuneoCsvParser();
        }

        if (fileContent.Contains("Description2"))
        {
            Console.WriteLine("Description2");
            return new Banque2CsvParser();
        }

        Console.WriteLine("Aucun...");

        throw new Exception("Aucune correspondance avec un fichier de banque connu");
    }
}
