using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Vector3 spawnPointCenter;
    public Vector3 spawnBoxSize;

    private float delayBetweenEnemies = 3f;

    private IEnumerator Start()
    {
        while (true)
        {
            yield return new WaitForSeconds(delayBetweenEnemies);
            SpawnNewEnemy();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(spawnPointCenter, spawnBoxSize);
    }

    private void SpawnNewEnemy()
    {
        Vector3 pos = new Vector3();

        pos.x = Random.Range(spawnPointCenter.x - spawnBoxSize.x * 0.5f, spawnPointCenter.x + spawnBoxSize.x * 0.5f);
        pos.y = Random.Range(spawnPointCenter.y - spawnBoxSize.y * 0.5f, spawnPointCenter.y + spawnBoxSize.y * 0.5f);

        Enemy lastEnemy = null;

        float speed = Random.Range(0.8f, 1.2f);
        int EnemyCount = Random.Range(0, 4);
        for (int i = EnemyCount; i > 0; i--)
        {
            lastEnemy = InstantiateEnemy(pos, i, lastEnemy, speed);
        }
    }

    private Enemy InstantiateEnemy(Vector3 position, int layer, Enemy parent, float speed)
    {
        Enemy enemy = GameObject.Instantiate(enemyPrefab, position, Quaternion.identity).GetComponent<Enemy>();
        enemy.Init(parent, layer, speed);

        return enemy;
    }
}
