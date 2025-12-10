using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "EntityData", menuName = "ScriptableObjects/EntityData")]
public class EntityData : ScriptableObject
{
    [Header("Base Stats")]
    public float moveSpeed = 5f;
    public int maxHealth = 100;
    public float baseDamage = 10f;
    
    [Header("Resource Stats")]
    public float maxMana = 100f;
    public float manaRegenRate = 5f;
    
    [Header("Abilities")]
    public List<Ability> abilities = new List<Ability>();
    
    [Header("Combat")]
    public float attackRange = 1.5f;
    public float attackSpeed = 1f; // Attacks per second
    
    [Header("Input Configuration (Player Only)")]
    public KeyCode[] abilityKeys ={KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4};
}