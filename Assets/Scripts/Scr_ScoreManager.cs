using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_ScoreManager : MonoBehaviour {

    private Dictionary<string, int> m_ScoreDictionary;
    private static Scr_ScoreManager m_ScoreManager;

    public static Scr_ScoreManager Instance
    {
        get
        {
            if (!m_ScoreManager)
            {
                m_ScoreManager = FindObjectOfType(typeof(Scr_ScoreManager)) as Scr_ScoreManager;

                if (!m_ScoreManager)
                    Debug.Log("No active ScoreManager script found in the current scene");
                else
                    m_ScoreManager.Init();
            }
            return m_ScoreManager;
        }
    }

    private void Init()
    {
        if (m_ScoreDictionary == null)
            m_ScoreDictionary = new Dictionary<string, int>();
    }

    public static void UpdateScore(GameObject player, int amount)
    {
        int currentScore = 0;

        if (Instance.m_ScoreDictionary.TryGetValue(player.name, out currentScore))
        {
            currentScore += amount;
            if (currentScore < 0) currentScore = 0;
            
            Instance.m_ScoreDictionary[player.name] = currentScore;
        }
        else
        {
            if (amount < 0) amount = 0;
            Instance.m_ScoreDictionary.Add(player.name, amount);
        }

        Scr_EventManager.TriggerEvent("Update_Scores");
    }

    public static int GetScore(string key)
    {
        int score = 0;

        if (Instance.m_ScoreDictionary.TryGetValue(key, out score))
            return score;
        else
            return -1;
    }

    public static Dictionary<string, int> GetScores()
    {
        return Instance.m_ScoreDictionary;
    }
}
