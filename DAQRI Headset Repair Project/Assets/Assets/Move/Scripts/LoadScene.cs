using UnityEngine;
using UnityEngine.EventSystems;
using JetBrains.Annotations;

public class LoadScene : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public int sceneIndex = 0;
    public KeyCode activationKey;

    #pragma warning disable 0649
    [SerializeField, UsedImplicitly] private EventTrigger.TriggerEvent triggerEventEnter;
    #pragma warning restore 0649
    #pragma warning disable 0649
    [SerializeField, UsedImplicitly] private EventTrigger.TriggerEvent triggerEventExit;
    #pragma warning restore 0649

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.gameObject == gameObject)
        {
            DoKey();
        }
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        triggerEventEnter.Invoke (eventData);
    }
    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        triggerEventExit.Invoke (eventData);
    }

    void Update()
    {
        if (Input.GetKeyDown(activationKey))
        {
            DoKey();
        }
    }

    private void DoKey()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
