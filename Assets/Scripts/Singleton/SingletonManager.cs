using System;
using System.Collections.Generic;
namespace Singleton {
	public class SingletonManager {

		private static readonly Dictionary<Type, ISingleton> Singletons = new Dictionary<Type, ISingleton>();

		public static void Register<T> (T obj) where T : ISingleton {
			if (Singletons.ContainsKey(obj.GetType())) {
				throw new ArgumentException($"A singleton of type {obj.GetType()} is already registered");
			}
			Singletons[obj.GetType()] = obj;
		}

		public static void Unregister<T> (T obj) where T : ISingleton {
			if (!Singletons.Remove(obj.GetType())) {
				throw new ArgumentException($"No singleton of type {obj.GetType()} is registered.");
			}
		}

		public static T Retrieve<T> () where T : ISingleton {
			if (!Singletons.ContainsKey(typeof(T))) {
				throw new ArgumentException($"No singleton of type {typeof(T)} is registered.");
			}
			return (T) Singletons[typeof(T)];
		}

		public static bool IsRegistered<T> () where T : ISingleton => Singletons.ContainsKey(typeof(T));
	}
}
