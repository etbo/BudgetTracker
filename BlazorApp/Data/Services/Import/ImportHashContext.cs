namespace BlazorApp.Data.Services.Import
{
    public class ImportHashContext
    {
        private readonly Dictionary<string, int> _hashOccurrences = new();

        public string GetUniqueHash(string baseHash)
        {
            if (!_hashOccurrences.TryGetValue(baseHash, out int count))
                count = 0;

            count++;
            _hashOccurrences[baseHash] = count;

            if (count == 1)
                return $"{baseHash}";
            else
                return $"{baseHash}#{count}";
        }
    }
}