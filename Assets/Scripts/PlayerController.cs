using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float speed;

	void Update ()
    {
        transform.position = transform.position + Vector3.right * Input.GetAxis("Horizontal") * speed * Time.deltaTime;
	}
}
