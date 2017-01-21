using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyColor
{
    RED,
    GREEN,
    BLUE,
}

public class Enemy : MonoBehaviour
{
    public Enemy parentEnemy;

    [Range(0.5f, 5f)]
    public float speed = 1f;

	void Update ()
    {
        transform.Translate(0f, -speed * Time.deltaTime, 0f);
	}

    public int GetEnemyParentCount()
    {
        if (parentEnemy == null)
        {
            return 0;
        }
        else
        {
            return parentEnemy.GetEnemyParentCount();
        }
    }
}