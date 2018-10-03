﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_PowerupSpawner : MonoBehaviour
{
    //[SerializeField] private Vector3 m_Center;
    //[SerializeField] private Vector3 m_Size;

    [SerializeField] private float m_PowerupTimerMin = 5.0f;
    [SerializeField] private float m_PowerupTimerMax = 10.0f;

    [SerializeField] private GameObject[] m_Powerups;
    [SerializeField] private Transform[] m_SpawnPoints;

    private float m_SpawnTimer;
 
	// Use this for initialization
	void Start ()
	{
	    //SpawnPowerup();
	    SetRandomPowerupTimer();
	}
	
	// Update is called once per frame
	void Update ()
	{
	    if (m_SpawnTimer >= 0.0f)
	        m_SpawnTimer -= Time.deltaTime;

	    if (m_SpawnTimer <= 0.0f)
	    {
	        SpawnPowerup();
	        SetRandomPowerupTimer();
	    }
    }

    //void OnDrawGizmosSelected ()
    //{
    //    Gizmos.color = new Color(1, 0, 0, 0.5f);
    //    Gizmos.DrawCube(transform.localPosition + m_Center, m_Size);
    //}

    private void SpawnPowerup ()
    {
        //Vector3 position = m_Center + new Vector3(Random.Range(-m_Size.x / 2, m_Size.x / 2), 
        //                                          Random.Range(-m_Size.y / 2, m_Size.y / 2), 
        //                                          Random.Range(-m_Size.z / 2, m_Size.z / 2));
        //Instantiate(m_Powerup, position, Quaternion.identity);

        int powerIndex = Random.Range(0, m_Powerups.Length);
        int spawnIndex = Random.Range(0, m_SpawnPoints.Length);
        Vector3 spawnPosition = m_SpawnPoints[spawnIndex].position;

        Collider[] hitColliders = Physics.OverlapSphere(spawnPosition, 1.0f);

        foreach (Collider collider in hitColliders)
        {
            if (collider.gameObject.GetComponent<Scr_PowerUp>() != null)
            {
                Debug.Log("A powerup has already spawned here!");
                return;
            }
        }

        Debug.Log("No powerup found, spawning powerup");
        Instantiate(m_Powerups[powerIndex], spawnPosition, Quaternion.identity);
        print(m_Powerups[powerIndex].tag);
    }

    private void SetRandomPowerupTimer()
    {
        m_SpawnTimer = Random.Range(m_PowerupTimerMin, m_PowerupTimerMax);
    }
}
