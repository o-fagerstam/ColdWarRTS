using System;
using System.Collections.Generic;
namespace Singleton {
	public static class SingletonManager {

		private static readonly Dictionary<Type, ISingleton> Singletons = new Dictionary<Type, ISingleton>();
		public static event EventHandler<OnSingletonRegisteredArgs> OnSingletonRegistered;

		public static void Register<T> (T obj) where T : ISingleton {
			Type objType = obj.GetType();
			if (Singletons.ContainsKey(objType)) {
				throw new ArgumentException($"A singleton of type {objType} is already registered");
			}
			Singletons[objType] = obj;
			OnSingletonRegistered?.Invoke(null, new OnSingletonRegisteredArgs {RegisteredType = objType});
		}

		public static void Unregister<T> (T obj) where T : ISingleton {
			Type objType = obj.GetType();
			if (!Singletons.Remove(objType)) {
				throw new ArgumentException($"No singleton of type {objType} is registered.");
			}
		}

		public static T Retrieve<T> () where T : ISingleton {
			Type objType = typeof(T);
			if (!Singletons.ContainsKey(objType)) {
				throw new ArgumentException($"No singleton of type {objType} is registered.");
			}
			return (T)Singletons[objType];
		}
		

		public static bool TryRetrieveAnySubclass<T> (out T singleton) where T : ISingleton {
			Type parentType = typeof(T);
			foreach (KeyValuePair<Type,ISingleton> pair in Singletons) {
				if (pair.Key == parentType || pair.Key.IsSubclassOf(parentType)) {
					singleton = (T) pair.Value;
					return true;
				}
			}
			singleton = default;
			return false;
		}
		
		public class OnSingletonRegisteredArgs : EventArgs {
			public Type RegisteredType;
		}
	}
}
