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
    public EnemyColor color;
    public Enemy parentEnemy;
    public float baseScale = 0.5f;
    [Range(0.5f, 5f)]
    public float speed = 1f;

	void Update ()
    {
        transform.Translate(0f, -speed * Time.deltaTime, 0f);
	}

    public void Init(Enemy parent, int layer)
    {
        parentEnemy = parent;

        Vector3 scale = Vector3.one;
        scale.x += layer;
        scale.y += layer;
        scale.z -= layer * 0.2f;

        transform.localScale = scale;

        ChangeColor();
    }

    private void ChangeColor()
    {
        do
        {
            color = (EnemyColor) Random.Range(0, 3);
        } while (parentEnemy != null && parentEnemy.color == this.color);
        ApplyColor();
    }


    private void ApplyColor()
    {
        Renderer renderer = GetComponent<Renderer>();

        switch (color)
        {
            case EnemyColor.BLUE:
                renderer.material.color = Color.blue;
                break;
            case EnemyColor.RED:
                renderer.material.color = Color.red;
                break;
            case EnemyColor.GREEN:
                renderer.material.color = Color.green;
                break;
        }
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