using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public float InputX, InputZ;
    public float Speed;
    public Vector3 desiredMoveDirection;
    public float desiredRotationSpeed;
    public Animator animator;
    public Camera camera;
    public bool isGrounded = false;
    public CharacterController controller;
    public float Angle2Target;
    public GameObject InputDirectionCompass;
    private float verticalVelocity;
    private Vector3 moveVector;
    private bool isInAir = false;
    private bool isPistolArmed = false;
    private bool isRifleArmed = false;

    public GameObject pistolOBJ;
    public GameObject rifleOBJ;

    // Start is called before the first frame update
    void Start()
    {
        animator = this.GetComponent<Animator>();
        controller = this.GetComponent<CharacterController>();
        camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        InputMagnitude();
    }

    void FixedUpdate()
    {
        // Parent the Input Direction Compass to the Player
        InputDirectionCompass.transform.position = this.transform.position;
        InputDirectionCompass.transform.rotation = Quaternion.identity;

        InputX = Input.GetAxis("Horizontal");
        InputZ = Input.GetAxis("Vertical");
        animator.SetFloat("InputZ", InputZ, 0f, Time.deltaTime);
        animator.SetFloat("InputX", InputX, 0f, Time.deltaTime);
        Speed = new Vector2(InputX, InputZ).sqrMagnitude;
        animator.SetFloat("InputMagnitude", Speed, 0f, Time.deltaTime);

        Vector3 targetDirection = InputDirectionCompass.transform.InverseTransformPoint(desiredMoveDirection);
        Angle2Target = Mathf.Atan2(targetDirection.x, targetDirection.z) * Mathf.Rad2Deg;

        var cam = Camera.main;
        var forward = cam.transform.forward;
        var right = cam.transform.right;
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();
        desiredMoveDirection = forward * InputZ + right * InputX;
        desiredMoveDirection.y = 0f;

        InputDirectionCompass.transform.Rotate(0, 0, Angle2Target);
        InputDirectionCompass.transform.Translate(desiredMoveDirection);

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredMoveDirection), desiredRotationSpeed * Time.deltaTime);

        controller.Move(desiredMoveDirection * Time.deltaTime);
    }

    void InputMagnitude() {
        bool running = Input.GetButton("Running");
        animator.SetBool("isRunning", running);
        if (running && Input.GetButtonDown("Sliding"))
            animator.SetBool("isSliding", true);
        else
            animator.SetBool("isSliding", false);

        bool crouching = Input.GetButton("Crouching");
        animator.SetBool("isCrouching", crouching);

        bool punching = Input.GetButton("Fire1");
        animator.SetBool("isPunching", punching);

        isGrounded = controller.isGrounded;
        animator.SetBool("isGrounded", controller.isGrounded);

        if (controller.isGrounded && !isInAir)
        {
            bool jumping = Input.GetButton("Jump");
            if (jumping)
            {
                animator.SetBool("isJumping", jumping);
                isInAir = true;
                jumping = false;
            }
        }
        else
            isInAir &= !controller.isGrounded;

        if (Input.GetButton("ArmPistol"))
        {
            isPistolArmed = !isPistolArmed;
            animator.SetBool("isPistolArmed", isPistolArmed);
            pistolOBJ.SetActive(true);
            isRifleArmed = false;
            animator.SetBool("isRifleArmed", isRifleArmed);
            rifleOBJ.SetActive(false);
        }

        if (Input.GetButton("ArmRifle"))
        {
            isRifleArmed = !isRifleArmed;
            animator.SetBool("isRifleArmed", isRifleArmed);
            isPistolArmed = false;
            animator.SetBool("isPistolArmed", isPistolArmed);
            pistolOBJ.SetActive(false);
        }

        if (Input.GetMouseButton(1))
            animator.SetBool("isAiming", true);
        else
            animator.SetBool("isAiming", false);
    }

    public void GrabRifle()
    {
        rifleOBJ.SetActive(true);
    }

    public void PutRifleAway()
    {
        rifleOBJ.SetActive(false);
    }

    public void PutPistolAway()
    {
        pistolOBJ.SetActive(false);
    }
}
