using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Windows;

public class Player : MonoBehaviour
{
    [Header("References")]
    public Camera mainCam;
    [SerializeField] private PortalManager portalManager;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private ParticleSystem particlesA;
    [SerializeField] private ParticleSystem particlesB;

    [Header("Player Stats")]
    [SerializeField] private float speed;
    [SerializeField] private float jump;
    [SerializeField] private float dash;
    [SerializeField] private float dashCooldown;
    [SerializeField] private float portalCooldown;

    [Header("Player Physics")]
    [SerializeField] private float extraGravity;
    [SerializeField] private float airDrag;
    [SerializeField] private float mouseSense;


    //Camera Stuff
    private float rotationSmoothTime;
    private float horizontalLook;
    private float verticalLook;
    private float horizontalSmoothing;
    private float verticalSmoothing;
    private float xSmoothReference;
    private float ySmoothReference;

    //Player Stuff
    private Rigidbody rb;
    private float jumpRay;
    private bool canDash;
    private bool canShootPortal;
    private bool hasDoubleJump;

    // Awake is called when the script instance is being loaded
    void Awake()
    {
        AddInputs();
        rb = GetComponent<Rigidbody>();
        jumpRay = transform.localScale.y + 0.05f;
        rotationSmoothTime = 0.1f;
        horizontalSmoothing = horizontalLook;
        verticalSmoothing = verticalLook;
        canDash = true;
        canShootPortal = true;
    }

    // FixedUpdate is called every fixed framerate frame
    void FixedUpdate()
    {
        AuxiliaryMovement();
    }

    // Moves the player by a constant force with mass in a normalized direction given by wasd input
    private void Move(Vector2 direction)
    {
        Vector3 moveDirection = rb.rotation * new Vector3(direction.x, 0f, direction.y).normalized;
        rb.AddForce(speed * moveDirection, ForceMode.Impulse);
    }

    // Changes camera and player rotation from mouse movements
    private void Look(Vector2 lookInput)
    {
        horizontalLook += lookInput.x * mouseSense;
        verticalLook -= lookInput.y * mouseSense;
        verticalLook = (verticalLook + 180) % 360 - 180;
        verticalLook = Mathf.Clamp(verticalLook, -90f, 90f);
        horizontalSmoothing = Mathf.SmoothDampAngle(horizontalSmoothing, horizontalLook, ref xSmoothReference, rotationSmoothTime);
        verticalSmoothing = Mathf.SmoothDampAngle(verticalSmoothing, verticalLook, ref ySmoothReference, rotationSmoothTime);
        rb.MoveRotation(Quaternion.Lerp(rb.rotation, Quaternion.Euler(Vector3.up * horizontalLook), .6f));
        mainCam.transform.localEulerAngles = Vector3.right * verticalSmoothing;
    }

    // Moves the player by a constant force ignoring its mass upwards if grounded or if the player has a double jump
    private void Jump()
    {
        if (IsGrounded())
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(jump * Vector3.up, ForceMode.VelocityChange);
        }
        else if (hasDoubleJump)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(jump * Vector3.up, ForceMode.VelocityChange);
            hasDoubleJump = false;
        }
    }

    // Moves the player by a constant force ignoring its mass in a normalized direction given by wasd input
    private void Dash(Vector2 direction)
    {
        if (canDash)
        {
            Vector3 moveDirection = rb.rotation * new Vector3(direction.x, 0f, direction.y).normalized;
            rb.AddForce(dash * moveDirection, ForceMode.VelocityChange);
            rb.AddForce(dash / 10 * Vector3.up, ForceMode.VelocityChange);
            StartCoroutine(DashCooldown());
        }
    }

    // Function to start a timer for when a player is able to dash 
    private IEnumerator DashCooldown()
    {
        canDash = false;

        yield return new WaitForSeconds(dashCooldown);

        canDash = true;
    }

    // Extra physics calculations for player movement
    private void AuxiliaryMovement()
    {
        // Apply Drag
        rb.linearVelocity = new Vector3(rb.linearVelocity.x / (1 + airDrag), rb.linearVelocity.y, rb.linearVelocity.z / (1 + airDrag));

        // Apply Extra Gravity
        if (!IsGrounded())
        {
            rb.AddForce(extraGravity * -transform.up, ForceMode.Acceleration);
        }
    }

    // Boolean using raycast to determing whether or not the player is touching the ground
    private bool IsGrounded()
    {
        if (Physics.Raycast(transform.position, Vector3.down, jumpRay))
        {
            hasDoubleJump = true;
            return true;
        }
        return false;
    }

    // Adds the player input listeners from the InputManager script
    private void AddInputs()
    {
        inputManager.OnMove.AddListener(Move);
        inputManager.OnLook.AddListener(Look);
        inputManager.OnSpacePressed.AddListener(Jump);
        inputManager.OnShiftPressed.AddListener(Dash);
        inputManager.OnMouseLeftPressed.AddListener(ShootPortalA);
        inputManager.OnMouseRightPressed.AddListener(ShootPortalB);
    }

    // Shoots the A portal and calls the PortalManager script SpawnPortalA function if it hits a portal wall
    private void ShootPortalA()
    {
        if (Physics.Raycast(mainCam.transform.position, mainCam.transform.forward, out RaycastHit hit, Mathf.Infinity))
        {
            GameObject target = hit.collider.gameObject;
            if (target.CompareTag("PortalWall"))
            {
                if (canShootPortal)
                {
                    portalManager.SpawnPortalA(target.transform);
                    Instantiate(particlesA, target.transform);
                    StartCoroutine(PortalCooldown());
                }

            }
        }
    }

    // Shoots the B portal and calls the PortalManager script SpawnPortalB function if it hits a portal wall
    private void ShootPortalB()
    {
        if (Physics.Raycast(mainCam.transform.position, mainCam.transform.forward, out RaycastHit hit, Mathf.Infinity))
        {
            GameObject target = hit.collider.gameObject;
            if (target.CompareTag("PortalWall"))
            {
                if (canShootPortal)
                {
                    portalManager.SpawnPortalB(target.transform);
                    Instantiate(particlesB, target.transform);
                    StartCoroutine(PortalCooldown());
                }
            }
        }
    }
    // Function to start a timer for when a player is able to dash 
    private IEnumerator PortalCooldown()
    {
        canShootPortal = false;

        yield return new WaitForSeconds(portalCooldown);

        canShootPortal = true;
    }


    // Sets the player rotation from a given quaternion
    public void SetRotation(Quaternion newRotation)
    {
        Vector3 targetEuler = newRotation.eulerAngles;
        // Update look variables to prevent the Look() function from immediately overriding it
        horizontalLook = targetEuler.y;
        rb.rotation = Quaternion.Euler(0f, horizontalLook, 0f);

        verticalLook = targetEuler.x;
        mainCam.transform.localRotation = Quaternion.Euler(verticalLook, 0f, 0f);
    }

    // Sets the player velocity in the forward direction
    public void SetVelocity()
    {
        rb.linearVelocity = Vector3.zero;
        rb.AddForce(speed * transform.forward, ForceMode.Impulse);
    }
}