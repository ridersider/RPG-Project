using UnityEngine;

public class PlayerController2D : EntityController2D
{
    private Camera mainCamera;
    
    protected override void Start()
    {
        base.Start();
        mainCamera = Camera.main;
        
        // Player is always on team "Player"
        gameObject.tag = "Player";
        gameObject.layer = LayerMask.NameToLayer("Player");
    }
    
    protected override void Update()
    {
        base.Update();
        HandlePlayerInput();
    }
    
    private void HandlePlayerInput()
    {
        // Movement input
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput = moveInput.normalized;
        
        // Combat input
        if (moveInput.sqrMagnitude > 0.01f)
            lastMoveDir = moveInput;
    }
    
    protected override void HandleMovement()
    {
        if (currentState == EntityState.Dead) return;
        rb.linearVelocity = moveInput * entityData.moveSpeed;
    }
    
    protected override void HandleCombatInput()
    {
        if (currentState == EntityState.Dead) return;
        
        // Ability Keys
        if (entityData?.abilityKeys != null)
        {
            for (int i = 0; i < Mathf.Min(entityData.abilityKeys.Length, equippedAbilities.Count); i++)
            {
                if (Input.GetKeyDown(entityData.abilityKeys[i]))
                {
                    Vector2 aimDir = GetAimDirection();
                    
                    // Mouse aiming für zielgerichtete Abilities
                    if (equippedAbilities[i].requiresAiming)
                    {
                        Vector2 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                        aimDir = (mousePos - (Vector2)transform.position).normalized;
                        LookAt(mousePos);
                    }
                    
                    TryUseAbility(i, aimDir);
                }
            }
        }
    }
    
    public override Vector2 GetAimDirection()
    {
        // Mouse Aiming hat Priorität
        if (mainCamera != null)
        {
            Vector2 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mouseDir = (mousePos - (Vector2)transform.position).normalized;
            
            // Nur wenn Maus bewegt wurde oder geklickt wurde
            if (mouseDir.sqrMagnitude > 0.01f)
                return mouseDir;
        }
        
        // Fallback: Bewegung oder letzte Richtung
        return base.GetAimDirection();
    }
    
    public void LookAt(Vector2 position)
    {
        Vector2 direction = (position - (Vector2)transform.position).normalized;
        lastMoveDir = direction;
        
        if (animator != null)
            animator.SetFacingDirection(direction);
    }
}