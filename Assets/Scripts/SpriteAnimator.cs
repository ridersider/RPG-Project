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

    private void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
    }

    public void Play(PlayerAnimState newState)
    {
        if (_currentState == newState)
            return;

        _currentState = newState;

        // passende Animation laden
        foreach (var anim in animations)
        {
            if (anim.state == newState)
            {
                _currentFrames = anim.frames;
                _frameTime = anim.frameTime;
                _frameIndex = 0;
                _timer = 0f;
                _renderer.sprite = _currentFrames[0];
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
            _frameIndex = (_frameIndex + 1) % _currentFrames.Length;
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
}

public enum PlayerAnimState
{
    Idle,
    Run
}