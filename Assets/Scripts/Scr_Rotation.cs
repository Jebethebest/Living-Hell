using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_Rotation : MonoBehaviour 
{
    [SerializeField] private float m_RotationSpeed;
	
	// Update is called once per frame
	void Update () 
    {
        transform.Rotate(0, m_RotationSpeed * Time.deltaTime, 0);
    }
}
