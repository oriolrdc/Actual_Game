using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    //Componentes
    private CharacterController _controller;
    [SerializeField] private CapsuleCollider _collider;
    private Animator _animator;
    //Inputs 
    private InputAction _moveAction;
    private InputAction _jumpAction;
    private InputAction _lookAction;
    private InputAction _AttackAction;
    private InputAction _dashAction;
    [SerializeField] private Vector2 _lookInput;
    private InputAction _grabAction;
    private Vector2 _moveInput;
    //Variables
    [SerializeField] private float _jumpHeight = 2;
    [SerializeField] private float _movementSpeed = 5;
    [SerializeField] private float _smoothTime = 0.1f;
    private float _turnSmoothVelocity;
    [SerializeField] private float _pushForce = 10;
    [SerializeField] private float _throwForce = 20;
    //Camera
    private Transform _mainCamera;
    [SerializeField] float _speedChangeRate = 10;
    float _speed;
    float _animationSpeed;
    float sprintSpeed = 10;
    bool isSprinting = false;
    float targetAngle;
    //Cambio de personajes
    [SerializeField] private GameObject _Chico;
    [SerializeField] private GameObject _Chica;
    private InputAction _changeAction;
    [SerializeField] private bool _ChicoActive = false;
    [SerializeField] private bool _ChicaActive = true;
    private bool _isChanging = false;
    //DASH
    [SerializeField] private float _dashSpeed = 20;
    private bool _dashing = false;
    float _dashTimer;
    [SerializeField] float _dashDuration = 0.5f;
    //AttackChica
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform _shooter;

    void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _moveAction = InputSystem.actions["Move"];
        _lookAction = InputSystem.actions["Look"];
        _changeAction = InputSystem.actions["Change"];
        _AttackAction = InputSystem.actions["Attack"];
        _dashAction = InputSystem.actions["Jump"];
        _mainCamera = Camera.main.transform;
        _animator = GetComponentInChildren<Animator>();
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
        
        if(_changeAction.WasPressedThisFrame() && _isChanging == false)
        {
            _isChanging = true;
            StartCoroutine(Change());
        }
        if(_AttackAction.WasPressedThisFrame())
        {
            Attack();
        }
        if(_AttackAction.WasCompletedThisFrame())
        {
            Attack();
        }

        if(_dashAction.WasPressedThisFrame() && _dashing == false)
        {
            StartCoroutine(Dash());
        }
    }

    void Movimiento()
    {
         Vector3 direction = new Vector3(_moveInput.x, 0, _moveInput.y);

        /*Ray ray = Camera.main.ScreenPointToRay(_lookInput);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            Vector3 playerForward = hit.point - transform.position;
            playerForward.y = 0;
            transform.forward = playerForward;
        }*/

        if (direction != Vector3.zero)
        {
            _controller.Move(direction.normalized * _movementSpeed * Time.deltaTime);
            targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, _smoothTime);
            transform.rotation = Quaternion.Euler(0, smoothAngle, 0);
        }
    }

    void Attack()
    {
        if(_ChicaActive)
        {
            Instantiate(projectilePrefab, _shooter.position, _shooter.rotation);
        }
        if(_ChicoActive)
        {
            Debug.Log("Ataque Chico");
        }
    }

    IEnumerator Dash()
    {
        _dashing = true;
        _dashTimer = 0;
        Physics.IgnoreLayerCollision(0, 3, true);

        Vector3 dashDirection = new Vector3(_moveInput.x, 0, _moveInput.y).normalized;

        if (dashDirection == Vector3.zero)
        {
            while (_dashTimer < _dashDuration)
            {
                _dashDuration += Time.deltaTime;
                _controller.Move(transform.forward * 1 * _dashSpeed * Time.deltaTime);
                yield return null;
            }
        }

        while (_dashTimer < _dashDuration)
        {
            
            _dashTimer += Time.deltaTime;
            _controller.Move(dashDirection.normalized * _dashSpeed * Time.deltaTime);
            yield return null;
        }

        Physics.IgnoreLayerCollision(0, 3, false);
        _dashing = false;
    }

    IEnumerator Change()
    {
        if(_ChicaActive)
        {
            _ChicaActive = false;
            _ChicoActive = true;
            _Chica.SetActive(false);
            _Chico.SetActive(true);
            yield return new WaitForSeconds(1);
            _isChanging = false;
        }
        else if(_ChicoActive)
        {
            _ChicoActive = false;
            _ChicaActive = true;
            _Chico.SetActive(false);
            _Chica.SetActive(true);
            yield return new WaitForSeconds(1);
            _isChanging = false;
        }
    }
}
