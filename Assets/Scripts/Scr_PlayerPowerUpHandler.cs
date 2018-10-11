using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_PlayerPowerUpHandler : MonoBehaviour
{
    [SerializeField] private float m_PowerupTimer = 10.0f;
    [SerializeField] private float m_PunchingPowerMultiplier = 2.0f;
    [SerializeField] private float m_SpeedMultiplier = 1.5f;
    [SerializeField] private int m_NrOfJumpsGivenByPowerup = 2;
    [SerializeField] private GameObject m_SpeedParticle;
    [SerializeField] private GameObject m_PowerParticle;

    private Scr_PlayerStateController m_PlayerState;
    private Scr_Input m_Input;

    private float m_OriginalPunchingPower;
    private int m_OriginalNrOfMaxJumps;
    private float m_OriginalSpeed;
    private float m_OriginalPowerupTimer;
    private bool m_MustCountDown = false;

    private Scr_PowerUp.Type m_HeldPowerup = Scr_PowerUp.Type.Empty;

    private void Awake()
    {
        m_OriginalPunchingPower = gameObject.GetComponent<Scr_Combat>().GetPunchingPower();
        m_OriginalSpeed = gameObject.GetComponent<Scr_CharacterController>().GetSpeed();
        m_OriginalNrOfMaxJumps = gameObject.GetComponent<Scr_CharacterController>().GetMaxJumps();

        m_OriginalPowerupTimer = m_PowerupTimer;

        m_PlayerState = GetComponent<Scr_PlayerStateController>();
        m_Input = GetComponent<Scr_Input>();
    }

    // Use this for initialization
    void Start ()
	{
	    
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (m_PlayerState.PlayerState != Scr_PlayerStateController.State.BabyBox && !m_PlayerState.IsMomInRoom)
        {
            if (Input.GetButtonDown(m_Input.GetUse()))
            {
                if (m_HeldPowerup != Scr_PowerUp.Type.Empty)
                {
                    switch (m_HeldPowerup)
                    {
                        case Scr_PowerUp.Type.ExtraSpeed:
                            gameObject.GetComponent<Scr_CharacterController>().SetSpeed(m_OriginalSpeed * m_SpeedMultiplier);
                            m_MustCountDown = true;
                            m_SpeedParticle.SetActive(true);
                            Debug.Log("Extra Speed Activated!");
                            break;
                        case Scr_PowerUp.Type.Knockback:
                            gameObject.GetComponent<Scr_Combat>().SetPunchingPower(m_OriginalPunchingPower * m_PunchingPowerMultiplier);
                            m_MustCountDown = true;
                            Debug.Log("Extra Punching power Activated");
                            m_PowerParticle.SetActive(true);
                            break;
                        case Scr_PowerUp.Type.MultiJump:
                            gameObject.GetComponent<Scr_CharacterController>().SetMaxJumps(m_NrOfJumpsGivenByPowerup);
                            m_MustCountDown = true;
                            Debug.Log("Multi jump Activated");
                            break;
                    }
                    //Activate powerup countdown
                }
                else
                {
                    Debug.Log("You are not holding a powerup!");
                }
            }
        }
        

        if(m_MustCountDown)
        {
            if (m_HeldPowerup != Scr_PowerUp.Type.Empty)
                m_PowerupTimer -= Time.deltaTime;

            if (m_PowerupTimer <= 0.0f)
                ResetPlayerState();
        }
    }

    public bool PickupPowerup(Scr_PowerUp.Type type)
    {
        if (m_HeldPowerup == Scr_PowerUp.Type.Empty)
        {
            m_HeldPowerup = type;
            Scr_EventManager.TriggerEvent("PowerupUpdate");
            return true;
        }

        return false;
    }

    public void ResetPlayerState()
    {
        gameObject.GetComponent<Scr_CharacterController>().SetSpeed(m_OriginalSpeed);
        gameObject.GetComponent<Scr_CharacterController>().SetMaxJumps(m_OriginalNrOfMaxJumps);
        gameObject.GetComponent<Scr_Combat>().SetPunchingPower(m_OriginalPunchingPower);
        m_PowerupTimer = m_OriginalPowerupTimer;
        m_HeldPowerup = Scr_PowerUp.Type.Empty;
        m_MustCountDown = false;

        m_SpeedParticle.SetActive(false);
        m_PowerParticle.SetActive(false);

        Scr_EventManager.TriggerEvent("PowerupUpdate");
    }

    public Scr_PowerUp.Type GetCurrentPowerup()
    {
        return m_HeldPowerup;
    }

    public float GetTotalPowerTime()
    {
        return m_OriginalPowerupTimer;
    }

    public float GetPowerupTimeLeft()
    {
        return m_PowerupTimer;
    }
}
