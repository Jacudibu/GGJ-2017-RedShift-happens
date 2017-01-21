using UnityEngine;
using System.Collections;

public class Revolve : MonoBehaviour
{

	public float revolveSpeed = 360f;
	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update () {

		transform.Rotate(Time.deltaTime * revolveSpeed, 0f, 0f);
	
	}
}
