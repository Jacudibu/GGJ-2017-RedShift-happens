﻿using System.Collections;
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

    public int life = 1;
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
                gameObject.layer = LayerMask.NameToLayer("BLUE");
                break;
            case EnemyColor.RED:
                renderer.material.color = Color.red;
                gameObject.layer = LayerMask.NameToLayer("RED");
                break;
            case EnemyColor.GREEN:
                renderer.material.color = Color.green;
                gameObject.layer = LayerMask.NameToLayer("GREEN");
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<LaserCollider>() != null)
        {
            Destroy(collision.gameObject);

            if (parentEnemy != null)
                return;

            life--;

            if (life == 0)
                DiePainfully();
        }
    }

    private void DiePainfully()
    {
        Destroy(gameObject);
    }
}