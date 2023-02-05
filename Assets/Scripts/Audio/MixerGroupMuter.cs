using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MixerGroupMuter : MonoBehaviour
{
    public AudioMixerGroup mixerGroup;
    public string propName;
    
    public void MuteVolume(bool isMuted)
    {
        mixerGroup.audioMixer.SetFloat(propName, isMuted ? -80 : 0);
    }
}
