namespace Persistence.Extensions
{
    public static class SearchExtensions
    {
        public static string ToFtsString(this string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return "";

            var words = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            return string.Join(" AND ", words.Select(w => $"\"{w}*\""));
        }
    }
}
