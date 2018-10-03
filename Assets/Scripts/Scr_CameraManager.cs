using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_CameraManager : MonoBehaviour 
{
    [SerializeField] private Camera m_MainCamera;
   // [SerializeField] private Camera m_StartCamera;
   // [SerializeField] private Camera m_EndCamera;

    public void StartGame()
    {
        m_MainCamera.enabled = true;
    }

    //public void GameOver()
    //{
    //    Enable(m_EndCamera);
    //}

    //private void DisableAll()
    //{
    //    m_MainCamera.enabled = false;
    //    m_StartCamera.enabled = false;
    //    m_EndCamera.enabled = false;
    //}

    //private void Enable(Camera cam)
    //{
    //    DisableAll();
    //    cam.enabled = true;
    //}
}
