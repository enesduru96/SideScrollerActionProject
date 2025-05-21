using UnityEngine;

public abstract class ConsumableItem : ScriptableObject
{
    public abstract void ConsumeItem();

    public eConsumableItemSlot ConsumableSlot;

    public bool isEquipped;
}
