using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace TournamentManager.Core.Handler
{
    public static class ExtensionMethods
    {
        public static void Sort<TSource, TKey>(this ObservableCollection<TSource> collection, Func<TSource, TKey> keySelector)
        {
            List<TSource> sorted = collection.OrderBy(keySelector).ToList();
            for (int i = 0; i < sorted.Count(); i++)
                collection.Move(collection.IndexOf(sorted[i]), i);
        }
    }
}
