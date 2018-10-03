using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_LavaController : MonoBehaviour {

    [SerializeField] private GameObject[] m_Players;
    [SerializeField] private float m_TransitionSpeed;
    [SerializeField] private GameObject m_Light;
    [SerializeField] private GameObject m_FloorCollider;

    private Scr_Combat[] m_PlayerCombat;
    private bool m_IsUpdating = false;
    private float m_Direction;
    private float m_MaxHeight;

    private enum LightState { On, Off };
    private LightState m_LightState = LightState.On;

	// Use this for initialization
	private void Start () 
    {
		m_PlayerCombat = new Scr_Combat[m_Players.Length];

        for (int i = 0; i < m_PlayerCombat.Length; ++i)
        {
            m_PlayerCombat[i] = m_Players[i].GetComponent<Scr_Combat>();
        }

        m_MaxHeight = transform.position.y;
	}
	
	// Update is called once per frame
	private void Update () 
    {
        Vector3 pos = transform.position;

        if (m_IsUpdating)
        {         
            pos.y += m_TransitionSpeed * m_Direction * Time.deltaTime;
            transform.position = pos;

            if (pos.y > m_MaxHeight && m_Direction > 0)
            {
                m_IsUpdating = false;
                pos.y = m_MaxHeight;
            }
        } 
        
        //Toggle red light
        if (pos.y > 0.0f && m_LightState == LightState.Off)
        {
            m_Light.SetActive(true);
            m_LightState = LightState.On;
            m_FloorCollider.SetActive(false);
            Debug.Log("Disable collider");
            Scr_AudioManager.Play("CombatTheme");
        }
        else if (pos.y <= 0.0f && m_LightState == LightState.On)
        {
            m_Light.SetActive(false);
            m_LightState = LightState.Off;
            m_FloorCollider.SetActive(true);
            Debug.Log("Enable collider");
        }
	}

    private void OnTriggerEnter(Collider other)
    {
        for (int i = 0; i < m_Players.Length; ++i)
        {
            if (other.gameObject == m_Players[i])
            {
                Debug.Log(m_Players[i].name + " touches the lava!");

                GameObject hitBy = m_PlayerCombat[i].GetLastHit();
                if (hitBy != null)
                {
                    if (hitBy.tag == "Player")
                    {
                        Scr_ScoreManager.UpdateScore(hitBy, 100);
                        m_PlayerCombat[i].SetHitBy(null);
                        Debug.Log(hitBy.name + " got 100 points");
                    }
                    else if (hitBy.tag == "ThrowableObject")
                    {
                        Debug.Log("LavaController: " + m_Players[i] + "got hit by object");

                        GameObject thrower = hitBy.GetComponent<Scr_ThrowableObject>().GetThrownBy();
                        Scr_ScoreManager.UpdateScore(thrower, 200);
                        m_PlayerCombat[i].SetHitBy(null);
                    }
                }
                else
                {
                    Scr_ScoreManager.UpdateScore(m_Players[i], -50);
                }

                //Lava particle
                Vector3 rotation = new Vector3(-90.0f, 0.0f, 0.0f);
                Scr_ParticleManager.SpawnParticle("LavaSpark", other.gameObject.transform.position, Quaternion.Euler(rotation));

                //Smoke particle
                Scr_ParticleManager.SpawnParticle("Smoke", other.gameObject.transform.position, Quaternion.Euler(rotation));

                //Sound
                Scr_AudioManager.Play("Splash");

                //Apply vibration
                Scr_Input playerInput = m_Players[i].GetComponent<Scr_Input>();
                playerInput.Vibrate(0.3f, 0.6f);
            }
        }

        if (other.gameObject.tag == "ThrowableObject")
        {
            Vector3 rotation = new Vector3(-90.0f, 0.0f, 0.0f);
            Scr_ParticleManager.SpawnParticle("Smoke", other.gameObject.transform.position, Quaternion.Euler(rotation));
        }
    }

    private void OnEnable()
    {
        Scr_EventManager.StartListening("Mom_Comes", () => { 
            m_IsUpdating = true;
            m_Direction = -1.0f;
            Scr_AudioManager.Pause("CombatTheme");
        });

        Scr_EventManager.StartListening("Mom_Leaves", () => { 
            m_IsUpdating = true;
            m_Direction = 1.5f;
        });

        Scr_EventManager.StartListening("Mom_In_Room", () => { 
            m_IsUpdating = false;
        });
    }

    private void OnDisable()
    {
        Scr_EventManager.StopListening("Mom_Comes", () => {
            m_IsUpdating = true;
            m_Direction = -1.0f;
        });

        Scr_EventManager.StopListening("Mom_Leaves", () => {
            m_IsUpdating = true;
            m_Direction = 1.5f;
        });

        Scr_EventManager.StopListening("Mom_In_Room", () => {
            m_IsUpdating = false;
        });
    }
}
