using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXPlayer : MonoBehaviour
{
    private AudioSource player;

    public AudioClip[] clips;

    private void Start()
    {
        player = GetComponent<AudioSource>();
    }
    public bool play(int i)
    {
        if(i < clips.Length)
        {
            player.PlayOneShot(clips[i], 1f);
        }
        return i < clips.Length;        
    }
    public bool play(string clip)
    {
        bool result = false;
        foreach(AudioClip c in clips)
        {
            if (c.name == clip)
            {
                player.PlayOneShot(c,1f);
                result = true;
            }
        }
        return result;
    }
}
