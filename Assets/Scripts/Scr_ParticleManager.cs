using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Scr_ParticleManager : MonoBehaviour 
{
    [SerializeField] private Scr_Particle[] m_ParticlePrefabs;

    private static Scr_ParticleManager m_ParticleManager;
    private List<Scr_Particle> m_ActiveParticles;
    private bool m_IsInitialized = false;

    public static Scr_ParticleManager Instance
    {
        get
        {
            if (!m_ParticleManager)
            {
                m_ParticleManager = FindObjectOfType(typeof(Scr_ParticleManager)) as Scr_ParticleManager;

                if (!m_ParticleManager)
                    Debug.Log("No active ParticleManager script found in the current scene");
                else
                    m_ParticleManager.Init();
            }
            return m_ParticleManager;
        }
    }

    private void Init()
    {
        m_ActiveParticles = new List<Scr_Particle>();
        m_IsInitialized = true;
    }

    private void Update () 
    {
        if (m_IsInitialized)
        {
            for (int i = m_ActiveParticles.Count - 1; i >= 0; --i)
            {
                if (m_ActiveParticles.ElementAt(i).Particle.isStopped)
                {
                    Destroy(m_ActiveParticles.ElementAt(i).Particle.gameObject);
                    m_ActiveParticles.RemoveAt(i);
                }
            }
        }
    }

    public static void SpawnParticle(string name, Vector3 position, Quaternion rotation)
    {
        Scr_Particle particle = Instance.GetParticle(name);

        if (particle != null)
        {
            ParticleSystem spawned = Instantiate(particle.Particle, position, rotation);
            Instance.m_ActiveParticles.Add(new Scr_Particle(name, spawned));
        }
    }

    private Scr_Particle GetParticle(string name)
    {
        Scr_Particle particle = Array.Find(m_ParticlePrefabs, p => p.Name == name);

        if (particle == null)
            Debug.LogWarning("Particle with name " + name + " could not be found");

        return particle;
    }
}
