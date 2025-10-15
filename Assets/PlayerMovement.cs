using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public float speed = 6f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;
    public Transform cameraTransform; // assign your main camera in Inspector

    private CharacterController controller;
    private Vector3 velocity;
    private Animator animator;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Get input from keyboard
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        // Movement and rotation
        if (direction.magnitude >= 0.1f)
        {
            // Rotate player towards camera direction
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * speed * Time.deltaTime);

            // Trigger walking animation
            animator.SetFloat("Speed", 1f);
        }
        else
        {
            animator.SetFloat("Speed", 0f);
        }

        // Jumping
        if (controller.isGrounded)
        {
            velocity.y = -2f; // keep player grounded
            if (Input.GetButtonDown("Jump"))
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                animator.SetTrigger("Jump");
            }
        }
      
            if (Input.GetMouseButtonDown(0))
            {
                animator.SetTrigger("Stab");
            }
        

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // Dodge (optional)
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            animator.SetTrigger("Dodge");
        }
    }
}