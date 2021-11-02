using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EventValue", menuName = "ScriptableObjects/EventsRegister", order = 1)]
public class EventValues : ScriptableObject
{
    public int eventStartIndex;
    [SerializeField]
    public anEvent[] eventList;

}
[System.Serializable]
public class anEvent
{
    public string eventName;
    public Color eventColor;
}
