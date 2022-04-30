using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InventoryItem", menuName = "Project Element/Inventory Item")]
[System.Serializable]
public class InventoryItem : ScriptableObject
{
    public int ItemID;
    public string ItemName;
    public string ItemTitle;
    public string ItemDescription;

    public InventoryItemType ItemType;

    public Sprite ItemIcon;
    public GameObject ItemPrefab;
}
