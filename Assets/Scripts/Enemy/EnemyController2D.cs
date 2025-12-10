using System.Collections;
using UnityEngine;

public class EnemyController2D : EntityController2D
{
    [Header("AI Settings")]
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float wanderRadius = 5f;
    [SerializeField] private float wanderInterval = 3f;
    [SerializeField] private LayerMask targetLayer = 1 << 6; // Player layer
    
    [Header("Behavior")]
    [SerializeField] private bool isAggressive = true;
    [SerializeField] private bool canWander = true;
    [SerializeField] private bool usePathfinding = false;
    
    private Vector2 wanderCenter;
    private Vector2 wanderTarget;
    private float stateTimer;
    private float idleTimer;
    
    // AI States
    private AIState aiState = AIState.Idle;
    
    protected override void Start()
    {
        base.Start();
        
        // Enemy setup
        gameObject.tag = "Enemy";
        gameObject.layer = LayerMask.NameToLayer("Enemy");
        
        wanderCenter = transform.position;
        
        // Start AI routine
        StartCoroutine(AIUpdateRoutine());
    }
    
    protected override void Update()
    {
        base.Update();
        
        // Update timers
        if (stateTimer > 0) stateTimer -= Time.deltaTime;
        if (idleTimer > 0) idleTimer -= Time.deltaTime;
    }
    
    protected override void HandleMovement()
    {
        if (currentState == EntityState.Dead) return;
        
        switch (aiState)
        {
            case AIState.Chasing:
                if (target != null)
                {
                    MoveTowards(target.position);
                    LookAt(target.position);
                }
                break;
                
            case AIState.Wandering:
                MoveTowards(wanderTarget);
                if (Vector2.Distance(transform.position, wanderTarget) < 0.5f)
                {
                    SetAIState(AIState.Idle);
                }
                break;
                
            case AIState.Attacking:
                StopMovement();
                LookAt(target.position);
                break;
                
            case AIState.Idle:
                StopMovement();
                break;
        }
    }
    
    protected override void HandleCombatInput()
    {
        // AI-controlled combat
        if (aiState == AIState.Attacking)
        {
            // Use basic attack if in range
            if (IsInRange(attackRange))
            {
                TryUseAbility(0, GetAimDirection());
            }
        }
    }
    
    private IEnumerator AIUpdateRoutine()
    {
        while (currentState != EntityState.Dead)
        {
            UpdateAI();
            yield return new WaitForSeconds(0.2f); // 5 Hz update rate
        }
    }
    
    private void UpdateAI()
    {
        // Check for targets
        FindNearestTarget();
        
        // State transitions
        switch (aiState)
        {
            case AIState.Idle:
                HandleIdleState();
                break;
                
            case AIState.Wandering:
                HandleWanderState();
                break;
                
            case AIState.Chasing:
                HandleChaseState();
                break;
                
            case AIState.Attacking:
                HandleAttackState();
                break;
                
            case AIState.Fleeing:
                HandleFleeState();
                break;
        }
    }
    
    private void FindNearestTarget()
    {
        if (!isAggressive) return;
        
        Collider2D[] potentialTargets = Physics2D.OverlapCircleAll(
            transform.position, 
            detectionRange, 
            targetLayer
        );
        
        float closestDistance = Mathf.Infinity;
        Transform closestTarget = null;
        
        foreach (var potentialTarget in potentialTargets)
        {
            float distance = Vector2.Distance(transform.position, potentialTarget.transform.position);
            
            // Line of sight check (optional)
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTarget = potentialTarget.transform;
            }
        }
        
        target = closestTarget;
    }
    
    private void HandleIdleState()
    {
        if (target != null)
        {
            SetAIState(AIState.Chasing);
            return;
        }
        
        if (canWander && idleTimer <= 0)
        {
            // Start wandering
            SetWanderTarget();
            SetAIState(AIState.Wandering);
            idleTimer = wanderInterval;
        }
        
        if (stateTimer <= 0)
        {
            stateTimer = Random.Range(1f, 3f);
        }
    }
    
    private void HandleWanderState()
    {
        if (target != null)
        {
            SetAIState(AIState.Chasing);
            return;
        }
        
        if (Vector2.Distance(transform.position, wanderTarget) < 0.5f)
        {
            SetAIState(AIState.Idle);
        }
    }
    
    private void HandleChaseState()
    {
        if (target == null)
        {
            SetAIState(AIState.Idle);
            return;
        }
        
        float distance = DistanceToTarget();
        
        if (distance <= attackRange)
        {
            SetAIState(AIState.Attacking);
        }
        else if (distance > detectionRange * 1.5f)
        {
            // Lost target
            target = null;
            SetAIState(AIState.Idle);
        }
    }
    
    private void HandleAttackState()
    {
        if (target == null)
        {
            SetAIState(AIState.Idle);
            return;
        }
        
        float distance = DistanceToTarget();
        
        if (distance > attackRange * 1.2f)
        {
            // Target moved away
            SetAIState(AIState.Chasing);
        }
        else if (distance < attackRange * 0.8f)
        {
            // Too close, maybe move back
            // Optional: add a backing away behavior
        }
    }
    
    private void HandleFleeState()
    {
        // Flee from target
        if (target != null)
        {
            Vector2 fleeDirection = (transform.position - target.position).normalized;
            MoveTowards((Vector2)transform.position + fleeDirection * 5f);
        }
        
        if (stateTimer <= 0)
        {
            SetAIState(AIState.Idle);
        }
    }
    
    private void SetWanderTarget()
    {
        wanderTarget = wanderCenter + Random.insideUnitCircle * wanderRadius;
    }
    
    private void SetAIState(AIState newState)
    {
        if (aiState == newState) return;
        
        // Enter new state
        aiState = newState;
        
        switch (aiState)
        {
            case AIState.Idle:
                stateTimer = Random.Range(2f, 5f);
                break;
                
            case AIState.Wandering:
                stateTimer = 10f; // Max wander time
                break;
                
            case AIState.Chasing:
                stateTimer = 20f; // Max chase time
                break;
                
            case AIState.Attacking:
                stateTimer = 3f; // Max attack time before re-evaluating
                break;
                
            case AIState.Fleeing:
                stateTimer = 5f; // Flee duration
                break;
        }
    }
    
    private IEnumerator ResetAttackFlag(float delay)
    {
        yield return new WaitForSeconds(delay);
    }
    
    // Gizmos for debugging
    private void OnDrawGizmosSelected()
    {
        // Detection range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        // Attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
        // Wander area
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(Application.isPlaying ? wanderCenter : transform.position, wanderRadius);
        
        // Current target
        if (target != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, target.position);
        }
    }
}

public enum AIState
{
    Idle,
    Wandering,
    Chasing,
    Attacking,
    Fleeing,
    Patrolling
}