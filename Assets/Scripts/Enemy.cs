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

    public int life = 40;
    public float baseScale = 0.5f;
    [Range(0.5f, 5f)]
    public float speed = 1f;

    private int playerDamage = 0;
    private int playerPoints = 0;

    private Vector3 defaultScale;
    private float timeSinceLastHit;

    private float rotationSpeed = 1f;

    private static PlayerController player;

	void Update ()
    {
        transform.position = transform.position - new Vector3(0f, speed * Time.deltaTime, 0f);

        float damageScaler = Mathf.Lerp(10f, 1f, life / 40f);

        transform.Rotate(0f, Time.deltaTime * rotationSpeed, 0f);
        transform.localScale = defaultScale + (Vector3.one * 0.1f * Mathf.Sin(Time.time * damageScaler));

        if (transform.position.y < 1){
            // enemy is too low, destroy and lower player health
            player.plHealth -= this.playerDamage;
            // destroy
            Destroy(gameObject);
        }
    }

    public void Init(int layer, float speed)
    {
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
                this.playerDamage = 1;
                this.playerPoints = 10;
                break;
            case 2: // green
                this.life = (int) (this.life*1.2f);
                this.speed = 0.8f*speed;
                this.playerDamage = 1;
                this.playerPoints = 20;
                break;
            default:// blue at the moment
                this.speed = speed;
                this.playerDamage = 1;
                this.playerPoints = 30;
                break;
        }
        // sign
        rotationSpeed = Random.Range(0, 2) == 0 ? -rotationSpeed : rotationSpeed;

        ChangeColor(layer);

        // find player if not already done
        if (player == null){
            player = FindObjectOfType<PlayerController>();
        }
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<LaserCollider>() != null)
        {
            Destroy(collision.gameObject);
            life--;

            if (life == 0)
                DiePainfully();
        }
    }

    private void DiePainfully()
    {
        ScreenShaker.cameraInstance.Shake(0.5f, 0.2f);
        player.points += playerPoints;
        Destroy(gameObject);
    }
}
