using UnityEngine;

[CreateAssetMenu(fileName = "NewHealAbility", menuName = "Abilities/Heal")]
public class HealAbility : Ability
{
    public float healAmount = 30f;
    public bool healOverTime = false;
    public float healDuration = 3f;
    
    public override void Activate(GameObject owner, Vector2 direction)
    {
        TriggerOnAbilityStart(); 
        
        Health health = owner.GetComponent<Health>();
        if (health != null)
        {
            if (healOverTime)
            {
                owner.GetComponent<MonoBehaviour>().StartCoroutine(HealOverTime(health));
            }
            else
            {
                health.Heal((int)healAmount);
            }
        }
    }
    
    private System.Collections.IEnumerator HealOverTime(Health health)
    {
        float healPerTick = healAmount / (healDuration * 10);
        for (int i = 0; i < healDuration * 10; i++)
        {
            health.Heal((int)healPerTick);
            yield return new WaitForSeconds(0.1f);
        }
    }
}