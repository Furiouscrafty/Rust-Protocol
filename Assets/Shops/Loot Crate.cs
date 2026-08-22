using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LootCrate : MonoBehaviour
{

    //PUBLIC CONFIG
    [Header("Weapon Lists")]
    public List<GameObject> displayWeapons = new List<GameObject>(); // Visual-only weapons
    public List<GameObject> actualWeapons = new List<GameObject>(); // Real pickup weapons

    [Header("Crate Models")]
    public GameObject crateModelClosed;
    public GameObject crateModelOpen;
    public Transform weaponDisplayPoint;

    [Header("Interaction")]
    public KeyCode interactKey = KeyCode.E;
    public float interactRange = 3f;
    public string playerTag = "Player";
    public TMP_Text interactionPrompt;

    [Header("Settings")]
    public int baseCost = 200;
    public float costIncreasePerRound = 0.1f;
    public RoundSettings roundSettings;
    public float cycleDuration = 5f;
    public float cycleInterval = 0.1f;
    public float cooldownTime = 3f;
    public float weaponDespawnTime = 8f;
    public Vector3 spawnOffset = new Vector3(0f, 0.5f, 0f);

    // PRIVATE STATE
    private enum CrateState
    {
        Closed,
        Cycling,
        AwaitingPickup,
        Cooldown
    }

    private CrateState _state = CrateState.Closed;
    private Transform _player;
    private Camera _mainCamera;
    private GameObject _currentDisplayInstance; // Fake model
    private GameObject _spawnedActualWeapon; // Real weapon prefab
    private int _chosenIndex = -1;
    private Coroutine _despawnRoutine;

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObj != null)
            _player = playerObj.transform;

        _mainCamera = Camera.main;
        SetCrateModel(false);

        if (interactionPrompt != null)
            interactionPrompt.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (_player == null) return;

        float distance = Vector3.Distance(transform.position, _player.position);
        bool inRange = distance <= interactRange;

        HandlePrompt(inRange);

        //Handle interaction input
        if (inRange && Input.GetKeyDown(interactKey))
        {
            if (_state == CrateState.Closed)
                AttemptPurchase();
            else if (_state == CrateState.AwaitingPickup)
                OnWeaponPickedUp();
        }
    }

    private void LateUpdate()
    {
        if (interactionPrompt != null && interactionPrompt.gameObject.activeSelf && _mainCamera != null)
        {
            interactionPrompt.transform.LookAt(_mainCamera.transform);
            interactionPrompt.transform.Rotate(0, 180, 0);
        }
    }

    private int GetCurrentCost()
    {
        if (roundSettings == null) return baseCost;

        float multiplier = 1f + (roundSettings.CurrentRound * costIncreasePerRound);
        return Mathf.RoundToInt(baseCost * multiplier);
    }

    private void AttemptPurchase()
    {
        if (roundSettings == null)
        {
            Debug.LogError("RoundSettings not assigned.");
            return;
        }

        int cost = GetCurrentCost();

        if (roundSettings.money < cost)
        {
            Debug.Log($"Not enough money. Need ${cost}, have ${roundSettings.money}");
            return;
        }

        roundSettings.money -= cost;
        StartCoroutine(CycleWeapons());
    }

    private IEnumerator CycleWeapons()
    {
        _state = CrateState.Cycling;
        SetCrateModel(true);

        float elapsed = 0f;
        int index = 0;

        while (elapsed < cycleDuration)
        {
            ShowDisplayWeapon(index);
            yield return new WaitForSeconds(cycleInterval);
            elapsed += cycleInterval;
            index = (index + 1) % displayWeapons.Count;
        }

        _chosenIndex = Random.Range(0, displayWeapons.Count);
        ShowDisplayWeapon(_chosenIndex);
        SpawnActualWeapon(_chosenIndex);

        _state = CrateState.AwaitingPickup;
        _despawnRoutine = StartCoroutine(DespawnWeaponTimer());
    }

    private void ShowDisplayWeapon(int index)
    {
        if (_currentDisplayInstance != null)
            Destroy(_currentDisplayInstance);

        _currentDisplayInstance = Instantiate(
            displayWeapons[index],
            weaponDisplayPoint.position,
            Quaternion.identity,
            weaponDisplayPoint
        );

        foreach (Collider col in _currentDisplayInstance.GetComponentsInChildren<Collider>())
            col.enabled = false;
        foreach (Rigidbody rb in _currentDisplayInstance.GetComponentsInChildren<Rigidbody>())
            rb.isKinematic = true;
        foreach (MonoBehaviour mb in _currentDisplayInstance.GetComponentsInChildren<MonoBehaviour>())
            mb.enabled = false;
    }

    
    private void SpawnActualWeapon(int index) // Spawns real usable weapon prefab
    {
        Vector3 spawnPos = weaponDisplayPoint.position + spawnOffset;
        _spawnedActualWeapon = Instantiate(actualWeapons[index], spawnPos, Quaternion.identity);
    }

    private IEnumerator DespawnWeaponTimer()
    {
        yield return new WaitForSeconds(weaponDespawnTime);

        if (_state == CrateState.AwaitingPickup)
        {
            if (_spawnedActualWeapon != null)
                Destroy(_spawnedActualWeapon);

            CloseCrate();
        }
    }

    private void OnWeaponPickedUp()
    {
        if (_state != CrateState.AwaitingPickup) return;

        if (_despawnRoutine != null)
            StopCoroutine(_despawnRoutine);

        if (_currentDisplayInstance != null)
            Destroy(_currentDisplayInstance);

        if (_spawnedActualWeapon != null)
            Destroy(_spawnedActualWeapon);

        StartCoroutine(Cooldown());
    }

    private IEnumerator Cooldown()
    {
        _state = CrateState.Cooldown;
        SetCrateModel(false);
        yield return new WaitForSeconds(cooldownTime);
        _state = CrateState.Closed;
    }

    private void CloseCrate()
    {
        if (_currentDisplayInstance != null)
            Destroy(_currentDisplayInstance);

        _state = CrateState.Closed;
        SetCrateModel(false);
    }

    private void SetCrateModel(bool open)
    {
        if (crateModelClosed != null)
            crateModelClosed.SetActive(!open);
        if (crateModelOpen != null)
            crateModelOpen.SetActive(open);
    }

    //UI PROMPT
    private void HandlePrompt(bool inRange)
    {
        if (interactionPrompt == null) return;

        bool show = inRange && (_state == CrateState.Closed || _state == CrateState.AwaitingPickup);
        interactionPrompt.gameObject.SetActive(show);

        if (!show) return;

        if (_state == CrateState.Closed)
        {
            int cost = GetCurrentCost();
            interactionPrompt.text = roundSettings != null && roundSettings.money >= cost
                ? $"Press E to Open Crate (${cost})"
                : $"Not Enough Money (${cost})";
        }
        else
        {
            interactionPrompt.text = "Press E to Take Weapon";
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}
