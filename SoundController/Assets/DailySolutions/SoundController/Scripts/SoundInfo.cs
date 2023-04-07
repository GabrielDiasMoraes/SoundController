using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace DailySolutions.SoundController.Scripts
{
    [Serializable]
    public struct SoundInfo
    {
        public SoundName SoundName;
        public List<AudioClip> AudioClipList;
        public AudioMixerGroup AudioMixerGroup;
        public bool IsCicleLoop;
        [Range(0f, 1f)] public float PitchChange;
        public bool IsPitchIncremental;
    }
}