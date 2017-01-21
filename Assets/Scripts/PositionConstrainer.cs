using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionConstrainer : MonoBehaviour
{
    public float minX, maxX;

    void Update ()
    {
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        transform.position = pos;
	}
}
