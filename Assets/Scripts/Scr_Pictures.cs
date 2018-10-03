using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_Pictures : MonoBehaviour {

	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")// || other.gameObject.GetComponent<Scr_ThrowableObject>())
        {
            //gameObject.AddComponent<Rigidbody>();
            //gameObject.AddComponent<Scr_ThrowableObject>();
            //gameObject.tag = "ThrowableObject";
            gameObject.GetComponent<Rigidbody>().isKinematic = false;
        }

    }
}
