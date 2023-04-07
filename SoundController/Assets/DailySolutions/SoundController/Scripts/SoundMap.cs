using UnityEngine;
using UnityEngine.Audio;

namespace DailySolutions.SoundController.Scripts
{
    [CreateAssetMenu(menuName = "Data/SoundMap")]
    public class SoundMap : ScriptableObject
    {
        [SerializeField] SoundInfo[] _soundInfos;
        [SerializeField] AudioMixer _audioMixer;

        [SerializeField] private float _normalVolume = -40f;
        [SerializeField] private float _muteVolume = -80f;

        public bool TryGetSound(SoundName soundName, out SoundInfo soundInfo)
        {
            soundInfo = default;
            for (int i = 0; i < _soundInfos.Length; i++)
            {
                SoundInfo soundInfoTemp = _soundInfos[i];
                if (soundInfoTemp.SoundName == soundName)
                {
                    soundInfo = soundInfoTemp;
                    if (soundInfo.AudioClipList.Count == 0)
                    {
                        return false;
                    }
                    return true;
                }
            }
            return false;
        }

        public void MuteSounds(bool mute)
        {
            _audioMixer.SetFloat("MusicVolume", mute ? _muteVolume : _normalVolume);
        }

        public static SoundName[] SoundsToStopGame = new SoundName[] { SoundName.BackgroundMusicMain, SoundName.BackgroundMusicIconSelection, SoundName.BackgroundMusicResultScreen };
        public static SoundName[] SoundsToStopMain = new SoundName[] { SoundName.BackgroundMusicGame, SoundName.BackgroundMusicIconSelection, SoundName.BackgroundMusicResultScreen };
        public static SoundName[] SoundsToStopIcon = new SoundName[] { SoundName.BackgroundMusicMain, SoundName.BackgroundMusicGame, SoundName.BackgroundMusicResultScreen };
        public static SoundName[] SoundsToStopResult = new SoundName[] { SoundName.BackgroundMusicMain, SoundName.BackgroundMusicIconSelection, SoundName.BackgroundMusicGame };

    }
}
