using UnityEngine;

namespace OwlTawitTawoo
{
    public class SetStateOnStart : MonoBehaviour
    {
        [SerializeField] private StateManager.State _state;

        private void Start()
        {
            StateManager.Set(_state);
        }
    }
}
