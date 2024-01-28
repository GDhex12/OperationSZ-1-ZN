using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterMovement : MonoBehaviour
{
    Controls playerInput;
    Vector2 movement_dir;
    bool run;
    bool jump;
    bool onGround;
    Rigidbody rb;
    [SerializeField]
    private float speed=1;
    [SerializeField]
    private float runSpeed=2;
    [SerializeField]
    private float drag = 2;
    [SerializeField]
    private float maxWalkSpeed = 5;
    [SerializeField]
    private float maxRunSpeed = 10;
    [SerializeField]
    private float jumpHeight = 2;
    [SerializeField]
    private Transform groundCheck;
    [SerializeField]
    private float gravity;

    [SerializeField] private bool pickupTriggered = false;
    [SerializeField] private bool holdingItem = false;
    [SerializeField] private GameObject pickupItem = null;
    [SerializeField] private Transform itemLocation;

    Vector3 movement_rel;

    // Start is called before the first frame update
    void Start()
    {
        playerInput = new Controls();
        playerInput.OnFoot.Enable();
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        movement_dir = playerInput.OnFoot.Movement.ReadValue<Vector2>();
        movement_rel = (transform.forward * movement_dir.y + transform.right * movement_dir.x).normalized;
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.performed)
            run = !run;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
            jump = true;
    }

    public void OnPickUp(InputAction.CallbackContext context)
    {
        if (context.performed && pickupTriggered)
        {
            if (pickupItem.TryGetComponent(out Weaponscontrol wc))
            {
                wc.activate = true;
            }
            if (pickupItem.TryGetComponent(out Bullet1 bu))
            {
                bu.activate = true;
            }
            pickupItem.GetComponent<Collider>().enabled = false;
            pickupItem.transform.parent = itemLocation;

            pickupItem.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

            // I thought up this tasteful hardcoded value to make this game more funny
            pickupItem.transform.localScale = new Vector3(0.7f, 30f, pickupItem.transform.localScale.z);

            pickupTriggered = false;
            holdingItem = true;
        }
    }

    public void OnItemThrow(InputAction.CallbackContext context)
    {
        if (context.performed && holdingItem)
        {
            pickupItem.GetComponent<Item>().ThrowItem();
            holdingItem = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            pickupItem = other.gameObject;
            pickupTriggered = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        pickupTriggered = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (movement_dir.magnitude != 0 && onGround)
        {
            if (!run)
                rb.AddForce(movement_rel * (maxWalkSpeed - rb.velocity.magnitude) * speed) ;
            else if(run)
            rb.AddForce(movement_rel * (maxRunSpeed - rb.velocity.magnitude) * runSpeed);

            rb.velocity = new Vector3(movement_rel.x,rb.velocity.normalized.y,movement_rel.z) * rb.velocity.magnitude;
        }   
        else if(movement_dir.magnitude == 0 && onGround)
        {
            rb.AddForce(-rb.velocity * drag);
        }

        
        if(Physics.Raycast(groundCheck.position,-transform.up,0.2f,~LayerMask.GetMask("Player")))
        {
            onGround = true;
        }
        else
        {
            onGround = false;
            jump = false;
        }

        if(!onGround)
        rb.AddForce(transform.up * -gravity*2);

        if (jump && onGround)
        {
            float jumpForce = Mathf.Sqrt(jumpHeight * 2 * gravity);
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }

       
        
        
    }
}
