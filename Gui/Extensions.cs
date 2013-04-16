using System.Collections.Generic;

namespace Argscope
{
	static class Extensions
	{
		public static void AddRange<T>(this ICollection<T> coll, IEnumerable<T> toadd)
		{
			lock(coll)
				foreach (T t in toadd)
					coll.Add(t);
		}
	}
}
