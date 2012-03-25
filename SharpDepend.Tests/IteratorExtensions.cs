using System;
using System.Collections.Generic;

namespace SharpDepend.Tests
{
	public static class IteratorExtensions
	{
		public static void ForEach<TObject>(this IEnumerable<TObject> iterator,
			Action<TObject> action)
		{
			iterator.ForEach((x,i) => action(x));
		}

		public static void ForEach<TObject, TResult>(this IEnumerable<TObject> iterator,
			Func<TObject, TResult> action)
		{
			iterator.ForEach((x, i) => { action(x); });
		}

		public static void ForEach<TObject>(this IEnumerable<TObject> iterator,
			Action<TObject, int> action)
		{
			var index = 0;
			foreach(var item in iterator)
				action(item, index++);
		}

		public static void ForEach<TObject, TResult>(this IEnumerable<TObject> iterator,
			Func<TObject, int, TResult> action)
		{
			iterator.ForEach((x, i) => { action(x, i); });
		}
	}
}