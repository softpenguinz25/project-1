using MyBox;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;

    public List<AudioClip> clips = new List<AudioClip>();

    [MinMaxRange(0, 1)] public RangedFloat volume = new RangedFloat(0.8f, 1);
    [MinMaxRange(-3, 3)] public RangedFloat pitch = new RangedFloat(0.9f, 1.1f);

    [HideInInspector]
    public List<AudioSource> sources = new List<AudioSource>();
}