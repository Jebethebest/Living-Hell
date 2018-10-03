using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_AnimationController : MonoBehaviour 
{
    private Animator m_Animator;

	// Use this for initialization
	void Awake () 
    {
		m_Animator = gameObject.transform.Find("Mesh").GetComponent<Animator>();
        m_Animator.updateMode = AnimatorUpdateMode.UnscaledTime;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Animate(string flagName)
    {
        m_Animator.SetTrigger(flagName);
    }

    public void Animate(string flagName, bool value)
    {
        m_Animator.SetBool(flagName, value);
    }

    public void Enable(bool value)
    {
        m_Animator.speed = (value) ? 1.0f : 0.0f;
    }
}
