using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(EventSystem))]
public class EventSystemHelper : MonoBehaviour
{
    [SerializeField, ReadOnly]
    private EventSystem _eventSystem;

    private GameObject _cacheObject;

    private void OnValidate()
    {
        _eventSystem = GetComponent<EventSystem>();
    }

    private void Update()
    {
        if (_eventSystem.currentSelectedGameObject != null && _eventSystem.currentSelectedGameObject != _cacheObject)
        {
            _cacheObject = _eventSystem.currentSelectedGameObject;
        }
        else if (_cacheObject != null && _eventSystem.currentSelectedGameObject == null)
        {
            _eventSystem.SetSelectedGameObject(_cacheObject);
        }
    }
}
