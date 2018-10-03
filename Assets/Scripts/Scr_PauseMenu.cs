using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Scr_PauseMenu : MonoBehaviour
{
    public EventSystem m_MyEventSystem;
    
    [SerializeField] private static bool m_IsGamePaused = false;
    [SerializeField] private GameObject m_PauseMenuUI;
    [SerializeField] private GameObject m_ResumeButton;
    [SerializeField] private GameObject m_MenuButton;

    private Scr_GameController m_Game;
    
    private bool m_CanPauseMenuOpen = false;
    private bool m_IsGameFinished = false;

    private bool m_IsInputDisabled = true;
    private float m_InputDisableTimer = 0.0f;

    // Use this for initialization
    void Awake ()
	{
        GameObject mainGame = GameObject.Find("MainGame");
        m_Game = mainGame.GetComponent<Scr_GameController>();
	}
	
	// Update is called once per frame
	void Update ()
	{
	    if (m_CanPauseMenuOpen && !m_IsGameFinished)
	    {
	        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.JoystickButton7))
	        {
	            m_MyEventSystem.SetSelectedGameObject(m_ResumeButton);

                if (m_IsGamePaused)
	                Resume();
	            else
	                Pause();
	        }
        }

        else if (m_IsGameFinished)
        {
            DisableInput();
        }
	}

    public void Resume()
    {
        m_Game.PauseGame(false);
        m_PauseMenuUI.SetActive(false);
        //Time.timeScale = 1.0f;
        m_IsGamePaused = false;
    }

    public void Pause()
    {
        m_Game.PauseGame(true);
        m_PauseMenuUI.SetActive(true);
        //Time.timeScale = 0.0f;
        m_IsGamePaused = true;
    }

    public void LoadMenu()
    {
        Time.timeScale = 1.0f;
        Scr_AudioManager.Stop("WinScreen");
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting the game!");
        Application.Quit();
    }

    public void SetCanPauseMenuOpen(bool canPauseMenuOpen)
    {
        m_CanPauseMenuOpen = canPauseMenuOpen;
    }

    public void SetIsGameFinished(bool isGameFinished)
    {
        m_IsGameFinished = isGameFinished;
        //SetSelectedButton();
    }

    public bool GetIsGameFinished()
    {
        return m_IsGameFinished;
    }

    public void SetSelectedButton()
    {
        m_MyEventSystem.SetSelectedGameObject(m_MenuButton);
    }

    private void DisableInput()
    {
        if (m_IsInputDisabled)
        {
            m_MyEventSystem.enabled = false;

            m_InputDisableTimer += Time.deltaTime;

            if (m_InputDisableTimer >= 3.0f)
            {
                m_MyEventSystem.enabled = true;
                m_IsInputDisabled = false;
                m_InputDisableTimer = 0.0f;
                m_MyEventSystem.SetSelectedGameObject(m_MenuButton);
            }
        }
    }
}
