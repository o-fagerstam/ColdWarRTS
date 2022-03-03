using System;
namespace Architecture.Singleton {
	public class SingletonException : Exception {
		public SingletonException() {}
		public SingletonException (string message) : base(message) {
		}
		public SingletonException (string message, Exception innerException) : base(message, innerException) {
		}
	}
}
