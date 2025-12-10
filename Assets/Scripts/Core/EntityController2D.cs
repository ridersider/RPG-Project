// Core/EntityController2D.cs
using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(SpriteAnimator))]
public abstract class EntityController2D : MonoBehaviour
{
    [Header("Entity Components")]
    [SerializeField] protected EntityData entityData;
    
    // Komponenten
    protected Rigidbody2D rb;
    protected Health health;
    protected SpriteAnimator animator;
    
    [Header("Movement")]
    protected Vector2 moveInput;
    protected Vector2 lastMoveDir = Vector2.right;
    
    [Header("Combat")]
    protected List<Ability> equippedAbilities = new List<Ability>();
    protected float[] abilityCooldowns;
    protected Transform target;
    
    [Header("State")]
    public EntityState currentState = EntityState.Idle;
    
    // Events
    public System.Action<EntityController2D> OnDeath;
    public System.Action<Ability, int> OnAbilityUsed;
    
    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<Health>();
        animator = GetComponent<SpriteAnimator>();
    
        // Health events - von AddListener zu C# Event ändern
        health.OnDeath += HandleDeath;
    }
    
    protected virtual void Start()
    {
        if (entityData != null)
        {
            health.Initialize(entityData.maxHealth);
            
            // Load abilities from entity data
            if (entityData.abilities != null)
            {
                equippedAbilities = new List<Ability>(entityData.abilities);
                abilityCooldowns = new float[equippedAbilities.Count];
            }
        }
    }
    
    protected virtual void Update()
    {
        UpdateCooldowns();
        UpdateAnimation();
        HandleCombatInput();
    }
    
    protected virtual void FixedUpdate()
    {
        HandleMovement();
    }
    
    // ===== ABSTRACT METHODS =====
    protected abstract void HandleMovement();
    protected abstract void HandleCombatInput();
    
    // ===== SHARED METHODS =====
    protected void UpdateCooldowns()
    {
        for (int i = 0; i < abilityCooldowns.Length; i++)
        {
            if (abilityCooldowns[i] > 0)
                abilityCooldowns[i] -= Time.deltaTime;
        }
    }
    
    protected void UpdateAnimation()
    {
        if (animator == null) return;
        
        // Animation basierend auf State (kann erweitert werden)
        if (moveInput.sqrMagnitude > 0.01f)
        {
            animator.Play(PlayerAnimState.Run);
            animator.SetFacingDirection(moveInput);
        }
        else
        {
            animator.Play(PlayerAnimState.Idle);
        }
    }
    
    public bool TryUseAbility(int abilityIndex, Vector2 direction)
    {
        if (abilityIndex < 0 || abilityIndex >= equippedAbilities.Count)
            return false;
            
        if (abilityCooldowns[abilityIndex] > 0)
            return false;
            
        Ability ability = equippedAbilities[abilityIndex];
        
        if (ability != null && ability.CanActivate(gameObject))
        {
            ability.Activate(gameObject, direction);
            abilityCooldowns[abilityIndex] = ability.cooldown;
            
            // Animation triggern
            if (ability.triggerAnimation != PlayerAnimState.Idle)
            {
                animator.Play(ability.triggerAnimation);
            }
            
            OnAbilityUsed?.Invoke(ability, abilityIndex);
            return true;
        }
        
        return false;
    }
    
    public virtual Vector2 GetAimDirection()
    {
        if (target != null)
        {
            // Aim at target
            return (target.position - transform.position).normalized;
        }
        
        return lastMoveDir.sqrMagnitude > 0.01f ? lastMoveDir : Vector2.right;
    }

    
    public void SetAimDirection(Vector2 dir)
    {
        
    }
    
    public float DistanceToTarget()
    {
        if (target == null) return Mathf.Infinity;
        return Vector2.Distance(transform.position, target.position);
    }
    
    protected void HandleDeath(Health health)
    {
        currentState = EntityState.Dead;
        OnDeath?.Invoke(this);
        
        // Disable components
        rb.simulated = false;
        enabled = false;
    }
    
    // ===== PUBLIC API =====
    public void MoveTowards(Vector2 position, float speedMultiplier = 1f)
    {
        Vector2 direction = (position - (Vector2)transform.position).normalized;
        moveInput = direction;
        
        if (rb != null)
        {
            rb.linearVelocity = direction * entityData.moveSpeed * speedMultiplier;
        }
    }
    
    public void StopMovement()
    {
        moveInput = Vector2.zero;
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }
    
    public bool IsInRange(float range)
    {
        return DistanceToTarget() <= range;
    }
    
    public void LookAt(Vector2 position)
    {
        Vector2 direction = (position - (Vector2)transform.position).normalized;
        lastMoveDir = direction;
        
        if (animator != null)
        {
            animator.SetFacingDirection(direction);
        }
    }
    
    // ===== GETTER & SETTER =====
    public EntityData GetEntityData() => entityData;
    public List<Ability> GetAbilities() => equippedAbilities;
    public float GetAbilityCooldown(int index) => 
        (index >= 0 && index < abilityCooldowns.Length) ? abilityCooldowns[index] : 0f;
    public float GetAbilityCooldownPercent(int index) =>
        (index >= 0 && index < equippedAbilities.Count) ? 
            abilityCooldowns[index] / equippedAbilities[index].cooldown : 0f;
    
    public void SetTarget(Transform newTarget) => target = newTarget;
    public bool HasTarget() => target != null;
}