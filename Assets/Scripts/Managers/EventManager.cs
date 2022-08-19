using System;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    private static EventManager eventManager;
    private Dictionary<string, Action<Dictionary<string, object>>> eventDictionary;

    public static EventManager instance
    {
        get
        {
            if (eventManager) return eventManager;
            eventManager = FindObjectOfType(typeof(EventManager)) as EventManager;

            if (!eventManager)
            {
                Debug.LogError("There needs to be one active EventManager script on a GameObject in your scene.");
            }
            else
            {
                eventManager.Init();

                //  Sets this to not be destroyed when reloading scene
                DontDestroyOnLoad(eventManager);
            }

            return eventManager;
        }
    }

    private void Init()
    {
        eventDictionary ??= new Dictionary<string, Action<Dictionary<string, object>>>();
    }

    public static void StartListening(string eventName, Action<Dictionary<string, object>> listener)
    {
        Action<Dictionary<string, object>> thisEvent;

        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent += listener;
            instance.eventDictionary[eventName] = thisEvent;
        }
        else
        {
            thisEvent += listener;
            instance.eventDictionary.Add(eventName, thisEvent);
        }
    }

    public static void StopListening(string eventName, Action<Dictionary<string, object>> listener)
    {
        if (eventManager == null) return;
        Action<Dictionary<string, object>> thisEvent;
        if (!instance.eventDictionary.TryGetValue(eventName, out thisEvent)) return;
        thisEvent -= listener;
        instance.eventDictionary[eventName] = thisEvent;
    }

    public static void TriggerEvent(string eventName, Dictionary<string, object> message)
    {
        Action<Dictionary<string, object>> thisEvent = null;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent)) thisEvent.Invoke(message);
    }
}