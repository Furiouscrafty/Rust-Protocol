using UnityEngine;

public class ItemPickable : MonoBehaviour, IPickable
{
    public ItemSO itemScriptableObject;

    public void PickItem()
    { 
        Destroy(gameObject);
    }
}
