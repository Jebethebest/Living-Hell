using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_SafezoneController : MonoBehaviour 
{
	// Use this for initialization
	private void Start () 
    {
		//nothing to do
	}
	
	// Update is called once per frame
	private void Update () 
    {
		//nothing to do
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
            other.gameObject.GetComponent<Scr_PlayerStateController>().PlayerState = Scr_PlayerStateController.State.Safe;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
            other.gameObject.GetComponent<Scr_PlayerStateController>().PlayerState = Scr_PlayerStateController.State.Unsafe;
    }
}
