using UnityEngine;
[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Objects/Items")]
public class ItemSO : ScriptableObject
{
    [Header("Properties")]
    public itemType item_type;
    public bool isEquipped;
    public bool inInventory;
    public GunType Type;
    public bool isAnimating;
    public Sprite item_sprite;

    //Reset Data
    public int DefaultMag;
    public int DefaultTotalMag;
    public bool WasInInventory;
}

public enum itemType {Atlas,  Gautlet, Johnson, Spleefer, Raptor, STAR, Vanguard, Sweeper, Falcon, Sprinkler}

public enum GunType {Automatic,  Shotgun, Semi_Automatic, Burst}
