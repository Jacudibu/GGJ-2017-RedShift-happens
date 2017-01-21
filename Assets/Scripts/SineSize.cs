using UnityEngine;
using System.Collections;

public class SineSize : MonoBehaviour
{

	public float SizeSpeed = 1;
	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update () {

		transform.localScale = Vector3.one * (1f + Mathf.Sin(Time.time * SizeSpeed) * 0.2f);
	
	}
}
