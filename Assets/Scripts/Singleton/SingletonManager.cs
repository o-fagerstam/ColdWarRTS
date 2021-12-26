using System;
using System.Collections.Generic;
namespace Singleton {
	public class SingletonManager {

		private static readonly Dictionary<Type, ASingleton> Singletons = new Dictionary<Type, ASingleton>();

		public static void Register<T> (T obj) where T : ASingleton {
			if (Singletons.ContainsKey(obj.GetType())) {
				throw new ArgumentException($"A singleton of type {obj.GetType()} is already registered");
			}
			Singletons[obj.GetType()] = obj;
		}

		public static void Unregister<T> (T obj) where T : ASingleton {
			if (!Singletons.Remove(obj.GetType())) {
				throw new ArgumentException($"No singleton of type {obj.GetType()} is registered.");
			}
		}

		public static T Retrieve<T> () where T : ASingleton {
			if (!Singletons.ContainsKey(typeof(T))) {
				throw new ArgumentException($"No singleton of type {typeof(T)} is registered.");
			}
			return (T) Singletons[typeof(T)];
		}
	}
}
