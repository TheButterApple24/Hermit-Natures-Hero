using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilitySoundManager : MonoBehaviour
{
    public AudioSource AbilityAudio;
    public AudioClip FireIgnite;
    public AudioClip FireImpact;
    public AudioClip PlantPopIgnite;
    public AudioClip PlantSprayIgnite;
    public AudioClip WaterSprayIgnite;
    public AudioClip WaterStormIgnite;
    public AudioClip SteamBombIgnite;
    public AudioClip SteamBombLinger;
    public AudioClip MudslideLinger;
    public AudioClip BonfireLinger;

    public static AbilitySoundManager sfxInstance;

    private void Awake()
    {
        if (sfxInstance != null && sfxInstance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        sfxInstance = this;
    }
}
