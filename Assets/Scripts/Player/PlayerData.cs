using System.Collections.Generic;
using UnityEngine;

public class PlayerData : ScriptableObject
{
    public float moveSpeed = 5f;
    public int maxHealth = 100;
    
    public List<Ability> abilities;
}
