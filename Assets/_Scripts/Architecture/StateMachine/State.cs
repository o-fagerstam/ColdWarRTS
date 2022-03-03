namespace Architecture.StateMachine {
	public abstract class State<T> {
		protected T Context;
		protected State (T context) {
			Context = context;
		}
		public abstract void EnterState ();
		public abstract void ExitState ();
	}
}
