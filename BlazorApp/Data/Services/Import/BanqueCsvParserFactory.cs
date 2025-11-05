public static class BanqueCsvParserFactory
{
    public static IBanqueCsvParser GetParser(string fileContent)
    {
        Console.WriteLine("test");

        if (fileContent.Contains("Description1")) // ou analyser colonnes
        {
            Console.WriteLine("Description1");
            return new Banque1CsvParser();
        }

        if (fileContent.Contains("Description2"))
        {
            Console.WriteLine("Description2");
            return new Banque2CsvParser();
        }

        Console.WriteLine("Aucun...");

        throw new Exception("Format de fichier non reconnu");
    }
}
