using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Argscope
{
	static class Utils
	{
		public static void AddRange<T>(this ObservableCollection<T> collection,
			IEnumerable<T> toAdd)
		{
			foreach (T t in toAdd)
				collection.Add(t);
		}

		public static void RemoveRange<T>(this ObservableCollection<T> collection,
			IEnumerable<T> toRemove)
		{
			foreach (T t in toRemove)
				collection.Remove(t);
		}
	}
}
