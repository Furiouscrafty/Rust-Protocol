using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public Camera playerCamera;
    public float walkSpeed = 10f;
    public float jumpPower = 7f;
    public float gravity = 10f;

    [Header("Look Settings")]
    public float lookSpeed = 2f; // Mouse sensitivity
    public float lookXLimit = 45f; // How far up & down the player can look

    [Header("Height Settings")]
    public float defaultHeight = 2f;
    public float cameraHeightOffset = 1f; // Offset from top of character controller

    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;
    private CharacterController characterController;
    public bool canMove = true;

    // Health
    public float InitialHealth = 100f;
    public float CurrentHealth;

    //Temporary Points
    public float Score;

    //Dead Screen
    public GameObject DeathMenu;
    public PlayerMovement playerMovement;
    public GameObject hudUI;


    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        DeathMenu.SetActive(false);

        // Set character controller height
        characterController.height = defaultHeight;

        // Set camera position
        Vector3 cameraPos = playerCamera.transform.localPosition;
        cameraPos.y = defaultHeight - cameraHeightOffset;
        playerCamera.transform.localPosition = cameraPos;

        // Adjust character controller center
        Vector3 center = characterController.center;
        center.y = characterController.height / 2f;
        characterController.center = center;

        CurrentHealth = InitialHealth;
    }

    void Update()
    {

        if (CurrentHealth < 0)
            CurrentHealth = 0;
        else if (CurrentHealth > InitialHealth)
            CurrentHealth = InitialHealth;
        // Checking Health
        if (CurrentHealth <= 0)
        {
            Time.timeScale = 0f;
            if (playerMovement != null)
                playerMovement.canMove = false;

            // UI
            if (DeathMenu != null)
                DeathMenu.SetActive(true);

            if (hudUI != null)
                hudUI.SetActive(false);

            // Free mouse
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

        }
        else
        {
            HandleMovement();
            HandleMouseLook();
        }

    }

    private void HandleMovement()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        float curSpeedX = canMove ? walkSpeed * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? walkSpeed * Input.GetAxis("Horizontal") : 0;

        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpPower;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime * 2;
        }


        characterController.Move(moveDirection * Time.deltaTime);
    }

    private void HandleMouseLook()
    {
        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
    }

    public void Add_Health(float AddH)
    {
        if (CurrentHealth >= InitialHealth)
            print("No health added");
        else
        {
            CurrentHealth = CurrentHealth + AddH;
            print("Health is");
            print(CurrentHealth);
        }
    }

    public void Remove_Health(float RemoveH)
    {
        CurrentHealth = CurrentHealth - RemoveH;
        print("Health is");
        print(CurrentHealth);
    }

    public float GetHealth()
    {
        return CurrentHealth;
    }

    public void Add_Score(float Add)
    { 
        Score = Score + Add;
        print("Score is");
        print(Score);
    }

    public float Get_Score()
    { 
        return Score;
    }

}