using System;
using System.Collections.Generic;
using System.Linq;

namespace Testing.WPFBinding
{
    public static class TestExtensions
    {
        public static Boolean CompareCollection<TSource>(this ICollection<TSource> a, ICollection<TSource> b)
        {
            if (a != null && b != null)
            {
                var bools = from index in Enumerable.Range(0, a.Count)
                            let aE = a.ElementAt(index)
                            let bE = b.ElementAt(index)
                            select aE.Equals(bE);

                return bools.Aggregate(true,
                        (acc, mem) =>
                        {
                            return acc && mem;
                        });
            }

            return false;
        }
    }
}