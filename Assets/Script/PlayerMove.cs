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
    private float LaneDistance = 4f;// ���� �Ÿ�
    [SerializeField]
    private float laneSwitchSpeed = 30f;
    [SerializeField]
    private float jumpForce = 20;
    [SerializeField]
    private bool isJumping = false;
    [SerializeField]
    private bool isSliding = false;
    [SerializeField]
    private int currentLane = 1;//0 ����, 1 ���, 2 ������
    [SerializeField]
    private bool isGrounded;

    private Vector3 _targetPosition;
    private bool _isMoving = false;
    private Rigidbody _rigidbody;


    private void Awake()
    {
        currentLane = 1;
        isJumping = false;
        isSliding = false;
        _rigidbody = GetComponent<Rigidbody>();
        Physics.gravity = new Vector3(0, -20f, 0);
    }


    // Update is called once per frame

    private void Update()
    {
        CheckGrounded();
    
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
        if (context.started && !_isMoving &&!isJumping&&!isSliding&&isGrounded)
        {
            _rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
           isJumping=true;
        }
        isJumping = false;
    }

    public void Sliding(InputAction.CallbackContext context)
    {
        if (context.started && !_isMoving && !isJumping && !isSliding&&isGrounded)
        {

            isSliding = true;
        }
        isSliding = false;
    }

    public void MoveDeley(InputAction.CallbackContext context)
    {
        if (context.started && !_isMoving&&context.canceled)
        {
            _isMoving = false; // ��ư�� ������ �̵� ���� �ʱ�ȭ
        }
    }
    private void CheckGrounded()
    {
        if (transform.position.y <=1 &&isJumping == false)
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Hurdle")
        {
            Debug.Log("Game Over");
        }
    }
}