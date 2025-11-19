using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Health))]
public class PlayerController2D : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody2D _rb;
    private Health _health;

    private Vector2 _moveInput;
    private Vector2 _lastMoveDir = Vector2.right;
    
    public PlayerData playerData;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _health = GetComponent<Health>();
    }

    private void Start()
    {
        ApplyCharacterDefinition();
    }

    public void ApplyCharacterDefinition()
    {
        if (playerData == null) return;

        moveSpeed = playerData.moveSpeed;

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
        }
    }

    private void FixedUpdate()
    {
        _rb.linearVelocity = _moveInput * moveSpeed;
    }

    public Vector2 GetAimDirection()
    {
        // Im Grundgerüst: Aim = Lauf-Richtung
        return _lastMoveDir.sqrMagnitude > 0.01f ? _lastMoveDir : Vector2.right;
    }
}
