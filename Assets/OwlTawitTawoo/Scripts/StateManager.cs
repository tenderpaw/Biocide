using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace OwlTawitTawoo {

    public static class StateManager
    {
        public enum State
        {
            None, // start
            Intro,
            Game,
            Main,
            End,
            Shop
        }

        public static State state { get; private set; } = State.None;
        public static event UnityAction<State> stateChangedEvent;

        public static void Set(State newState)
        {
            if (state == newState)
                return;

            state = newState;
            stateChangedEvent?.Invoke(state);
        }

        public static bool Compare(State tgtState)
        {
            return state == tgtState;
        }

#if UNITY_EDITOR
        [MenuItem("Debug/State")]
        private static void PrintState()
        {
            Debug.LogFormat("State:{0}", state);
        }
#endif
    }
}
