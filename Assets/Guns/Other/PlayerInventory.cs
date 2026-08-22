using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerInventory : MonoBehaviour
{
    [Header("General")]
    public List<itemType> inventoryList = new List<itemType>();
    public int selectedItem;
    public float playerReach;
    [SerializeField] int maxInventorySize = 2;
    [SerializeField] float pickupCooldown = 1f;
    private float lastPickupTime = -1f;

    [Space(20)]
    [Header("Keys")]
    [SerializeField] KeyCode throwItemKey;
    [SerializeField] KeyCode pickItemKey;

    [Space(20)]
    [Header("Pickup UI")]
    [SerializeField] public TMP_Text pickupPromptText;
    [SerializeField] private string pickupPromptMessage = "Press E to Pick Up";
    [SerializeField] private float pickupDistance = 3f;

    [Space(20)]
    [Header("Item Gameobjects")]
    [SerializeField] public GameObject Atlas_item;
    [SerializeField] public GameObject Falcon_item;
    [SerializeField] public GameObject Gautlet_item;
    [SerializeField] public GameObject Johnson_item;
    [SerializeField] public GameObject Raptor_item;
    [SerializeField] public GameObject Spleefer_item;
    [SerializeField] public GameObject Sprinkler_item;
    [SerializeField] public GameObject STAR_item;
    [SerializeField] public GameObject Sweeper_item;
    [SerializeField] public GameObject Vanguard_item;

    [SerializeField] Camera Cam;

    public Dictionary<itemType, GameObject> itemSetActive = new Dictionary<itemType, GameObject>();

    [Space(20)]
    [Header("Scriptable Objects")]
    public ItemSO Atlas;
    public ItemSO Falcon;
    public ItemSO Gautlet;
    public ItemSO Johnson;
    public ItemSO Raptor;
    public ItemSO Spleefer;
    public ItemSO Sprinkler;
    public ItemSO STAR;
    public ItemSO Vanguard;
    public ItemSO Sweeper;

    void Start()
    {
        itemSetActive.Add(itemType.Atlas, Atlas_item);
        itemSetActive.Add(itemType.Falcon, Falcon_item);
        itemSetActive.Add(itemType.Gautlet, Gautlet_item);
        itemSetActive.Add(itemType.Johnson, Johnson_item);
        itemSetActive.Add(itemType.Raptor, Raptor_item);
        itemSetActive.Add(itemType.Spleefer, Spleefer_item);
        itemSetActive.Add(itemType.Sprinkler, Sprinkler_item);
        itemSetActive.Add(itemType.STAR, STAR_item);
        itemSetActive.Add(itemType.Sweeper, Sweeper_item);
        itemSetActive.Add(itemType.Vanguard, Vanguard_item);

        if (pickupPromptText != null)
            pickupPromptText.gameObject.SetActive(false);

        if (inventoryList.Count > 0)
            NewItemSelected();
    }

    void Update()
    {
        HandlePickup();
        HandleDrop();
        HandleEquip();
    }

    void HandlePickup()
    {
        Ray ray = Cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;

        bool canPickup = false;

        if (Physics.Raycast(ray, out hitInfo, playerReach))
        {
            IPickable item = hitInfo.collider.GetComponent<IPickable>();

            if (item != null)
            {
                float distance = Vector3.Distance(transform.position, hitInfo.collider.transform.position);

                if (distance <= pickupDistance)
                {
                    canPickup = true;

                    if (pickupPromptText != null)
                    {
                        pickupPromptText.gameObject.SetActive(true);
                        pickupPromptText.text = pickupPromptMessage;
                    }

                    if (Input.GetKeyDown(pickItemKey))
                    {
                        if (Time.time - lastPickupTime < pickupCooldown)
                            return;

                        ItemPickable pickableComponent = hitInfo.collider.GetComponent<ItemPickable>();
                        if (pickableComponent == null)
                            return;

                        itemType newItemType = pickableComponent.itemScriptableObject.item_type;

                        if (inventoryList.Count >= maxInventorySize)
                        {
                            // REMOVE CURRENT ITEM (NO DESTROY)
                            itemType removedType = inventoryList[selectedItem];

                            // Set inactive
                            itemSetActive[removedType].SetActive(false);

                            // Mark SO as not in inventory
                            ItemSO removedSO = GetItemSO(removedType);
                            if (removedSO != null)
                                removedSO.inInventory = false;

                            // Replace
                            inventoryList[selectedItem] = newItemType;
                        }
                        else
                        {
                            inventoryList.Add(newItemType);
                            selectedItem = inventoryList.Count - 1;
                        }

                        item.PickItem(); // remove world pickup

                        lastPickupTime = Time.time;

                        if (pickupPromptText != null)
                            pickupPromptText.gameObject.SetActive(false);

                        NewItemSelected();
                    }
                }
            }
        }

        if (!canPickup && pickupPromptText != null)
            pickupPromptText.gameObject.SetActive(false);
    }

    void HandleDrop()
    {
        if (Input.GetKeyDown(throwItemKey) && inventoryList.Count > 0)
        {
            itemType removedType = inventoryList[selectedItem];

            // Just disable it
            itemSetActive[removedType].SetActive(false);

            // Mark SO
            ItemSO removedSO = GetItemSO(removedType);
            if (removedSO != null)
                removedSO.inInventory = false;

            inventoryList.RemoveAt(selectedItem);

            if (selectedItem > 0)
                selectedItem--;

            if (inventoryList.Count > 0)
                NewItemSelected();
        }
    }

    void HandleEquip()
    {
        if (IsCurrentItemAnimating())
            return;

        for (int i = 0; i < inventoryList.Count && i < 10; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                selectedItem = i;
                NewItemSelected();
            }
        }
    }

    private bool IsCurrentItemAnimating()
    {
        if (inventoryList.Count == 0)
            return false;

        ItemSO currentItemSO = GetItemSO(inventoryList[selectedItem]);
        return currentItemSO != null && currentItemSO.isAnimating;
    }

    private ItemSO GetItemSO(itemType type)
    {
        switch (type)
        {
            case itemType.Atlas: return Atlas;
            case itemType.Falcon: return Falcon;
            case itemType.Gautlet: return Gautlet;
            case itemType.Johnson: return Johnson;
            case itemType.Raptor: return Raptor;
            case itemType.Spleefer: return Spleefer;
            case itemType.Sprinkler: return Sprinkler;
            case itemType.STAR: return STAR;
            case itemType.Sweeper: return Sweeper;
            case itemType.Vanguard: return Vanguard;
            default: return null;
        }
    }

    private void NewItemSelected()
    {
        if (inventoryList.Count == 0)
            return;

        foreach (var item in itemSetActive.Values)
            item.SetActive(false);

        itemSetActive[inventoryList[selectedItem]].SetActive(true);

        // Mark selected as in inventory
        ItemSO selectedSO = GetItemSO(inventoryList[selectedItem]);
        if (selectedSO != null)
            selectedSO.inInventory = true;
    }

    public void GetAnimated()
    {
        if (inventoryList.Count > 0)
        {
            itemSetActive[inventoryList[selectedItem]].SetActive(true);
        }
    }
}

public interface IPickable
{
    void PickItem();
}
