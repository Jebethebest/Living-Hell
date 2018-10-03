using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_CheckRoom : MonoBehaviour 
{
    [SerializeField] private GameObject m_BabyBox;

    private GameObject[] m_Players;
    private Scr_PlayerStateController[] m_PlayerStates;
    private Scr_CharacterController[] m_PlayerMovement;
    private Scr_Combat[] m_PlayerCombat;
    private Scr_Timer m_Timer;

    private int m_PlayerInBox = -1;
    private bool m_IsSeeking = false;
    private Vector3 m_MomPos;
    private bool m_IsInitialized = false;

    // Use this for initialization
    private void Start () 
    {
        
	}

    public void Initialize()
    {
        Scr_GameController game = GetComponent<Scr_GameController>();
        m_Players = game.GetPlayers();

        m_Timer = GetComponent<Scr_Timer>();

        m_PlayerStates = new Scr_PlayerStateController[m_Players.Length];
        m_PlayerMovement = new Scr_CharacterController[m_Players.Length];
        m_PlayerCombat = new Scr_Combat[m_Players.Length];

        for (int i = 0; i < m_Players.Length; ++i)
        {
            m_PlayerStates[i] = m_Players[i].GetComponent<Scr_PlayerStateController>();
            m_PlayerMovement[i] = m_Players[i].GetComponent<Scr_CharacterController>();
            m_PlayerCombat[i] = m_Players[i].GetComponent<Scr_Combat>();
        }

        m_IsInitialized = true;
    }

    // Update is called once per frame
    private void Update () 
    {
        if (m_IsInitialized)
        {
            if (m_IsSeeking)
                SeekClosestPlayer();
        }	
	}

    public void SetIsSeeking(bool value, Vector3 momPos)
    {
        m_IsSeeking = value;
        m_MomPos = momPos;
    }

    public void GetPlayerOutOfBox()
    {
        if (m_PlayerInBox > -1)
        {
            m_PlayerMovement[m_PlayerInBox].Respawn();
            Scr_AnimationController anim = m_Players[m_PlayerInBox].GetComponent<Scr_AnimationController>();
            anim.Animate("InBox", false);
            m_PlayerInBox = -1;
        }
    }

    private void SeekClosestPlayer()
    {
        Vector3 direction;
        RaycastHit hitInfo;
        float shortestDistance = float.MaxValue;
        int layerMask = ~(1 << 2);     //Ignore layer 2 (safezone layer)

        for (int i = 0; i < m_Players.Length; ++i)
        {
            direction = (m_Players[i].transform.position - m_MomPos).normalized;

            if (m_PlayerStates[i].PlayerState == Scr_PlayerStateController.State.Unsafe) 
            {
                if (Physics.Raycast(m_MomPos, direction, out hitInfo, Mathf.Infinity, layerMask))
                {
                    //Player is seen
                    if (hitInfo.collider == m_PlayerMovement[i].GetComponent<Collider>())
                    {
                        if (hitInfo.distance < shortestDistance)
                        {
                            shortestDistance = hitInfo.distance;
                            m_PlayerInBox = i;
                        }
                    }
                }
            }
        }
        
        PunishPlayer(m_PlayerInBox);
    }

    private void PunishPlayer(int index)
    {
        if (index > -1)
        {
            if (m_PlayerStates[index].IsHoldingObject)
                m_PlayerCombat[index].ReleaseHeldObject();

            m_PlayerMovement[index].GetInBox(m_BabyBox.transform.position);
            m_IsSeeking = false;
            m_Timer.ResetMomTimer();
            Scr_ScoreManager.UpdateScore(m_Players[index], -200);

            Scr_AnimationController anim = m_Players[index].GetComponent<Scr_AnimationController>();
            anim.Animate("InBox", true);

            Scr_EventManager.TriggerEvent("PlayerPunish");

            Debug.Log("MomPos: " + m_MomPos.z);
            Debug.Log("Closest player: " + index);
        }
    }
}
