using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;
    private void Awake()
    {
        foreach (Sound s in sounds)
        {
            for (int i = 0; i < s.clips.Count; i++)
            {
                s.sources.Add(gameObject.AddComponent<AudioSource>());
                s.sources[i].clip = s.clips[i];
                s.sources[i].volume = (s.volume.Min + s.volume.Max) * .5f;
                s.sources[i].pitch = (s.pitch.Min + s.pitch.Max) * .5f;
            }
        }
    }

    public void Play(string name)
    {
        Sound s = GetSoundFromName(name);
        if (s == null) return;

        AudioSource randomSource = s.sources[UnityEngine.Random.Range(0, s.sources.Count)];
        randomSource.volume = UnityEngine.Random.Range(s.volume.Min, s.volume.Max);
        randomSource.pitch = UnityEngine.Random.Range(s.pitch.Min, s.pitch.Max);
        randomSource.Play();
    }

    #region TODO: Add 3D position
    public void Play(string name, Vector3 worldPos)
    {
        Sound s = GetSoundFromName(name);
        if (s == null) return;

        /*for (int i = 0; i < s.clips.Count; i++)
        {
            GameObject sourceObj = new GameObject(name + " SFX");
            sourceObj.transform.position = worldPos;
            sourceObj.AddComponent<AudioSource>();
            Sound source = sourceObj.GetComponent<AudioSource>();

            source.sources[i] = gameObject.AddComponent<AudioSource>();
            source.sources[i].clip = s.clips[i];
            source.sources[i].volume = UnityEngine.Random.Range(s.volume.Min, s.volume.Max);
            source.sources[i].pitch = UnityEngine.Random.Range(s.pitch.Min, s.pitch.Max);
        }*/

        s.sources[UnityEngine.Random.Range(0, s.sources.Count)].Play();
    }
    #endregion

    private Sound GetSoundFromName(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return null;
        }
        return s;
    }
}
