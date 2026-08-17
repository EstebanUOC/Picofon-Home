namespace Picofon.Activities.CrossRiver
{
    public class StateMachine
    {
        public delegate int UpdateDelegate();
        public delegate void ActionDelegate();

        private class StateClass
        {
            public UpdateDelegate Update;

            public ActionDelegate Begin;

            public ActionDelegate End;
        }

        private readonly StateClass[] _states;

        public int State { get; private set; }

        public StateMachine(int stateCount)
        {
            _states = new StateClass[stateCount];

            for (int i = 0; i < stateCount; i++)
            {
                _states[i] = new StateClass();
            }
        }

        public void SetCallback(
            int state,
            UpdateDelegate update,
            ActionDelegate begin = null,
            ActionDelegate end = null
        )
        {
            _states[state].Update = update;
            _states[state].Begin = begin;
            _states[state].End = end;
        }

        public void Update()
        {
            int nextState = _states[State].Update.Invoke();

            if (nextState != State)
            {
                _states[State].End?.Invoke();

                State = nextState;

                _states[State].Begin?.Invoke();
            }
        }

        public void ForceState(int state)
        {
            _states[State].End?.Invoke();

            State = state;

            _states[State].Begin?.Invoke();
        }
    }
}
