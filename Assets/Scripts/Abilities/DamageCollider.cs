using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class DamageCollider : MonoBehaviour
{
    private float damage = 10f;
    public GameObject owner;
    public LayerMask targetLayers;
    
    // Event für Hits hinzufügen
    public event System.Action<GameObject> OnHit;
    
    public void SetDamage(float dmg, GameObject own)
    {
        damage = dmg;
        owner = own;
    }
    
    public void SetDuration(float dur)
    {
        Destroy(gameObject, dur);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (owner != null && other.gameObject == owner) return;
        
        if (((1 << other.gameObject.layer) & targetLayers) != 0)
        {
            Health health = other.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage((int)damage);
                
                // Event auslösen
                OnHit?.Invoke(other.gameObject);
            }
        }
    }
}