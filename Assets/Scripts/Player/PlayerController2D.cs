using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(SpriteAnimator))]
public class PlayerController2D : MonoBehaviour
{
    private float _moveSpeed = 5f;

    private Rigidbody2D _rb;
    private Health _health;
    private SpriteAnimator _animator;

    private Vector2 _moveInput;
    private Vector2 _lastMoveDir = Vector2.right;
    
    private bool _isMoving;
    
    public PlayerData playerData;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _health = GetComponent<Health>();
        _animator = GetComponent<SpriteAnimator>();
    }

    private void Start()
    {
        ApplyCharacterDefinition();
        _animator.Play(PlayerAnimState.Idle);
    }

    public void ApplyCharacterDefinition()
    {
        if (playerData == null) return;

        _moveSpeed = playerData.moveSpeed;
        _health.Initialize(playerData.maxHealth);
    }

    private void Update()
    {
        _moveInput.x = Input.GetAxisRaw("Horizontal");
        _moveInput.y = Input.GetAxisRaw("Vertical");
        _moveInput = _moveInput.normalized;

        if (_moveInput.sqrMagnitude > 0.01f)
        {
            _lastMoveDir = _moveInput;
            
            _animator.SetFacingDirection(_lastMoveDir);
            _animator.Play(PlayerAnimState.Run);
        }
        else
        {
            _animator.Play(PlayerAnimState.Idle);
        }
    }

    private void FixedUpdate()
    {
        _rb.linearVelocity = _moveInput * _moveSpeed;
    }

    public Vector2 GetAimDirection()
    {
        // Im Grundgerüst: Aim = Lauf-Richtung
        return _lastMoveDir.sqrMagnitude > 0.01f ? _lastMoveDir : Vector2.right;
    }
}
