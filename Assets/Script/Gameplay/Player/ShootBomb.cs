using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ShootBomb : MonoBehaviour
{
    [System.Serializable]
    public struct GrenadeData
    {
        public int idItem;         // Item ID for the grenade
        public GameObject prefab;  // Grenade prefab associated with the ID
    }

    [Header("Bomb Configuration")]
    public List<GrenadeData> grenadePrefabs; // List to store prefab and corresponding IDs
    public List<GameObject> listSpawnedBombs; // List of currently spawned bombs

    [Header("Dispense Configuration")]
    public Vector2 groundDispenseVelocity;
    public Vector2 verticalDispenseVelocity;

    [Header("Transforms")]
    public Transform trnsGun;
    public Transform trnsGunTip;

    [Header("Charge Timing")]
    public float maxChargeTime = 2f;
    public float minimumChargeTime = 1f;
    public float currentChargeTime = 0f;

    [Header("References")]
    public UIInventory uiInventory;
    private GameObject currentGrenadePrefab; // Grenade prefab to use based on item ID
    private ItemData currentGrenadeItem;     // Reference to the current grenade item in the inventory
    public bool hasPressedG = false;

    private void Awake()
    {
        uiInventory = FindAnyObjectByType<UIInventory>();
        UpdateGrenadeType();
    }

    private void Update()
    {
        UpdateGrenadeType();

        if (currentGrenadePrefab == null) return;

        if (Input.GetKeyDown(KeyCode.G) && !hasPressedG)
        {
            Debug.Log("G Key Pressed");
            hasPressedG = true;
        }

        if (Input.GetKey(KeyCode.G))
        {
            currentChargeTime += Time.deltaTime;
            currentChargeTime = Mathf.Min(currentChargeTime, maxChargeTime);
        }

        if (Input.GetKeyUp(KeyCode.G) && hasPressedG)
        {
            if (currentChargeTime < minimumChargeTime)
            {
                LaunchBomb(groundDispenseVelocity * minimumChargeTime, verticalDispenseVelocity * minimumChargeTime);
            }
            else if (currentChargeTime < maxChargeTime)
            {
                LaunchBomb(groundDispenseVelocity * currentChargeTime, verticalDispenseVelocity * currentChargeTime);
            }
            else
            {
                LaunchBomb(groundDispenseVelocity * maxChargeTime, verticalDispenseVelocity * maxChargeTime);
            }

            hasPressedG = false;
            currentChargeTime = 0;
        }
    }

    private void UpdateGrenadeType()
    {
        // Find the equipped grenade in the inventory
        currentGrenadeItem = uiInventory.listItemDataInventoryEquipment
            .FirstOrDefault(itemData => itemData.itemtype == Itemtype.Grenade);

        if (currentGrenadeItem != null)
        {
            currentGrenadePrefab = grenadePrefabs
                .FirstOrDefault(grenadeData => grenadeData.idItem == currentGrenadeItem.idItem).prefab;
        }
        else
        {
            currentGrenadePrefab = null;
        }
    }

    private void LaunchBomb(Vector2 ground, Vector2 vertical)
    {
        if (currentGrenadePrefab == null || currentGrenadeItem == null) return;

        GameObject instantiatedBomb = SpawnNewBomb();
        if (GetComponent<SpriteRenderer>().flipX)
        {
            instantiatedBomb.GetComponent<FakeHeightObject>().Initialize(
                -trnsGun.right * Random.Range(ground.x, ground.y),
                Random.Range(vertical.x, vertical.y));
        }
        else
        {
            instantiatedBomb.GetComponent<FakeHeightObject>().Initialize(
                trnsGun.right * Random.Range(ground.x, ground.y),
                Random.Range(vertical.x, vertical.y));
        }

        // Remove the grenade from the inventory
        RemoveGrenadeFromInventory();
    }

    private GameObject SpawnNewBomb()
    {
        GameObject instantiatedBomb = Instantiate(currentGrenadePrefab, trnsGunTip.position, Quaternion.identity);
        instantiatedBomb.GetComponent<FakeHeightObject>().shootBomb = this;
        listSpawnedBombs.Add(instantiatedBomb);
        Debug.Log("Bomb Spawned");
        return instantiatedBomb;
    }

    private void RemoveGrenadeFromInventory()
    {
        currentGrenadeItem.count--;

        if (currentGrenadeItem.count <= 0)
        {
            // Remove the item from the inventory if count reaches 0
            uiInventory.listItemDataInventoryEquipment.Remove(currentGrenadeItem);
            Debug.Log("Grenade item removed from inventory.");
        }

        // Optionally refresh the inventory UI
        uiInventory.RefreshUIInventory();
    }

    public void DestroyBomb(GameObject bombToDestroy)
    {
        if (bombToDestroy == null) return;

        listSpawnedBombs.Remove(bombToDestroy);
        Destroy(bombToDestroy);
    }
}
