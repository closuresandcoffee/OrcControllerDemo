using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 10f;
    [SerializeField] private float jumpForce = 3f;
    [SerializeField] private float mouseSensitivity = 120f;
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private GameObject mainMenu;

    private float horizontalInput;
    private float forwardInput;
    private Vector3 moveDirection;
    private bool isGrounded = true;
    private Rigidbody rbody;
    private Animator anim;

    private const float ANIMATION_DAMP_TIME = 0.1f;

    private void Start()
    {
        rbody = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        // If Paused
        if (Time.timeScale == 0 || mainMenu.activeSelf)
        {
            return;
        }

        // Check for game over
        if (transform.position.y < -30)
        {
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.Confined;
            gameOverScreen.SetActive(true);
        }

        forwardInput = Input.GetAxis("Vertical");
        moveDirection = new Vector3(0, 0, forwardInput);

        if (moveDirection == Vector3.zero)
        {
            Idle();
        }
        else if (moveDirection != Vector3.zero && Input.GetKey(KeyCode.LeftShift))
        {
            Run();
        }
        else if (moveDirection != Vector3.zero && !Input.GetKey(KeyCode.LeftShift))
        {
            Walk();
        }

        // Handle Rotation
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        transform.Rotate(Vector3.up, mouseX, Space.World);

        // Handle Jump
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }

        if (Input.GetMouseButtonDown(0))
        {
            Punch();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isGrounded = true;
        }
    }

    private void Idle()
    {
        anim.SetFloat("Speed", 0, ANIMATION_DAMP_TIME, Time.deltaTime);
    }

    private void Run()
    {
        translateMovement(runSpeed);
        anim.SetFloat("Speed", 1, ANIMATION_DAMP_TIME, Time.deltaTime);
    }

    private void Walk()
    {
        translateMovement(walkSpeed);
        anim.SetFloat("Speed", 0.5f, ANIMATION_DAMP_TIME, Time.deltaTime);
    }

    private void translateMovement(float moveSpeed)
    {
        horizontalInput = Input.GetAxis("Horizontal");
        forwardInput = Input.GetAxis("Vertical");

        transform.Translate(Vector3.forward * Time.deltaTime * moveSpeed * forwardInput);
        transform.Translate(Vector3.right * Time.deltaTime * moveSpeed * horizontalInput);
    }

    private void Jump()
    {
        rbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isGrounded = false;
        anim.SetTrigger("Jump");
    }

    private void Punch()
    {
        anim.SetTrigger("Punch");
    }
}
