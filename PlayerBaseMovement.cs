using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerBaseMovement : MonoBehaviour
{
    Vector3 moveDirection;
    [SerializeField] float moveSpeed = 1f;
    float moveCap = 5f;

    private float horizontalInput;
    private float verticalInput;

    [SerializeField] public GameObject playerOrientation;
    public Rigidbody rb;

    [SerializeField] private bool isGrounded;

    [SerializeField] float jumpForce;
     private bool isJumping;

    RaycastHit GroundNormal;

    //public RaycastHit groundNormalHit;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        GroundCheck();

        if(isGrounded) 
        {
            //rb.AddForce(Vector3.down * 5, ForceMode.Force);


            if (Input.GetKeyDown(KeyCode.Space) && !isJumping)
            {
                isJumping = true;
                Jump(1);
            }
        }

        if(Input.GetKeyUp(KeyCode.Space))
        {
            isJumping = false;
        }

        moveDirection = playerOrientation.transform.forward * verticalInput + playerOrientation.transform.right * horizontalInput;
    }

    private void FixedUpdate()
    {
        Move(0);
    }

    void Move(float modifier)
    {
        rb.velocity = new Vector3(moveDirection.x * moveSpeed, rb.velocity.y, moveDirection.z * moveSpeed);

        Vector3 xzVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        Vector3 yVelocity = new Vector3(0, rb.velocity.y, 0);

        if (rb.velocity.magnitude > moveCap)
        {
            xzVelocity = Vector3.ClampMagnitude(xzVelocity, moveCap);

            rb.velocity = Vector3.Lerp(rb.velocity, xzVelocity + yVelocity, Time.fixedDeltaTime * 10);

            Debug.Log("Capping");
        }

        if (horizontalInput + verticalInput == 0 && isGrounded)
        {
            rb.drag = 10f;
        }
        else
        {
            rb.drag = 1f;
        }
    }

    void GroundCheck()
    {
        isGrounded = Physics.CheckSphere(playerOrientation.transform.position, 1.1f, LayerMask.GetMask("Jumpable"));
        
        if(isGrounded)
        {
            isJumping = false;
            if (Physics.Raycast(new Vector3(playerOrientation.transform.position.x, playerOrientation.transform.position.y + 1, playerOrientation.transform.position.z), Vector3.down, out GroundNormal, 0.1f, LayerMask.GetMask("Jumpable")))
            {
                playerOrientation.transform.rotation = new Quaternion(GroundNormal.normal.x, playerOrientation.transform.rotation.y, GroundNormal.normal.z, 0);
            }
        }

        Debug.Log($"Is Grounded?: {isGrounded}");
    }

    void Jump(int Power)
    {
        isGrounded = false;
        rb.AddForce(playerOrientation.transform.up * Power * jumpForce, ForceMode.Impulse);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(playerOrientation.transform.position, 1.1f);
    }

}
