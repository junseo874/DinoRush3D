using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements.Experimental;

public class PlayerMove : MonoBehaviour
{


    [Header("Movement Settings")]
    [SerializeField]
<<<<<<< Updated upstream
    private float LaneDistance = 4f;// ·¹ÀÎ °Å¸®
=======
    private float LaneDistance = 4f;// ë ˆì¸ ê±°ë¦¬
>>>>>>> Stashed changes
    [SerializeField]
    private float laneSwitchSpeed = 30f;
    [SerializeField]
    private float jumpForce = 20;
    [SerializeField]
    private bool isJumping = false;
    [SerializeField]
    private bool isSliding = false;
    [SerializeField]
<<<<<<< Updated upstream
    private int currentLane = 1;//0 ¿ÞÂÊ, 1 °¡¿îµ¥, 2 ¿À¸¥ÂÊ
=======
    private int currentLane = 1;//0 ì™¼ìª½, 1 ê°€ìš´ë°, 2 ì˜¤ë¥¸ìª½
>>>>>>> Stashed changes
    [SerializeField]
    private bool isGrounded;

    private PlayerInput _playerInput;
    private InputAction _slidingAction;
    private Vector3 _targetPosition;
    private bool _isMoving = false;
    private Rigidbody _rigidbody;
    private Animator _playerAnimator;
    private bool _playerDeath = false;

    private void Awake()
    {
        currentLane = 1;
        isJumping = false;
        isSliding = false;
        _rigidbody = GetComponent<Rigidbody>();
        Physics.gravity = new Vector3(0, -20f, 0);
        _playerAnimator = GetComponent<Animator>();
        _playerInput = GetComponent<PlayerInput>();
        _slidingAction = _playerInput.actions.FindAction("Slide", true);
    }


    // Update is called once per frame

    private void Update()
    {


    }

    private void FixedUpdate()
    {
        _targetPosition = new Vector3((currentLane - 1) * LaneDistance, transform.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, _targetPosition, Time.deltaTime * laneSwitchSpeed);

    }
    public void MoveLeft(InputAction.CallbackContext context)
    {

        if (context.started && !_isMoving && currentLane > 0)
        {
            currentLane--;
        }
    }
    public void MoveRIght(InputAction.CallbackContext context)
    {

        if (context.started && !_isMoving && currentLane < 2)
        {
            currentLane++;
        }
    }

    public void Jump(InputAction.CallbackContext context)
    {

        if (context.started && !_isMoving && !isJumping && !isSliding && isGrounded)
        {
            _rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isJumping = true;
            isGrounded = false;
            _playerAnimator.SetTrigger("Jump");
            _playerAnimator.SetBool("Run", false);
        }
    }

    public void Sliding(InputAction.CallbackContext context)
    {
        if (context.performed && !_isMoving && !isJumping && !isSliding && isGrounded)
        {
            isSliding = true;
            _playerAnimator.SetTrigger("Slide");
            _playerAnimator.SetBool("Run", false);
        }
        else if (context.canceled)
        {
            isSliding = false;
            _playerAnimator.SetBool("Run", true);
        }
    }

    public void MoveDeley(InputAction.CallbackContext context)
    {
        if (context.started && !_isMoving && context.canceled)
        {
<<<<<<< Updated upstream
<<<<<<< Updated upstream
            _isMoving = false; // ¹öÆ°À» ³õÀ¸¸é ÀÌµ¿ »óÅÂ ÃÊ±âÈ­
=======
            _isMoving = false; 
>>>>>>> Stashed changes
=======
            _isMoving = false; // ë²„íŠ¼ì„ ë†“ìœ¼ë©´ ì´ë™ ìƒíƒœ ì´ˆê¸°í™”
>>>>>>> Stashed changes
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Hurdle")
        {
            _playerAnimator.SetTrigger("Death");
            _playerAnimator.SetBool("Run", false);

            Debug.Log("Game Over");
            _playerDeath = true;
        }

        if (collision.gameObject.tag == "Ground")
        {
            isGrounded = true;
            isJumping = false;
            _playerAnimator.SetBool("Run", true);

        }

    }

}