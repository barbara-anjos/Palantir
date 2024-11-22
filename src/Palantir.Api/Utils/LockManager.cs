using System.Collections.Generic;

namespace Palantir.Api.Utils
{
	/// <summary>
	/// Manages lock list for parallel notifications
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class LockManager<T>
	{
		private readonly HashSet<T> _lockSet;

		public LockManager()
		{
			_lockSet = new HashSet<T>();
		}

		public bool Lock(T item)
		{
			lock (_lockSet)
			{
				if (_lockSet.Contains(item))
				{
					return false;
				}

				_lockSet.Add(item);
				return true;
			}
		}

		public void Unlock(T item)
		{
			lock (_lockSet)
			{
				_lockSet.Remove(item);
			}
		}
	}
}
