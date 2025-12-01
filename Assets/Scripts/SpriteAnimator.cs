using UnityEngine;

public class SpriteAnimator : MonoBehaviour
{
    public AnimSet[] animations;

    private SpriteRenderer _renderer;
    private PlayerAnimState _currentState;
    private Sprite[] _currentFrames;

    private int _frameIndex;
    private float _timer;
    private float _frameTime;
    
    private bool _facingRight = true;
    private bool _isLocked;

    private void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
    }

    public void Play(PlayerAnimState newState = PlayerAnimState.Idle)
    {
        if (_isLocked) 
            return;

        if (_currentState == newState)
            return;

        _currentState = newState;

        foreach (var anim in animations)
        {
            if (anim.state == newState)
            {
                _currentFrames = anim.frames;
                _frameTime = anim.frameTime;
                _frameIndex = 0;
                _timer = 0f;
                _renderer.sprite = _currentFrames[0];

                // Lock aktivieren
                if (anim.uninterruptible)
                    _isLocked = true;

                return;
            }
        }
    }

    private void Update()
    {
        if (_currentFrames == null || _currentFrames.Length == 0)
            return;

        _timer += Time.deltaTime;

        if (_timer >= _frameTime)
        {
            _timer -= _frameTime;

            // Frame wechseln
            _frameIndex++;

            // Wenn am Ende angekommen
            if (_frameIndex >= _currentFrames.Length)
            {
                if (_isLocked)
                {
                    _frameIndex = _currentFrames.Length - 1; // Letztes Frame halten
                    _isLocked = false;                       // Lock aufheben
                    return;                                  // Stoppt Loop
                } else {
                    _frameIndex = 0; // Zurück zum ersten Frame
                }
            }

            _renderer.sprite = _currentFrames[_frameIndex];
        }
    }
    
    public void SetFacingDirection(Vector2 dir)
    {
        if (dir.x < -0.01f)
            _facingRight = false;
        else if (dir.x > 0.01f)
            _facingRight = true;

        _renderer.flipX = _facingRight;
    }
}

[System.Serializable]
public class AnimSet
{
    public PlayerAnimState state;
    public Sprite[] frames;
    public float frameTime = 0.15f; // Zeit pro Frame
    public bool uninterruptible = false;
}

public enum PlayerAnimState
{
    Idle,
    Run,
    Attack
}