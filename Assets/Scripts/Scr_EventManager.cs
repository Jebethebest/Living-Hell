using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Scr_EventManager : MonoBehaviour 
{
    private Dictionary<string, UnityEvent> m_EventDictionary;
    private static Scr_EventManager m_EventManager;

    public static Scr_EventManager Instance
    {
        get
        {
            if (!m_EventManager)
            {
                m_EventManager = FindObjectOfType(typeof(Scr_EventManager)) as Scr_EventManager;

                if (!m_EventManager)
                    Debug.Log("No active EventManager script found in the current scene");
                else
                    m_EventManager.Init();
            }
            return m_EventManager;
        }
    }

    private void Init()
    {
        if (m_EventDictionary == null)
            m_EventDictionary = new Dictionary<string, UnityEvent>();
    }

    public static void StartListening(string eventName, UnityAction listener)
    {
        UnityEvent thisEvent = null;

        if (Instance.m_EventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new UnityEvent();
            thisEvent.AddListener(listener);
            Instance.m_EventDictionary.Add(eventName, thisEvent);
        }
    }

    public static void StopListening(string eventName, UnityAction listener)
    {
        if (m_EventManager == null) return;

        UnityEvent thisEvent = null;

        if (Instance.m_EventDictionary.TryGetValue(eventName, out thisEvent))
            thisEvent.RemoveListener(listener);
    }

    public static void TriggerEvent(string eventName)
    {
        UnityEvent thisEvent = null;

        if (Instance.m_EventDictionary.TryGetValue(eventName, out thisEvent))
            thisEvent.Invoke();
    }
}
