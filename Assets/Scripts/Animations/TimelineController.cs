using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineController : MonoBehaviour
{
    [SerializeField] PlayableDirector playable;

    public void Play(float time)
    {
        playable.time = time;
        
    }
}
