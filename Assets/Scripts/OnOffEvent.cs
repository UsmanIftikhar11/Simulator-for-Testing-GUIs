using UnityEngine;
using UnityEngine.Events;


public class OnOffEvent : MonoBehaviour
{
    [Header("Events")]
    public UnityEvent onEnableEvent;
    public UnityEvent onDisableEvent;

    private void OnEnable()
    {
        onEnableEvent?.Invoke();
    }

    private void OnDisable()
    {
        onDisableEvent?.Invoke();
    }
}
