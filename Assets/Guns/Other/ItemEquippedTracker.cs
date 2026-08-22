using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemEquippedTracker : MonoBehaviour
{
    [Header("References")]
    public PlayerInventory playerInventory;

    private Dictionary<itemType, ItemSO> itemStates = new Dictionary<itemType, ItemSO>();

    void Start()
    {
        if (playerInventory == null)
        {
            Debug.LogError("PlayerInventory reference is missing in ItemEquippedTracker.");
            return;
        }

        AddItemSO(playerInventory.Atlas_item);
        AddItemSO(playerInventory.Falcon_item);
        AddItemSO(playerInventory.Gautlet_item);
        AddItemSO(playerInventory.Johnson_item);
        AddItemSO(playerInventory.Raptor_item);
        AddItemSO(playerInventory.Spleefer_item);
        AddItemSO(playerInventory.Sprinkler_item);
        AddItemSO(playerInventory.STAR_item);
        AddItemSO(playerInventory.Sweeper_item);
        AddItemSO(playerInventory.Vanguard_item);
    }

    void Update()
    {
        foreach (var pair in itemStates)
        {
            itemType type = pair.Key;
            ItemSO itemSO = pair.Value;

            // Check if item is actually in the player's inventory list
            bool isInInventory = playerInventory.inventoryList.Contains(type);
            itemSO.inInventory = isInInventory;

            // Check if item is currently equipped (selected and active)
            if (isInInventory)
            {
                bool isSelected = playerInventory.inventoryList[playerInventory.selectedItem] == type;
                bool isActive = playerInventory.itemSetActive[type].activeSelf;
                itemSO.isEquipped = isSelected && isActive;
            }
            else
            {
                itemSO.WasInInventory = true;
                itemSO.isEquipped = false;
            }
        }
    }

    private void AddItemSO(GameObject itemObj)
    {
        if (itemObj != null)
        {
            Item item = itemObj.GetComponent<Item>();
            if (item != null && item.itemScriptableObject != null)
            {
                itemStates[item.itemScriptableObject.item_type] = item.itemScriptableObject;
            }
            else
            {
                Debug.LogWarning($"Item component or ItemSO missing on: {itemObj.name}");
            }
        }
    }
}