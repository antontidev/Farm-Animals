using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource backgroundSource;

    public AudioEvent backgroundEvent;

    void Start()
    {
        backgroundEvent.Play(backgroundSource);
    }
}
