using System;
using System.Collections.Generic;
using System.Linq;

namespace RxDivideAggregate
{
    public static class LinqExtensions
    {
        /// <summary>
        /// This method works like python "itertools.groupby".
        /// Difference with Linq GroupBy is that GroupSeqBy is grouping sequentially elements with equal until first element with another key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TG"></typeparam>
        /// <param name="dataSource"></param>
        /// <param name="groupKeySelector">NotNull parameters</param>
        /// <returns></returns>
        public static IEnumerable<IGrouping<TG, T>> GroupSeqBy<T, TG>(this IEnumerable<T> dataSource, Func<T, TG> groupKeySelector)
        {
            if (dataSource == null) yield break;
            if (groupKeySelector == null) throw new ArgumentNullException(nameof(groupKeySelector));
            
            var currentGroupElements = new LinkedList<T>();
            var lastKey = default(TG);
            foreach (var element in dataSource)
            {
                var elementGroupKey = groupKeySelector(element);
                if (Equals(lastKey, elementGroupKey))
                {
                    currentGroupElements.AddLast(element);
                }
                else
                {
                    if (currentGroupElements.Count > 0)
                    {
                        yield return currentGroupElements.GroupBy(_ => lastKey).First();
                        currentGroupElements = new LinkedList<T>();
                    }
                    lastKey = elementGroupKey;
                    currentGroupElements.AddLast(element);
                }
            }
            if (currentGroupElements.Count > 0)
            {
                yield return currentGroupElements.GroupBy(_ => lastKey).First();
            }
        }

        private static bool Equals<TG>(TG firstKey, TG secondKey)
        {
            return (firstKey == null && secondKey == null) 
                   || (firstKey != null && firstKey.Equals(secondKey));
        }

    }
}
