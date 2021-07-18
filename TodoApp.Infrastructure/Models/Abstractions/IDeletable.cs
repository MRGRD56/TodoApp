using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TodoApp.Infrastructure.Models.Abstractions
{
    public interface IDeletable
    {
        public bool IsDeleted { get; set; }
    }

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