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

    public int life = 40;
    public float baseScale = 0.5f;
    [Range(0.5f, 5f)]
    public float speed = 1f;

    private Vector3 defaultScale;
    private float timeSinceLastHit;

    private float rotationSpeed = 1f;

	void Update ()
    {
        transform.position = transform.position - new Vector3(0f, speed * Time.deltaTime, 0f);

        float damageScaler = Mathf.Lerp(10f, 1f, life / 40f);

        transform.Rotate(0f, 0f, Time.deltaTime * rotationSpeed);
        transform.localScale = defaultScale + (Vector3.one * 0.1f * Mathf.Sin(Time.time * damageScaler));
	}

    public void Init(Enemy parent, int layer)
    {
        parentEnemy = parent;

        defaultScale = Vector3.one * 0.2f;
        defaultScale.x += layer * 0.25f;
        defaultScale.y += layer * 0.25f;

        transform.localScale = defaultScale;

        GetComponent<SpriteRenderer>().sortingOrder = 5 - layer;

        rotationSpeed = Random.Range(45f, 200f);

        // sign
        rotationSpeed = Random.Range(0, 2) == 0 ? -rotationSpeed : rotationSpeed;

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
            if (parentEnemy != null)
                return;

            Destroy(collision.gameObject);
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