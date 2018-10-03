using UnityEngine.Audio;
using System;
using UnityEngine;

public class Scr_AudioManager : MonoBehaviour 
{
    [SerializeField] private Scr_Sound[] m_Sounds;
    private static Scr_AudioManager m_AudioManager;

    private float m_MinPitch = 0.9f;
    private float m_MaxPitch = 1.1f;

    public static Scr_AudioManager Instance
    {
        get
        {
            if (!m_AudioManager)
            {
                m_AudioManager = FindObjectOfType(typeof(Scr_AudioManager)) as Scr_AudioManager;

                if (!m_AudioManager)
                    Debug.Log("No active AudioManager script found in the current scene");
                else
                    m_AudioManager.Init();
            }
            return m_AudioManager;
        }
    }

    private void Init() 
    {
		foreach (Scr_Sound sound in m_Sounds)
        {
            sound.Source = gameObject.AddComponent<AudioSource>();

            sound.Source.clip = sound.Clip;
            sound.Source.volume = sound.Volume;
            sound.Source.pitch = sound.Pitch;
            sound.Source.loop = sound.Loop;
        }
	}

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public static void Play(string name)
    {
        Scr_Sound sound = Instance.GetSound(name);

        if (sound != null)
        {
            if (sound.IsPaused)
            {
                sound.Source.UnPause();
                sound.IsPaused = false;
            }
            else
                sound.Source.Play();
        }
    }

    public static void Pause(string name)
    {
        Scr_Sound sound = Instance.GetSound(name);

        if (sound != null)
        {
            sound.Source.Pause();
            sound.IsPaused = true;
        }
    }

    public static void Stop(string name)
    {
        Scr_Sound sound = Instance.GetSound(name);

        if (sound != null)
        {
            sound.Source.Stop();
            sound.IsPaused = false;
        }
    }

    public static void SetRandomPitch(string name)
    {
        Scr_Sound sound = Instance.GetSound(name);

        if (sound != null)
        {
            float newPitch = UnityEngine.Random.Range(Instance.m_MinPitch, Instance.m_MaxPitch);
            sound.Source.pitch = newPitch;
        }
    }

    private Scr_Sound GetSound(string name)
    {
        Scr_Sound sound = Array.Find(Instance.m_Sounds, s => s.Name == name);

        if (sound == null)
            Debug.LogWarning("Sound with name " + name + " could not be found");

        return sound;
    }
}
