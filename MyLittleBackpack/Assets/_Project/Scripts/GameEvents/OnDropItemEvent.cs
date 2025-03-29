using UnityEngine;

public class OnDropItemEvent : GameEventBase
{
    public GameObject Item;

    public OnDropItemEvent(GameObject item)
    {
        Item = item;
    }
}
