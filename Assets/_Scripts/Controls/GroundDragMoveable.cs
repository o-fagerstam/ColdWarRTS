using System.Collections.Generic;
namespace Controls {
	public interface IGroundDragMovable {
		public IEnumerable<GroundDraggable> GroundDraggables { get; }
		public void EnableHandles ();
		public void DisableHandles ();
	}
}
