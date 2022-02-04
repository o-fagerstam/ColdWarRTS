using System;
using System.Collections.Generic;
namespace Singleton {
	public static class SingletonManager {

		private static readonly Dictionary<Type, ASingletonMonoBehaviour> Singletons = new Dictionary<Type, ASingletonMonoBehaviour>();
		public static event EventHandler<OnSingletonRegisteredArgs> OnSingletonRegistered;

		public static void Register<T> (T obj) where T : ASingletonMonoBehaviour {
			Type objType = obj.GetType();
			if (Singletons.ContainsKey(objType)) {
				throw new ArgumentException($"A singleton of type {objType} is already registered");
			}
			Singletons[objType] = obj;
			OnSingletonRegistered?.Invoke(null, new OnSingletonRegisteredArgs {registeredType = objType});
		}

		public static void Unregister<T> (T obj) where T : ASingletonMonoBehaviour {
			Type objType = obj.GetType();
			if (!Singletons.Remove(objType)) {
				throw new ArgumentException($"No singleton of type {objType} is registered.");
			}
		}

		public static T Retrieve<T> () where T : ASingletonMonoBehaviour {
			Type objType = typeof(T);
			if (!Singletons.ContainsKey(objType)) {
				throw new ArgumentException($"No singleton of type {objType} is registered.");
			}
			return (T)Singletons[objType];
		}

		public static bool TryRetrieve<T> (out T singleton) where T : ASingletonMonoBehaviour {
			if (Singletons.ContainsKey(typeof(T))) {
				singleton = (T)Singletons[typeof(T)];
				return true;
			}
			singleton = null;
			return false;
		}

		public static bool TryRetrieveAnySubclass<T> (out T singleton) where T : ASingletonMonoBehaviour {
			Type parentType = typeof(T);
			foreach (KeyValuePair<Type,ASingletonMonoBehaviour> pair in Singletons) {
				if (pair.Key == parentType || pair.Key.IsSubclassOf(parentType)) {
					singleton = (T) pair.Value;
					return true;
				}
			}
			singleton = null;
			return false;
		}

		public static bool IsRegistered<T> () where T : ASingletonMonoBehaviour => Singletons.ContainsKey(typeof(T));

		public class OnSingletonRegisteredArgs : EventArgs {
			public Type registeredType;
		}
	}
}
