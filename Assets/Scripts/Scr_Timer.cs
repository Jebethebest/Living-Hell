using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_Timer : MonoBehaviour 
{
    [SerializeField] private float m_GameTime;

    [SerializeField] private float m_MomMinTriggerTime;
    [SerializeField] private float m_MomMaxTriggerTime;
    [SerializeField] private float m_MomStayTime;
    [SerializeField] private float m_MomComingTime;

    [SerializeField] private GameObject m_SafeArrow;

    private float m_MomTimer;
    private bool m_IsMomInRoom = false;
    private bool m_IsWarningFired = false;

	// Use this for initialization
	private void Start () 
    {
        SetRandomMomTimer();
    }
	
	// Update is called once per frame
	private void Update () 
    {
		if (m_GameTime > 0.0f)
            m_GameTime -= Time.deltaTime;

        if (m_MomTimer >= 0.0f)
            m_MomTimer -= Time.deltaTime;

        if (m_MomTimer < m_MomComingTime && !m_IsWarningFired)
        {
            Scr_EventManager.TriggerEvent("Mom_Comes");
            Scr_AudioManager.Play("Suspense");
            m_IsWarningFired = true;

            m_SafeArrow.SetActive(true);
        }

        if (m_MomTimer < 0.0f)
        {
            if (!m_IsMomInRoom)
            {
                Scr_EventManager.TriggerEvent("Mom_In_Room");
                m_MomTimer = m_MomStayTime;
                m_IsMomInRoom = true;
                m_SafeArrow.SetActive(false);
            }
            else
            {
                Scr_EventManager.TriggerEvent("Mom_Is_Leaving");
                SetRandomMomTimer();
                m_IsMomInRoom = false;
                m_IsWarningFired = false;
            }
        }
	}

    private void SetRandomMomTimer()
    {
        //Set amount of seconds until mom comes
        m_MomTimer = Random.Range(m_MomMinTriggerTime, m_MomMaxTriggerTime);
    }

    public void ResetMomTimer()
    {
        m_MomTimer = 0.0f;
    }

    public float GetGameTime()
    {
        return m_GameTime;
    }

    public float GetMomComingTime()
    {
        return m_MomComingTime;
    }
}
