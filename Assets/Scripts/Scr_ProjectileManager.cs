using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_ProjectileManager : MonoBehaviour 
{
    [SerializeField] private GameObject m_Projectile;
    [SerializeField] private Transform m_FirePos;
    [SerializeField] private float m_ExplosionRadius;
    [SerializeField] private float m_ExplosionForce;

    private GameObject[] m_Players;
    private CharacterController[] m_PlayerControllers;
    private Scr_CharacterController[] m_ControllerScripts;
    private Transform[] m_PlayerTargets;
    private bool m_IsInitialized = false;

	// Use this for initialization
	void Start () 
    {
        
	}

    public void Initialize()
    {
        Scr_GameController game = GetComponent<Scr_GameController>();
        m_Players = game.GetPlayers();

        m_PlayerControllers = new CharacterController[m_Players.Length];
        m_ControllerScripts = new Scr_CharacterController[m_Players.Length];
        m_PlayerTargets = new Transform[m_Players.Length];

        for (int i = 0; i < m_Players.Length; ++i)
        {
            m_PlayerControllers[i] = m_Players[i].GetComponent<CharacterController>();
            m_ControllerScripts[i] = m_Players[i].GetComponent<Scr_CharacterController>();
            m_PlayerTargets[i] = m_Players[i].transform.Find("Target");
        }

        //Set projectile inactive
        m_Projectile.SetActive(false);

        m_IsInitialized = true;
    }

    // Update is called once per frame
    void Update () 
    {
    }

    private void SpawnProjectile()
    {
        if (!m_Projectile.activeSelf)      //Cooldown?
        {
            m_Projectile.SetActive(true);
            m_Projectile.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
            Vector3 startPos = m_FirePos.position + new Vector3(0.0f, 1.0f, 1.0f);      //Value tweaking?
            m_Projectile.transform.position = startPos;
            Scr_Projectile script = m_Projectile.GetComponent<Scr_Projectile>();
            script.Fire(GetTargetPosition());

            //Animate player and play sound
            GameObject boxPlayer = GetPlayerInBox();
            if (boxPlayer != null)
            {
                Scr_AnimationController animation = boxPlayer.GetComponent<Scr_AnimationController>();
                animation.Animate("Puke");
            }
            Scr_AudioManager.SetRandomPitch("PlayerBurp");
            Scr_AudioManager.Play("PlayerBurp");
        }
    }

    private void PukeExplode()
    {
        if (m_Projectile.activeSelf)
        {
            Vector3 explosionPos = m_Projectile.transform.position;
            GameObject boxPlayer = GetPlayerInBox();
            string boxPlayerName = (boxPlayer != null) ? boxPlayer.name : "";

            for (int i = 0; i < m_Players.Length; ++i)
            {
                if (m_Players[i].name != boxPlayerName)
                {
                    float distance = Vector3.Distance(m_Players[i].transform.position, explosionPos);

                    //Explode on player
                    if (distance <= m_ExplosionRadius)
                    {
                        Vector3 forceDir = m_Players[i].transform.position - explosionPos;
                        forceDir.Normalize();
                        m_ControllerScripts[i].AddImpact(forceDir, m_ExplosionForce * 0.5f);
                        m_Players[i].GetComponent<Scr_Combat>().SetHitBy(boxPlayer);
                        if (boxPlayer)
                            Scr_ScoreManager.UpdateScore(boxPlayer, 25);

                        //Vibration
                        Scr_Input playerInput = m_Players[i].GetComponent<Scr_Input>();
                        playerInput.VibrateDecrease(0.1f, 0.3f, 1.0f);
                    }
                }     
            }

            m_Projectile.SetActive(false);

            //Particle
            Scr_ParticleManager.SpawnParticle("Explosion", explosionPos, Quaternion.identity);

            //Sound
            Scr_AudioManager.SetRandomPitch("Explosion");
            Scr_AudioManager.Play("Explosion");
        }
    }

    private GameObject GetPlayerInBox()
    {
        foreach (GameObject player in m_Players)
        {
            Scr_PlayerStateController playerState = player.GetComponent<Scr_PlayerStateController>();

            if (playerState.PlayerState == Scr_PlayerStateController.State.BabyBox)
                return player;
        }

        return null;
    }

    private Vector3 GetTargetPosition()
    {
        Transform activeTarget = null;

        foreach (Transform target in m_PlayerTargets)
        {
            if (target.gameObject.activeInHierarchy)
            {
                activeTarget = target;
                break;
            }
        }

        RaycastHit hit;
        int layerMask = 1 << 2;     //bit shift to get layer mask (2 = index of safespots layer, the layer we want to ignore)
        layerMask = ~layerMask;     //invert bitmask

        if (Physics.Raycast(activeTarget.position, Vector3.down, out hit, Mathf.Infinity, layerMask))
        {
            return hit.point;
        }
        return Vector3.zero;
    }

    private void OnEnable()
    {
        Scr_EventManager.StartListening("Fire_Projectile", SpawnProjectile);
        Scr_EventManager.StartListening("Projectile_Hit", PukeExplode);
    }

    private void OnDisable()
    {
        Scr_EventManager.StopListening("Fire_Projectile", SpawnProjectile);
        Scr_EventManager.StopListening("Projectile_Hit", PukeExplode);
    }
}
