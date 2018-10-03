using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_FurnitureBounce : MonoBehaviour 
{
    private Animator m_Animator;

	// Use this for initialization
	void Start () 
    {
		m_Animator = GetComponent<Animator>();
	}

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
            m_Animator.SetTrigger("Bounce");
    }
}
