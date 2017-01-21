using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserCollider : MonoBehaviour
{
    public float maxYCoordinate = 30f;
    private float speed;

    public void Init(float speed)
    {
        this.speed = speed;
    }

    void Update ()
    {
        transform.Translate(0f, speed * Time.deltaTime, 0f);

        if (transform.position.y > maxYCoordinate)
        {
            Destroy(gameObject);
        }
	}
}
