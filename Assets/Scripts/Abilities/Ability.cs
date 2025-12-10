using UnityEngine;

public abstract class Ability : ScriptableObject
{
    public string abilityName;
    public float cooldown = 1f;
    public float manaCost = 10f;
    public Sprite icon;
    
    [Header("Animation")]
    public PlayerAnimState triggerAnimation;
    public bool requiresAiming = false;
    
    public event System.Action<Ability> OnAbilityStart;
    public event System.Action<Ability, GameObject> OnAbilityHit;
    public event System.Action<Ability> OnAbilityEnd;
    
    [Header("Collider Configuration")]
    public BoxCollider2D colliderPrefab;
    public float colliderDuration = 0.5f;
    public Vector2 colliderOffset = Vector2.zero;
    
    public abstract void Activate(GameObject owner, Vector2 direction);
    
    public virtual bool CanActivate(GameObject owner)
    {
        return true;
    }
    
    // Event-Trigger-Methoden hinzufügen
    protected virtual void TriggerOnAbilityStart()
    {
        OnAbilityStart?.Invoke(this);
    }
    
    protected virtual void TriggerOnAbilityHit(GameObject target = null)
    {
        OnAbilityHit?.Invoke(this, target);
    }
    
    protected virtual void TriggerOnAbilityEnd()
    {
        OnAbilityEnd?.Invoke(this);
    }
}