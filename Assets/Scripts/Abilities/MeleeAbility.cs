using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "NewMeleeAbility", menuName = "Abilities/Melee")]
public class MeleeAbility : Ability
{
    public float damage = 15f;
    public override void Activate(GameObject owner, Vector2 direction)
    {
        TriggerOnAbilityStart();
        // Instantiate collider
        colliderPrefab.enabled = true;
        
        // Activate collider on owner
        if (colliderPrefab != null)
        {
            DamageCollider dmgCollider = colliderPrefab.GetComponent<DamageCollider>();
            if (dmgCollider != null)
            {
                dmgCollider.SetDamage(damage, owner);
                dmgCollider.SetDuration(colliderDuration);
                
                // Event für Hit in DamageCollider setzen
                dmgCollider.OnHit += (target) => TriggerOnAbilityHit(target);
            }
        }
        colliderPrefab.enabled = false;
    }
}