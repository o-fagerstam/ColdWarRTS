using System;
using System.Runtime.Serialization;
using JetBrains.Annotations;
namespace Utils {
	public class GroundRaycastException : Exception {
		public GroundRaycastException () {
		}
		protected GroundRaycastException ([NotNull] SerializationInfo info, StreamingContext context) : base(info, context) {
		}
		public GroundRaycastException (string message) : base(message) {
		}
		public GroundRaycastException (string message, Exception innerException) : base(message, innerException) {
		}
	}
}
