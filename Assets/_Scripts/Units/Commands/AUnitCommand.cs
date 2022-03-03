using System;
namespace Units.Commands {
	public abstract class AUnitCommand {
		protected Unit _commandOwner;
		public event EventHandler<OnCommandFinishedArgs> OnCommandFinished; 
		public AUnitCommand (Unit commandOwner) {
			_commandOwner = commandOwner;
		}

		public abstract void DoCommand ();

		public class OnCommandFinishedArgs : EventArgs {
			public AUnitCommand Command;
		}

		protected void InvokeOnCommandFinished () {
			OnCommandFinished?.Invoke(this, new OnCommandFinishedArgs(){Command = this});
		}
	}
}
