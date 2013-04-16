using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
