using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Vector2 velocity;
    public float damage;
    public GameObject owner;
    public float lifetime = 3f;
    
    // Event für Hits hinzufügen
    public event System.Action<GameObject> OnHit;
    
    public void Initialize(Vector2 vel, float dmg, GameObject own)
    {
        velocity = vel;
        damage = dmg;
        owner = own;
        Destroy(gameObject, lifetime);
    }
    
    private void Update()
    {
        transform.position += (Vector3)velocity * Time.deltaTime;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (owner != null && other.gameObject == owner) return;
        
        Health health = other.GetComponent<Health>();
        if (health != null)
        {
            health.TakeDamage((int)damage);
            
            // Event auslösen
            OnHit?.Invoke(other.gameObject);
            Destroy(gameObject);
        }
    }
}