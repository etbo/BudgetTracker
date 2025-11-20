public class ParserInputContext
{
    public string? TextContent { get; set; }
    public Stream? FileStream { get; set; }
    public string FileName { get; set; } = "";
    public string ContentType { get; set; } = "";

    public TextReader? GetTextReader()
    {
        // Priorité : TextContent déjà lu (cas CSV)
        if (!string.IsNullOrEmpty(TextContent))
            return new StringReader(TextContent);

        // Sinon, si le fichier est textuel (.csv)
        if (FileStream != null && 
            FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
        {
            FileStream.Position = 0;
            return new StreamReader(FileStream);
        }

        // Excel ou non-text → pas de TextReader possible
        return null;
    }
}
