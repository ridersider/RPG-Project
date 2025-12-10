using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class SpawnWave
    {
        public EnemyData enemyType;
        public int count;
        public float delayBetweenSpawns = 1f;
    }
    
    [SerializeField] private List<SpawnWave> waves = new List<SpawnWave>();
    [SerializeField] private float spawnRadius = 5f;
    [SerializeField] private bool spawnOnStart = true;
    [SerializeField] private Transform targetOverride; // Optional player override
    
    private List<EnemyController2D> activeEnemies = new List<EnemyController2D>();
    
    private void Start()
    {
        if (spawnOnStart)
        {
            SpawnWaveImmediate(0);
        }
    }
    
    public void SpawnWaveImmediate(int waveIndex)
    {
        if (waveIndex < 0 || waveIndex >= waves.Count) return;
        
        SpawnWave wave = waves[waveIndex];
        StartCoroutine(SpawnWaveRoutine(wave));
    }
    
    private IEnumerator SpawnWaveRoutine(SpawnWave wave)
    {
        for (int i = 0; i < wave.count; i++)
        {
            SpawnEnemy(wave.enemyType);
            yield return new WaitForSeconds(wave.delayBetweenSpawns);
        }
    }
    
    public EnemyController2D SpawnEnemy(EnemyData enemyData)
    {
        Vector2 spawnPos = (Vector2)transform.position + Random.insideUnitCircle * spawnRadius;
        
        //ToDo: Use object pooling in a real project
        // Create enemy GameObject
        GameObject enemyObj = new GameObject($"Enemy_{enemyData.name}");
        enemyObj.transform.position = spawnPos;
        
        // Add required components
        enemyObj.AddComponent<Rigidbody2D>();
        enemyObj.AddComponent<Health>();
        enemyObj.AddComponent<SpriteAnimator>();
        
        // Add enemy controller
        EnemyController2D enemy = enemyObj.AddComponent<EnemyController2D>();
        
        // Set data
        // Note: In practice, you'd use a prefab system
        
        // Set target (find player if not overridden)
        if (targetOverride == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                enemy.SetTarget(player.transform);
            }
        }
        else
        {
            enemy.SetTarget(targetOverride);
        }
        
        // Register for death event
        enemy.OnDeath += HandleEnemyDeath;
        
        activeEnemies.Add(enemy);
        return enemy;
    }
    
    private void HandleEnemyDeath(EntityController2D enemy)
    {
        activeEnemies.Remove(enemy as EnemyController2D);
        
        // Handle loot drops, experience, etc.
        EnemyData enemyData = (enemy as EnemyController2D).GetEntityData() as EnemyData;
        if (enemyData != null)
        {
            // Drop loot
            if (Random.value < enemyData.dropChance && enemyData.lootDrops.Length > 0)
            {
                GameObject loot = enemyData.lootDrops[Random.Range(0, enemyData.lootDrops.Length)];
                Instantiate(loot, enemy.transform.position, Quaternion.identity);
            }
        }
    }
}