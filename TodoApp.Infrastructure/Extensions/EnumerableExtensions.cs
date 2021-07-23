using System.Collections.Generic;
using System.Linq;
using TodoApp.Infrastructure.Models.Abstractions;

namespace TodoApp.Infrastructure.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<TSource> NotDeleted<TSource>(this IEnumerable<TSource> source)
            where TSource : IDeletable =>
            source.Where(item => !item.IsDeleted);
        
        public static IQueryable<TSource> NotDeleted<TSource>(this IQueryable<TSource> source)
            where TSource : IDeletable =>
            source.Where(item => !item.IsDeleted);
    }
}