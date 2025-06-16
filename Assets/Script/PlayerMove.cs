// ============================================================================
// PlayerMove.cs - 플레이어 이동, 점프, 슬라이드, 사망 애니메이션 컨트롤
// 목적: PlayerInput 시스템을 기반으로 Lane 이동, 점프, 슬라이드, 부스트 등을 제어
// 작성자: 최경빈
// 작성일: 2025년 06월 01일
// 구조: Rigidbody + Animator 기반, 입력 액션마다 상태값 갱신 및 애니메이션 전환
// ============================================================================

using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerMove : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float LaneDistance = 4f;         // 레인 간 거리
    [SerializeField] private float laneSwitchSpeed = 30f;     // 레인 전환 속도
    [SerializeField] private float jumpForce = 20f;           // 점프 힘
    [SerializeField] private bool isJumping = false;          // 점프 중 여부
    [SerializeField] private bool isSliding = false;          // 슬라이딩 중 여부
    [SerializeField] private int currentLane = 1;             // 현재 레인 (0: 좌, 1: 중간, 2: 우)
    [SerializeField] private bool isGrounded;                 // 지면에 붙어있는지 여부
    [SerializeField] private float gravityVal = -15f;         // 중력 설정 값

    private PlayerInput _playerInput;                         // 입력 시스템
    private Vector3 _targetPosition;                          // 목표 위치
    private bool _isMoving = false;                           // 이동 중 여부
    private Rigidbody _rigidbody;                             // 물리 제어
    private Animator _playerAnimator;                         // 애니메이션 제어
    private bool _playerDeath = false;                        // 사망 여부

    private void Awake()
    {
        currentLane = 1;
        isJumping = false;
        isSliding = false;
        _rigidbody = GetComponent<Rigidbody>();
        Physics.gravity = new Vector3(0, gravityVal, 0); // 사용자 정의 중력 설정
        _playerAnimator = GetComponent<Animator>();
        _playerInput = GetComponent<PlayerInput>();
    }

    private void Update()
    {
        // 현재는 비어 있음 (입력이나 상태 변화 없이 이동은 FixedUpdate에서 처리)
    }

    private void FixedUpdate()
    {
        // 목표 레인 위치로 부드럽게 이동
        _targetPosition = new Vector3((currentLane - 1) * LaneDistance, transform.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, _targetPosition, Time.deltaTime * laneSwitchSpeed);
    }

    // 왼쪽 이동 입력
    public void MoveLeft(InputAction.CallbackContext context)
    {
        if (context.started && !_isMoving && currentLane > 0)
        {
            currentLane--;
        }
    }

    // 오른쪽 이동 입력
    public void MoveRIght(InputAction.CallbackContext context)
    {
        if (context.started && !_isMoving && currentLane < 2)
        {
            currentLane++;
        }
    }

    // 점프 입력 처리
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

    // 슬라이딩 입력 처리
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
        }
    }

    // 애니메이션 이벤트: 사망 애니메이션 중 호출
    public void Death()
    {
        Debug.Log("Death animation event triggered");
    }

    // 속도 부스트 시작
    public void ActivateSpeedBoost(float duration)
    {
        StartCoroutine(SpeedBoostCoroutine(duration));
    }

    // 일정 시간 동안 이동 속도 증가
    private IEnumerator SpeedBoostCoroutine(float duration)
    {
        float originalSpeed = laneSwitchSpeed;
        laneSwitchSpeed *= 2f;
        yield return new WaitForSeconds(duration);
        laneSwitchSpeed = originalSpeed;
    }

    // 사용되지 않거나 불완전한 이동 지연 처리
    public void MoveDeley(InputAction.CallbackContext context)
    {
        if (context.started && !_isMoving && context.canceled)
        {
            _isMoving = false;
        }
    }

    // 충돌 처리 (장애물, 지면 등)
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Obstacle")
        {
            _playerAnimator.SetTrigger("Death");
            _playerAnimator.SetBool("Run", false);
            Debug.Log("Game Over");
            _playerDeath = true;
        }

        if (collision.gameObject.tag == "Ground" && isSliding == false) 
        {
            _playerAnimator.SetBool("Run", true);
            isGrounded = true;
            isJumping = false;
        }
    }
}