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
        BoxCollider2D colliderPrefab = Instantiate(this.colliderPrefab, owner.transform);
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
                dmgCollider.OnHit += (target) => TriggerOnAbilityHit(target, owner);
            }
        }
        // Collider nach der Dauer deaktivieren
        owner.GetComponent<MonoBehaviour>().StartCoroutine(DisableColliderAfterDuration(colliderPrefab, colliderDuration));
    }
    
    // Coroutine zum Deaktivieren des Colliders
    private IEnumerator DisableColliderAfterDuration(BoxCollider2D collider, float duration)
    {
        yield return new WaitForSeconds(duration);
        if (collider != null)
        {
            collider.enabled = false;
            Destroy(collider.gameObject); // Optional: Collider-Objekt zerstören
        }
    }
}