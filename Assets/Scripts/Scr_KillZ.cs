using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_KillZ : MonoBehaviour 
{
	// Use this for initialization
	void Start () 
    {
        //nothing to do
	}
	
	// Update is called once per frame
	void Update () 
    {
		//nothing to do
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
            other.gameObject.GetComponent<Scr_CharacterController>().Respawn();

        if (other.gameObject.tag == "Projectile")
            other.gameObject.SetActive(false);

        if (other.gameObject.tag == "ThrowableObject")
            other.gameObject.SetActive(false);
    }
}
