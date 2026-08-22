using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUIManager : MonoBehaviour
{
    [Header("Item Scriptable Objects")]
    public List<ItemSO> allItems = new List<ItemSO>();

    [Header("UI References")]
    [SerializeField] private Image equippedItemImage;
    [SerializeField] private Image secondaryItemImage;

    [Header("Settings")]
    [SerializeField] private Color emptySlotColor = new Color(1f, 1f, 1f, 0.3f);
    [SerializeField] private Color filledSlotColor = new Color(1f, 1f, 1f, 1f);

    void Update()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        ItemSO equippedItem = null;
        ItemSO secondaryItem = null;

        // Find the equipped item and the secondary item
        foreach (ItemSO item in allItems)
        {
            if (item == null) continue;

            if (item.isEquipped)
            {
                equippedItem = item;
            }
            else if (item.inInventory && !item.isEquipped)
            {
                secondaryItem = item;
            }
        }

        // Update equipped item UI
        if (equippedItemImage != null)
        {
            if (equippedItem != null && equippedItem.item_sprite != null)
            {
                equippedItemImage.sprite = equippedItem.item_sprite;
                equippedItemImage.color = filledSlotColor;
                equippedItemImage.enabled = true;
            }
            else
            {
                equippedItemImage.sprite = null;
                equippedItemImage.color = emptySlotColor;
                equippedItemImage.enabled = false;
            }
        }

        // Update secondary item UI
        if (secondaryItemImage != null)
        {
            if (secondaryItem != null && secondaryItem.item_sprite != null)
            {
                secondaryItemImage.sprite = secondaryItem.item_sprite;
                secondaryItemImage.color = filledSlotColor;
                secondaryItemImage.enabled = true;
            }
            else
            {
                secondaryItemImage.sprite = null;
                secondaryItemImage.color = emptySlotColor;
                secondaryItemImage.enabled = false;
            }
        }
    }
}