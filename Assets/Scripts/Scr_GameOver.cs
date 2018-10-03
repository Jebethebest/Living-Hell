using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_GameOver : MonoBehaviour
{
    [SerializeField] private GameObject m_UI;
    
    private void OnEnable()
    {
        Scr_EventManager.StartListening("GameOver", ShowGameOver);
    }

    private void OnDisable()
    {
        Scr_EventManager.StopListening("GameOver", ShowGameOver);
    }

    private void ShowGameOver()
    {
        Time.timeScale = 0.0f;
        m_UI.transform.Find("EndScores").gameObject.SetActive(true);
        m_UI.transform.Find("EndScores").gameObject.GetComponent<Scr_PauseMenu>().SetIsGameFinished(true);
    }

    // Use this for initialization
    void Start () 
    {
		//Nothing to do
	}
	
	// Update is called once per frame
	void Update () 
    {
		//Nothing to do
	}
}
