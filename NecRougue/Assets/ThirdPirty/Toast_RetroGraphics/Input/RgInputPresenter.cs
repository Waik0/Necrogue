using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class RgInputPresenter : MonoBehaviour
{
    private static RgInputPresenter _instance;
    //Singleton
    public static RgInputPresenter Instance
    {
        get {
        if (_instance == null)
        {
            _instance = GameObject.FindObjectOfType<RgInputPresenter>();
        }
        return _instance;
        }
    }

    void Awake()
    {
        var trigger = gameObject.AddComponent<EventTrigger>();
        trigger.triggers = new List<EventTrigger.Entry>();
        var pushDown = new EventTrigger.Entry();
        pushDown.eventID = EventTriggerType.PointerDown;
        pushDown.callback.AddListener( OnPointerDown );
        trigger.triggers.Add(pushDown);
        var drag = new EventTrigger.Entry();
        drag.eventID = EventTriggerType.Drag;
        drag.callback.AddListener( OnDrag );
        trigger.triggers.Add(drag);
        var pushUp = new EventTrigger.Entry();
        pushUp.eventID = EventTriggerType.PointerUp;
        pushUp.callback.AddListener( OnPointerUp);
        trigger.triggers.Add(pushUp);
    }

    void OnPointerDown(BaseEventData e)
    {
        var pe = (PointerEventData) e;
        Debug.Log(pe.position);
        
    }
    void OnDrag(BaseEventData e)
    {
        var pe = (PointerEventData) e;
        Debug.Log(pe.position);
    }

    void OnPointerUp(BaseEventData e)
    {
        var pe = (PointerEventData) e;
        Debug.Log(pe.position);
    }


}
