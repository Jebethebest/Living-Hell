using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_ThrowableObject : MonoBehaviour 
{
    [SerializeField] private Vector3 m_HoldPosition;
    [SerializeField] private Vector3 m_HoldRotation;

    private Rigidbody m_Rigidbody;
    private GameObject m_ThrownBy;
    private bool m_IsThrown = false;
    private bool m_IsHeld = false;

	// Use this for initialization
	private void Start ()
    {
		m_Rigidbody = GetComponent<Rigidbody>();

        //m_Rigidbody.constraints = RigidbodyConstraints.FreezeAll;
	}
	
	// Update is called once per frame
	private void Update () 
    {
		//do nothing
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log(m_Rigidbody.velocity.magnitude);
            if (m_Rigidbody.velocity.magnitude > 0 && m_ThrownBy != null && m_ThrownBy != other.gameObject && m_IsThrown)
            {
                Scr_PlayerStateController state = m_ThrownBy.GetComponent<Scr_PlayerStateController>();
                if (!state.IsMomInRoom)
                    Scr_ScoreManager.UpdateScore(m_ThrownBy, 500);
                else
                    Scr_ScoreManager.UpdateScore(m_ThrownBy, -50);

                Vector3 dir = transform.forward + transform.up;
                other.gameObject.GetComponent<Scr_CharacterController>().AddImpact(dir, 250.0f);
                other.gameObject.GetComponent<Scr_Combat>().SetHitBy(gameObject);
                m_IsThrown = false;

                //gameObject.SetActive(false);
                Scr_AudioManager.SetRandomPitch("ObjectBreak");
                Scr_AudioManager.Play("ObjectBreak");
            }
        }
    }

    public void SetHold(bool value)
    {
        m_IsHeld = value;
    }

    public bool IsHeld()
    {
        return m_IsHeld;
    }

    public void SetThrownBy(GameObject player)
    {
        m_ThrownBy = player;
        m_IsThrown = true;
    }

    public GameObject GetThrownBy()
    {
        return m_ThrownBy;
    }

    public Vector3 GetHoldPosition()
    {
        return m_HoldPosition;
    }

    public Vector3 GetHoldRotation()
    {
        return m_HoldRotation;
    }
}
