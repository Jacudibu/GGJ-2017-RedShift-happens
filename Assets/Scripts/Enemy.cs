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

        transform.Rotate(0f, Time.deltaTime * rotationSpeed, 0f);
        transform.localScale = defaultScale + (Vector3.one * 0.1f * Mathf.Sin(Time.time * damageScaler));
	}

    public void Init(Enemy parent, int layer, float speed)
    {
        parentEnemy = parent;

        defaultScale = Vector3.one;
        defaultScale.x += layer * 2.5f;
        defaultScale.y += layer * 3f;
        defaultScale.z += layer * 2.5f;

        transform.localScale = defaultScale;

        //GetComponent<SpriteRenderer>().sortingOrder = 5 - layer;
        GetComponent<MeshRenderer>().sortingOrder = 5 - layer;

        rotationSpeed = Random.Range(45f, 100f);
        //this.speed = speed;
        switch (layer){
            case 3: // red
                this.life = this.life*2;
                this.speed = 0.5f*speed;
                break;
            case 2:
                this.life = (int) (this.life*1.2f);
                this.speed = 0.8f*speed;
                break;
            default:
                this.speed = speed;
                break;
        }
        // sign
        rotationSpeed = Random.Range(0, 2) == 0 ? -rotationSpeed : rotationSpeed;

        ChangeColor(layer);
    }

    private void ChangeColor(int layer)
    {
        //do
        //{
        //    color = (EnemyColor) Random.Range(0, 3);
        //} while (parentEnemy != null && parentEnemy.color == this.color);
        switch(layer){
            case 1:
                color= EnemyColor.BLUE;
                break;
            case 2:
                color= EnemyColor.GREEN;
                break;
            case 3:
                color= EnemyColor.RED;
                break;
        }
        ApplyColor();
    }


    private void ApplyColor()
    {
        Renderer renderer = GetComponent<Renderer>();

        switch (color)
        {
            case EnemyColor.BLUE:
                renderer.material.SetColor("_Color", Color.blue);
                renderer.material.SetColor("_EmissionColor", Color.blue);
                gameObject.layer = LayerMask.NameToLayer("BLUE");
                break;
            case EnemyColor.RED:
                renderer.material.SetColor("_Color", Color.red);
                renderer.material.SetColor("_EmissionColor", Color.red);
                gameObject.layer = LayerMask.NameToLayer("RED");
                break;
            case EnemyColor.GREEN:
                renderer.material.SetColor("_Color", Color.green);
                renderer.material.SetColor("_EmissionColor", Color.green);
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
