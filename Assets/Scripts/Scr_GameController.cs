using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using System;
using XInputDotNetPure;

public class Scr_GameController : MonoBehaviour 
{
    private class PlayerStart
    {
        public PlayerStart(string start, string suffix)
        {
            button = start;
            inputSuffix = suffix;
        }

        public string button;
        public string inputSuffix;
    }

    [SerializeField] private GameObject[] m_Players;
    [SerializeField] private GameObject m_PowerupSpawner;
    [SerializeField] private GameObject m_UI;
    //[SerializeField] private GameObject[] m_EndParticles;
    [SerializeField] private ParticleSystem m_ConfettiParticle;
    
    private Scr_ProjectileManager m_ProjManager;
    private Scr_UIManager m_UIManager;
    private Scr_Timer m_Timer;
    private Scr_CheckRoom m_MomScript;

    private Scr_Input[] m_PlayerInput;
    private bool m_InitPlayers = false;
    private int m_InitPlayerIndex = 0;
    private PlayerStart[] m_StartButtons;
    private List<int> m_JoinedPlayerIndexes;
    private int m_NrPlayers = 4;

    private bool m_GameStarted = false;
    private bool m_GameEnd = false;
    private bool m_ShouldRunScoreBoardTimer = false;
    
    [SerializeField] private float m_RoundStartTimer = 4.0f;
    [SerializeField] private float m_RoundEndTimer = 3.0f;
    [SerializeField] private float m_ScoreBoardTimer = 10.0f;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    // Use this for initialization
    void Awake ()
    {
		m_Timer = GetComponent<Scr_Timer>();
        m_UIManager = GetComponent<Scr_UIManager>();
        m_ProjManager = GetComponent<Scr_ProjectileManager>();
        m_MomScript = GetComponent<Scr_CheckRoom>();
    }

    void Start()
    {
        m_PlayerInput = new Scr_Input[m_Players.Length];

        for (int i = 0; i < m_Players.Length; ++i)
        {
            m_PlayerInput[i] = m_Players[i].GetComponent<Scr_Input>();
        }

        m_JoinedPlayerIndexes = new List<int>();

        m_StartButtons = new PlayerStart[] { new PlayerStart("Start_P1", "P1"),
                                             new PlayerStart("Start_P2", "P2"),
                                             new PlayerStart("Start_P3", "P3"),
                                             new PlayerStart("Start_P4", "P4") };
    }

    // Update is called once per frame
    void Update () 
    {
		if (m_Timer.GetGameTime() < 1.0f && m_Timer.isActiveAndEnabled) 
        {
            EnableGameObjects(false);
            m_GameEnd = true;
            m_UIManager.ShowEndSplash(true);
            Scr_AudioManager.Stop("CombatTheme");
            Scr_AudioManager.Play("RoundEnd");
        }

        if (m_InitPlayers)
            InitializePlayers();


        if (!m_InitPlayers && !m_GameStarted)
        {
            InitializeRound();
            m_UIManager.UpdateRoundTimer(m_RoundStartTimer - 1.0f);
        }

        if (m_GameEnd)
        {
            m_RoundEndTimer -= Time.deltaTime;

            if (m_RoundEndTimer <= 0.0f)
            {
                m_UIManager.GameOver();
                m_GameEnd = false;
                m_UIManager.ShowEndSplash(false);
                Scr_AudioManager.Play("WinScreen");
                m_ConfettiParticle.Play();
                //PlayWinParticles();
                Debug.Log("Game end");
                m_ShouldRunScoreBoardTimer = true;

            }
        }

        if (m_ShouldRunScoreBoardTimer)
        {
            m_ScoreBoardTimer -= Time.deltaTime;

            if (m_ScoreBoardTimer <= 0.0f)
            {
                m_UIManager.ShowScoreBoard();
                m_ShouldRunScoreBoardTimer = false;
            }
            Debug.Log(m_ScoreBoardTimer);
        }
	}

    private void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        PauseGame(true);
        m_InitPlayers = true;
    }

    private void LoadGame()
    {
        if (m_InitPlayers)
            DisableUnboundActors();

        m_UIManager.Initialize();
        m_ProjManager.Initialize();
        m_MomScript.Initialize();

        m_UI.GetComponent<Scr_PauseMenu>().SetCanPauseMenuOpen(true);
        m_InitPlayers = false;
        m_UIManager.ShowGameHUD();
        m_UI.transform.Find("JoinScreen").gameObject.SetActive(false);
        m_UI.transform.Find("HUD").gameObject.SetActive(true);

        foreach (GameObject player in m_Players)
        {
            Scr_CharacterController controller = player.GetComponent<Scr_CharacterController>();
            controller.Respawn();
        }
    }

    public void PauseGame(bool pause)
    {
        EnableGameObjects(!pause);
        //Scr_EventManager.TriggerEvent("PauseGame");
    }

    private void EnableGameObjects(bool value)
    {
        foreach (GameObject player in m_Players)
        {
            Scr_CharacterController controller = player.GetComponent<Scr_CharacterController>();
            Scr_Combat combat = player.GetComponent<Scr_Combat>();
            Scr_PlayerPowerUpHandler powerup = player.GetComponent<Scr_PlayerPowerUpHandler>();
            Scr_AnimationController animation = player.GetComponent<Scr_AnimationController>();

            controller.enabled = value;
            combat.enabled = value;
            powerup.enabled = value;
            animation.Enable(value);
        }

        m_Timer.enabled = value;
        Scr_PowerupSpawner powerupSpawner = m_PowerupSpawner.GetComponent<Scr_PowerupSpawner>();
        powerupSpawner.enabled = value;
    }

    private void InitializePlayers()
    {
        //Init player controllers
        if (m_InitPlayerIndex < m_Players.Length)
        {
            //for (int i = 0; i < m_StartButtons.Length; ++i)
            //{
            //    if (Input.GetButtonDown(m_StartButtons[i].button))
            //    {
            //        m_UIManager.ShowPlayerJoined(m_InitPlayerIndex);
            //        m_PlayerInput[m_InitPlayerIndex].SetInput(m_StartButtons[i].inputSuffix);
            //        m_PlayerInput[m_InitPlayerIndex].SetGamepadIndex((PlayerIndex)m_InitPlayerIndex);

            //        List<PlayerStart> temp = new List<PlayerStart>(m_StartButtons);
            //        temp.RemoveAt(i);
            //        m_StartButtons = temp.ToArray();
            //        ++m_InitPlayerIndex;
            //        break;
            //    }
            //}

            for (int i = 0; i < 4; ++i)
            {
                if (Input.GetKeyDown("joystick " + (i + 1) + " button 7"))
                {
                    if (!m_JoinedPlayerIndexes.Contains(i))
                    {
                        Debug.Log("Controller " + i + " pressed start!");

                        m_UIManager.ShowPlayerJoined(m_InitPlayerIndex);
                        m_PlayerInput[m_InitPlayerIndex].SetInput("P" + (i + 1));
                        m_PlayerInput[m_InitPlayerIndex].SetGamepadIndex((PlayerIndex)i);
                        m_JoinedPlayerIndexes.Add(i);
                        m_PlayerInput[m_InitPlayerIndex].Vibrate(0.2f, 0.5f);

                        //List<PlayerStart> temp = new List<PlayerStart>(m_StartButtons);
                        //temp.RemoveAt(i);
                        //m_StartButtons = temp.ToArray();
                        ++m_InitPlayerIndex;
                        break;
                    }               
                }
            }
        }

        //Start game
        foreach (Scr_Input input in m_PlayerInput)
        {
            if (input.IsSet() && Input.GetButtonDown(input.GetJump()))
            {
                LoadGame();
                m_InitPlayers = false;
                Scr_AudioManager.Stop("MainMenuTheme");
            }
        }
    }

    private void InitializeRound()
    {
        m_RoundStartTimer -= Time.deltaTime;

        if (m_RoundStartTimer <= 0.0f)
        {
            EnableGameObjects(true);
            Scr_AudioManager.Play("CombatTheme");
            m_GameStarted = true;
        }
    }

    private void DisableUnboundActors()
    {
        for (int i = 0; i < m_PlayerInput.Length; ++i)
        {
            if (!m_PlayerInput[i].IsSet())
            {
                m_Players[i].SetActive(false);
                --m_NrPlayers;
            }
        }
    }

    //private void PlayWinParticles()
    //{
    //    foreach (GameObject particle in m_EndParticles)
    //    {
    //        ForkParticlePlugin plugin = GetComponent<ForkParticlePlugin>();
    //        plugin.AddEffect(particle);
    //        particle.GetComponent<ForkParticleEffect>().PlayEffect();
    //    }
    //}

    public int GetNrPlayers()
    {
        return m_NrPlayers;
    }

    public GameObject[] GetPlayers()
    {
        GameObject[] players = new GameObject[m_NrPlayers];
        Array.Copy(m_Players, 0, players, 0, m_NrPlayers);

        return players;
    }
}
