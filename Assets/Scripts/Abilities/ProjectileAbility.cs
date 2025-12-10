using UnityEngine;

[CreateAssetMenu(fileName = "NewProjectileAbility", menuName = "Abilities/Projectile")]
public class ProjectileAbility : Ability
{
    public GameObject projectilePrefab;
    public float projectileSpeed = 10f;
    public float damage = 20f;
    
    public override void Activate(GameObject owner, Vector2 direction)
    {
        TriggerOnAbilityStart();
        
        if (projectilePrefab != null)
        {
            Vector3 spawnPos = owner.transform.position + 
                               (Vector3)colliderOffset + 
                               (Vector3)direction.normalized * 0.5f;
            
            GameObject projectile = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
            Projectile projScript = projectile.GetComponent<Projectile>();
            if (projScript != null)
            {
                projScript.Initialize(direction.normalized * projectileSpeed, damage, owner);
                
                // Event für Hit in Projectile setzen
                projScript.OnHit += (target) => TriggerOnAbilityHit(target);
            }
        }
    }
}