using CsvHelper;

public static class ParserFactory
{
    public static IBankParser? GetParser(ParserInputContext ctx)
    {
        // 1) Détection Excel (extension + MIME)
        if (ctx.FileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase) ||
            ctx.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
        {
            return new CreditAgricoleExcelParser();
        }

        // 2) Détection CSV (fallback texte)
        if (!string.IsNullOrEmpty(ctx.TextContent))
        {
            // tu gardes ta logique actuelle
            return CsvParserFactory.GetParser(ctx.TextContent);
        }

        return null;

    }
}
