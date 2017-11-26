using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour {

    private Dictionary<string, UnityEvent> eventsDictionary;

    private static EventManager eventManager;
    
    private static EventManager instance
    {
        get
        {
            if(!eventManager)
            {
                eventManager = FindObjectOfType(typeof(EventManager)) as EventManager;

                //If not set, we have no objects with this script attached in the scene
                if(!eventManager)
                    Debug.LogError("Unable to locate an active EventManager script in the scene, attach one to a GameObject.");
                else
                {
                    eventManager.Init();
                }
            }
            return eventManager;
        }
    }

    private void Init()
    {
       if(eventsDictionary == null)
        {
            eventsDictionary = new Dictionary<string, UnityEvent>();
        }
    }

    public static void StartListening(string eventName, UnityAction listener)
    {
        UnityEvent thisEvent = null;

        if (instance.eventsDictionary.TryGetValue(eventName, out thisEvent))
            thisEvent.AddListener(listener);
        else
        {
            thisEvent = new UnityEvent();
            thisEvent.AddListener(listener);
            instance.eventsDictionary.Add(eventName, thisEvent);
        }
    }

    public static void StopListening(string eventName, UnityAction listener)
    {
        if (eventManager == null) return;
        UnityEvent thisEvent = null;

        if (instance.eventsDictionary.TryGetValue(eventName, out thisEvent))
            thisEvent.RemoveListener(listener);
    }

    public static void TriggerEvent(string triggeredEvent)
    {
        UnityEvent thisEvent = null;
        if (instance.eventsDictionary.TryGetValue(triggeredEvent, out thisEvent))
            thisEvent.Invoke();
    }
}
