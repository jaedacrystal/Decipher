using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeManager : MonoBehaviour
{
    public SoundFX sfx;

    void Start()
    {
        sfx.GetComponent<SoundFX>();
    }

    
    public void LowerVolume()
    {
        sfx.SetVolume(volume);
    }
}
