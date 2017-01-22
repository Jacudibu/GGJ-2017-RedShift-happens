using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundScenePersistence : MonoBehaviour {

	// Use this for initialization
	void Start ()
	{
			Object.DontDestroyOnLoad(gameObject);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
