using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public List<Sound> sounds = new List<Sound>();

    private void Awake()
    {
        foreach(Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        Play("General_AmbientMusic");
    }

    public void Play(string name)
    {
        Sound s = sounds.Find(Sound => Sound.name == name);
        if (s == null) return;
        if (s.source.isPlaying) return;
        s.source.Play();
    }
}
