namespace Library.Utility
{
    public static class Extensions
    {
        public static IEnumerable<TSource> MaxsBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            return source.GroupBy(keySelector).MaxBy(g => g.Key);
        }
    }
}
