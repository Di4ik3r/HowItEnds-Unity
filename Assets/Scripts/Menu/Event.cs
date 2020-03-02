using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Menu
{
    class Event : MonoBehaviour
    {
        [SerializeField]
        UnityEvent anEvent;

        private void OnMouseDown()
        {
            anEvent.Invoke();
        }
    }
}
