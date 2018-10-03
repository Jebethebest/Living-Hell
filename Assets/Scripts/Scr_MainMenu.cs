using System.Collections;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Scr_MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject m_MainMenu;
    [SerializeField] private GameObject m_OptionsMenu;
    [SerializeField] private GameObject m_HelpMenu;

    public EventSystem m_MyEventSystem;

    public Dropdown m_ResolutionDropDown;
    
    private GameObject m_StoreSelected;

    private Resolution[] m_MyResolutions;
    
    void Start()
    {
        m_StoreSelected = m_MyEventSystem.firstSelectedGameObject;

        m_MyResolutions = Screen.resolutions;
        
        m_ResolutionDropDown.ClearOptions();
        
        List<string> options = new List<string>();

        int currentResolution = 0;
        
        for (int i = 0; i < m_MyResolutions.Length; ++i)
        {
            string option = m_MyResolutions[i].width + " x " + m_MyResolutions[i].height;
            options.Add(option);

            if (m_MyResolutions[i].width == Screen.currentResolution.width &&
                m_MyResolutions[i].height == Screen.currentResolution.height)
                    currentResolution = i;
        }
        m_ResolutionDropDown.AddOptions(options);
        m_ResolutionDropDown.value = currentResolution;
        m_ResolutionDropDown.RefreshShownValue();

        //Song
        Scr_AudioManager.Play("MainMenuTheme");
    }

    void Update()
    {
        if (m_StoreSelected == null)
        {
            m_StoreSelected = m_MyEventSystem.firstSelectedGameObject;
        }
        if (m_MyEventSystem.currentSelectedGameObject != m_StoreSelected)
        {
            if (m_MyEventSystem == null)
                m_MyEventSystem.SetSelectedGameObject(m_StoreSelected);

            else
                m_StoreSelected = m_MyEventSystem.currentSelectedGameObject;
        }
        
        //Debug.Log(m_StoreSelected.name);

        if (Input.GetKeyDown(KeyCode.JoystickButton1))
        {
            if (m_OptionsMenu.activeSelf)
            {
                m_MainMenu.SetActive(true);
                m_OptionsMenu.SetActive(false);
                m_MyEventSystem.SetSelectedGameObject(m_MainMenu.transform.Find("Options Button").gameObject);
            }
            
            else if (m_HelpMenu.activeSelf)
            {
                m_HelpMenu.SetActive(false);
                m_MainMenu.SetActive(true);
                m_MyEventSystem.SetSelectedGameObject(m_MainMenu.transform.Find("Help Button").gameObject);
            }
        }
    }

    public void PlayGame()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;

        if (isFullscreen)
            Screen.fullScreenMode = FullScreenMode.Windowed;

        else
            Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
        
    }

    public void SetResolution(int resolutionIndex)
    {
        Screen.SetResolution(m_MyResolutions[resolutionIndex].width, m_MyResolutions[resolutionIndex].height, Screen.fullScreen);
    }
}
