namespace Architecture.StateMachine {
	public abstract class StateMachine<TContext, TState> where TState : State<TContext> {
		private TState _currentState;
		protected readonly TContext Context;

		public TState State {
			get => _currentState;
			set => SetState(value);
		}

		protected StateMachine (TContext context) {
			Context = context;
		}

		private void SetState (TState state) {
			_currentState?.ExitState();
			_currentState = state;
			_currentState?.EnterState();
		}
	}
}
