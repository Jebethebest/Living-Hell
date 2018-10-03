using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Scr_Particle
{
    public Scr_Particle(string name, ParticleSystem particle)
    {
        Name = name;
        Particle = particle;
    }

    public string Name;
    public ParticleSystem Particle;
}