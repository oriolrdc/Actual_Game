using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    private CharacterController _controller;
    private InputAction _moveAction;
    private Vector2 _moveInput;
    private InputAction _lookAction;
    private Vector2 _lookInput;
    [SerializeField] private float _movementSpeed = 5;


    //Dash
    private InputAction _dashAction;
    [SerializeField] private float _dashSpeed = 20;
    [SerializeField] private float _dashDuration;
    [SerializeField] private float _dashTimer = 0.5f;
    [SerializeField] private bool _dashing;

    void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _moveAction = InputSystem.actions["Move"];
        _lookAction = InputSystem.actions["Look"];
        _dashAction = InputSystem.actions["Jump"];
    }

    void Start()
    {
        _dashing = false;
    }
    
    void Update()
    {
        _moveInput = _moveAction.ReadValue<Vector2>();
        _lookInput = _lookAction.ReadValue<Vector2>();

        Movimiento();

        if(_dashAction.WasPressedThisFrame() && _dashing == false)
        {
            StartCoroutine(Dash());
        }
    }

    void Movimiento()
    {
        Vector3 direction = new Vector3(_moveInput.x, 0, _moveInput.y);

        Ray ray = Camera.main.ScreenPointToRay(_lookInput);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            Vector3 playerForward = hit.point - transform.position;
            Debug.Log(hit.transform.name);
            playerForward.y = 0;
            transform.forward = playerForward;
        }

        if (direction != Vector3.zero)
        {
            _controller.Move(direction.normalized * _movementSpeed * Time.deltaTime);
        }
    }

    IEnumerator Dash()
{
    _dashing = true;
    _dashDuration = 0;

    Vector3 dashDirection = new Vector3(_moveInput.x, 0, _moveInput.y).normalized;

    if (dashDirection == Vector3.zero)
    {
        while (_dashDuration < _dashTimer)
        {
            _dashDuration += Time.deltaTime;
            _controller.Move(transform.forward * 1 * _dashSpeed * Time.deltaTime);
            yield return null;
        }
    }

    while (_dashDuration < _dashTimer)
    {
        _dashDuration += Time.deltaTime;
        _controller.Move(dashDirection.normalized * _dashSpeed * Time.deltaTime);
        yield return null;
    }

    _dashing = false;
}

    
}
