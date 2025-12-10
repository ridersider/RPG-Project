using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "ScriptableObjects/EnemyData")]
public class EnemyData : EntityData
{
    [Header("Combat Behavior")]
    public float attackCooldown = 1f;
    public int attackDamage = 10;
    public bool usesAbilities = false;
    
    [Header("Loot")]
    public int experienceReward = 10;
    public GameObject[] lootDrops;
    public float dropChance = 0.3f;
}