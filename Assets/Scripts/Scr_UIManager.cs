using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Scr_UIManager : MonoBehaviour 
{
    [SerializeField] private Image m_JoinMessage;

    [SerializeField] private GameObject[] m_PlayerPortraitsDeactived;
    [SerializeField] private GameObject[] m_PlayerPortraitsActivated;

    [SerializeField] private GameObject m_HUD;
    [SerializeField] private GameObject m_MomTimer;
    [SerializeField] private Text m_GameTime;
    [SerializeField] private GameObject m_WinScreen;
    [SerializeField] private GameObject m_UI;
    [SerializeField] private Text m_RoundTimer;
    [SerializeField] private GameObject m_ScoreBoard;
    [SerializeField] private GameObject[] m_PlayerScores;
    [SerializeField] private GameObject[] m_CountDowns;
    [SerializeField] private GameObject m_EndSplash;
    [SerializeField] private GameObject[] m_WinPlayerNames;
    [SerializeField] private GameObject[] m_WinPlayers;
    
    private GameObject[] m_Players;
    private Scr_PlayerPowerUpHandler[] m_PlayerPowers;
    private GameObject[] m_PlayerHuds;
    private Scr_Timer m_Timer;
    private bool m_IsInitialized = false;
    private float m_RoundTimeSecondFlag = 3.0f;     //Used to play countdown SFX each second

	// Use this for initialization
	void Start () 
    {
		
	}

    public void Initialize()
    {
        m_Timer = GetComponent<Scr_Timer>();

        Scr_GameController game = GetComponent<Scr_GameController>();
        m_Players = game.GetPlayers();

        m_PlayerHuds = new GameObject[m_Players.Length];
        m_PlayerPowers = new Scr_PlayerPowerUpHandler[m_Players.Length];

        for (int i = 0; i < m_Players.Length; ++i)
        {
            m_PlayerHuds[i] = m_HUD.transform.GetChild(i).gameObject;
            m_PlayerPowers[i] = m_Players[i].GetComponent<Scr_PlayerPowerUpHandler>();
        }

        m_IsInitialized = true;
    }

    // Update is called once per frame
    void Update () 
    {
        if (m_IsInitialized)
        {
            UpdateGameTime();
            UpdatePowerups();
            UpdateMomTimer();
        }
    }

    private void UpdateGameTime()
    {
        float time = m_Timer.GetGameTime();
        string minutes = Mathf.Floor(time / 60).ToString("00");
        string seconds = Mathf.Floor(time % 60).ToString("00");

        if (time <= 0.0f)
        {
            minutes = "00";
            seconds = "00";
        }

        m_GameTime.text = minutes + ":" + seconds;
    }

    private void UpdatePowerups()
    {
        for (int i = 0; i < m_Players.Length; ++i)
        {
            if (m_PlayerPowers[i].GetCurrentPowerup() != Scr_PowerUp.Type.Empty)
            {
                float totalTime = m_PlayerPowers[i].GetTotalPowerTime();
                float timeLeft = m_PlayerPowers[i].GetPowerupTimeLeft();
                float scale = timeLeft / totalTime;

                GameObject timeObject = m_PlayerHuds[i].transform.Find("TimerValue").gameObject;
                RectTransform timeRect = timeObject.GetComponent<RectTransform>();
                Image timeImage = timeObject.GetComponent<Image>();

                timeRect.localScale = new Vector3(scale, 1, 1);
                if (scale < 0.5f)
                    timeImage.color = Color.yellow;

                if (scale < 0.2f)
                    timeImage.color = Color.red;

                //Hide icons
                if (timeLeft < totalTime)
                {
                    m_PlayerHuds[i].transform.Find("PressY").gameObject.SetActive(false);
                    m_PlayerHuds[i].transform.Find("PowerupJump").gameObject.SetActive(false);
                    m_PlayerHuds[i].transform.Find("PowerupKnockback").gameObject.SetActive(false);
                    m_PlayerHuds[i].transform.Find("PowerupSpeed").gameObject.SetActive(false);
                }
            }
        }
    }

    private void UpdateMomTimer()
    {
        if (m_MomTimer.activeSelf)
        {
            //float totalValue = 250.0f - 6.2f;
            float width = 1017.0f;
            float momStayTime = m_Timer.GetMomComingTime();

            float speed = width / momStayTime;
            GameObject momTimer = m_MomTimer.transform.Find("Value").gameObject;
            RectTransform momRect = momTimer.GetComponent<RectTransform>();

            float newWidth = momRect.rect.width - speed * Time.deltaTime;
            momRect.sizeDelta = new Vector2(newWidth, momRect.rect.height);
        }
    }

    private void UpdateScoreUI()
    {
        for (int i = 0; i < m_Players.Length; ++i)
        {
            int score = Scr_ScoreManager.GetScore(m_Players[i].name);

            if (score >= 0)
            {
                GameObject scoreObject = m_PlayerHuds[i].transform.Find("Score").gameObject;
                Text playerScore = scoreObject.GetComponent<Text>();

                playerScore.text = score.ToString();
            }
        }
    }

    private void UpdatePowerupIcons()
    {
        if (m_Players == null)
            return;

        for (int i = 0; i < m_Players.Length; ++i)
        {
            Scr_PowerUp.Type heldPowerup = m_PlayerPowers[i].GetCurrentPowerup();

            switch (heldPowerup)
            {
                case Scr_PowerUp.Type.ExtraSpeed:
                    SetPowerupIcon(i, "PowerupSpeed", true);
                    break;
                case Scr_PowerUp.Type.Knockback:
                    SetPowerupIcon(i, "PowerupKnockback", true);
                    break;
                case Scr_PowerUp.Type.MultiJump:
                    SetPowerupIcon(i, "PowerupJump", true);
                    break;
                default:
                    SetPowerupIcon(i, "", false);
                    break;
            }
        }
    }

    KeyValuePair<string, int> GetWinner()
    {
        List<KeyValuePair<string, int>> scores = Scr_ScoreManager.GetScores().ToList();

        var items = from pair in scores
                    orderby pair.Value descending
                    select pair;

        if (scores.Any())
            return items.ElementAt(0);
        else
            return new KeyValuePair<string, int>(); //empty fallback
    }
    
    private void ShowWinScreen()
    {
        m_WinScreen.SetActive(true);
        KeyValuePair<string, int> winner = GetWinner();

        //Show winners
        for (int i = 0; i < m_WinPlayerNames.Length; ++i)
        {
            m_WinPlayerNames[i].SetActive(false);
            m_WinPlayers[i].SetActive(false);
        }
        ShowWinner(winner.Key);

        //Disable other UI elements
        for (int i = 0; i < m_Players.Length; ++i)
        {
            m_PlayerHuds[i].SetActive(false);
        }

        m_GameTime.enabled = false;
    }

    private void ShowWinner(string name)
    {
        //I would like to sincerely apologize for this piece of garbage code
        switch (name)
        {
            case "Charles":
                m_WinPlayerNames[0].SetActive(true);
                m_WinPlayers[0].SetActive(true);
                break;
            case "Juan":
                m_WinPlayerNames[1].SetActive(true);
                m_WinPlayers[1].SetActive(true);
                break;
            case "Chiquita":
                m_WinPlayerNames[2].SetActive(true);
                m_WinPlayers[2].SetActive(true);
                break;
            case "Sanchez":
                m_WinPlayerNames[3].SetActive(true);
                m_WinPlayers[3].SetActive(true);
                break;
        }
    }

    public void ShowPlayerJoined(int index)
    {
        m_PlayerPortraitsDeactived[index].SetActive(false);
        m_PlayerPortraitsActivated[index].SetActive(true);
    }

    public void ShowGameHUD()
    {
        for (int i = 0; i < m_Players.Length; ++i)
        {
            m_PlayerHuds[i].SetActive(true);
        }

        m_UI.transform.Find("HUD").gameObject.SetActive(true);
        m_UI.transform.Find("Timer").gameObject.SetActive(true);
        m_GameTime.transform.parent.gameObject.SetActive(true);
        m_GameTime.enabled = true;
        m_JoinMessage.enabled = false;
        m_UI.transform.Find("JoinScreen").gameObject.SetActive(false);
    }

    private void SetPowerupIcon(int playerIndex, string powerupName, bool fillBar)
    {
        //Hardcoded mess, refactorrrrrr
        GameObject jumpObject = m_PlayerHuds[playerIndex].transform.Find("PowerupJump").gameObject;
        jumpObject.SetActive(false);
        GameObject knockbackObject = m_PlayerHuds[playerIndex].transform.Find("PowerupKnockback").gameObject;
        knockbackObject.SetActive(false);
        GameObject speedObject = m_PlayerHuds[playerIndex].transform.Find("PowerupSpeed").gameObject;
        speedObject.SetActive(false);
        
        
        if (powerupName.Length > 0)
        {
            GameObject iconObject = m_PlayerHuds[playerIndex].transform.Find(powerupName).gameObject;
            iconObject.SetActive(true);
        }

        float scale = (fillBar) ? 1 : 0;
        GameObject timeObject = m_PlayerHuds[playerIndex].transform.Find("TimerValue").gameObject;
        RectTransform timeRect = timeObject.GetComponent<RectTransform>();
        Image timeImage = timeObject.GetComponent<Image>();
        timeRect.localScale = new Vector3(scale, 1, 1);
        timeImage.color = Color.green;

        if (fillBar)
            m_PlayerHuds[playerIndex].transform.Find("PressY").gameObject.SetActive(true);
        else
            m_PlayerHuds[playerIndex].transform.Find("PressY").gameObject.SetActive(false);

    }

    private void ShowMomTimer()
    {
        m_MomTimer.SetActive(true);
    }

    private void ResetMomTimer()
    {
        GameObject momTimer = m_MomTimer.transform.Find("Value").gameObject;
        RectTransform momRect = momTimer.GetComponent<RectTransform>();
        momRect.sizeDelta  = new Vector2(1017.0f, momRect.rect.height);

        m_MomTimer.SetActive(false);
    }

    public void GameOver()
    {
        ShowWinScreen();
    }

    public void ShowScoreBoard()
    {
        m_WinScreen.SetActive(false);
        m_ScoreBoard.SetActive(true);

        List<KeyValuePair<string, int>> scores = Scr_ScoreManager.GetScores().ToList();

        var items = from pair in scores
            orderby pair.Value descending
            select pair;

        for (int i = 0; i < items.Count(); ++i)
        {
            m_PlayerScores[i].SetActive(true);
            Text name = m_PlayerScores[i].transform.Find("Name").GetComponent<Text>();
            Text score = m_PlayerScores[i].transform.Find("Score").GetComponent<Text>();

            name.text = items.ElementAt(i).Key;
            score.text = items.ElementAt(i).Value.ToString();
        }

        //Text firstScore = m_ScoreBoard.transform.Find("First Score").gameObject.GetComponent<Text>();
        //Text secondScore = m_ScoreBoard.transform.Find("Second Score").gameObject.GetComponent<Text>();
        //Text thirdScore = m_ScoreBoard.transform.Find("Third Score").gameObject.GetComponent<Text>();
        //Text fourthScore = m_ScoreBoard.transform.Find("Fourth Score").gameObject.GetComponent<Text>();

        //List<Text> scoresText = new List<Text>();
        //scoresText.Add(firstScore);
        //scoresText.Add(secondScore);
        //scoresText.Add(thirdScore);
        //scoresText.Add(fourthScore);
        
        ////firstScore.text = items.ElementAt(0).Key + "   " + items.ElementAt(0).Value;

        //for (int i = 0; i < m_Players.Length; ++i)
        //{
        //    scoresText[i].enabled = true;

        //    scoresText[i].text = items.ElementAt(i).Key + "    " + items.ElementAt(i).Value;
        //}

        //Disable input temporarily
        m_UI.GetComponent<Scr_PauseMenu>().SetIsGameFinished(true);
    }

    public void UpdateRoundTimer(float time)
    {
        float ceiledTime = Mathf.Ceil(time);
        m_RoundTimer.text = ceiledTime.ToString();

        //End of countdown
        if (time <= 0.0f && m_RoundTimer.enabled)
        {
            m_RoundTimer.enabled = false;
            Scr_AudioManager.Play("BoxRing");
            foreach (GameObject countDown in m_CountDowns)
            {
                countDown.gameObject.SetActive(false);   
            }
        }

        //Play sound
        if (time <= m_RoundTimeSecondFlag && m_RoundTimer.enabled)
        {
            Scr_AudioManager.Play("Countdown");
            m_RoundTimeSecondFlag -= 1.0f;
            
            if((int)m_RoundTimeSecondFlag + 1 < m_CountDowns.Length)
                m_CountDowns[(int)m_RoundTimeSecondFlag + 1].SetActive(false);      //FIX
            m_CountDowns[(int)m_RoundTimeSecondFlag].SetActive(true);
        }
    }

    public void ShowEndSplash(bool value)
    {
        m_EndSplash.SetActive(value);
    }

    private void OnEnable()
    {
        Scr_EventManager.StartListening("Update_Scores", UpdateScoreUI);
        Scr_EventManager.StartListening("PowerupUpdate", UpdatePowerupIcons);

        Scr_EventManager.StartListening("Mom_Comes", ShowMomTimer);
        Scr_EventManager.StartListening("Mom_In_Room", ResetMomTimer);
    }

    private void OnDisable()
    {
        Scr_EventManager.StopListening("Update_Scores", UpdateScoreUI);
        Scr_EventManager.StopListening("PowerupUpdate", UpdatePowerupIcons);

        Scr_EventManager.StopListening("Mom_Comes", ShowMomTimer);
        Scr_EventManager.StopListening("Mom_In_Room", ResetMomTimer);
    }
}
