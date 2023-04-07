using System.Collections;
using System.Collections.Generic;
using DailySolutions.CoroutineRunner.Scripts.Interfaces;
using UnityEngine;
using UnityEngine.Events;

namespace DailySolutions.SoundController.Scripts
{
    public class SoundController : ISoundController
    {
        private readonly SoundMap _soundMap;
        private readonly SoundSource _soundSourcePrefab;
        private readonly ICoroutineRunner _coroutineRunner;
        private readonly PoolUtility.Scripts.PoolUtility _soundSoucePool;
        private readonly SoundSource _soundSourcePlayOneShot;
        private readonly SoundSource _soundSourceCycle;
        private readonly Dictionary<SoundName, SoundSource> _cycleRunning;
        private readonly Dictionary<SoundName, float> _incrementalPitch;

        public SoundController(SoundMap soundMap, SoundSource soundSourcePrefab, ICoroutineRunner coroutineRunner)
        {
            _soundMap = soundMap;
            _soundSourcePrefab = soundSourcePrefab;
            _coroutineRunner = coroutineRunner;
            _soundSoucePool = new PoolUtility.Scripts.PoolUtility(_soundSourcePrefab, 2);
            _soundSourceCycle = _soundSoucePool.GetFromPool<SoundSource>();
            _soundSourcePlayOneShot = _soundSoucePool.GetFromPool<SoundSource>();
            _cycleRunning = new Dictionary<SoundName, SoundSource>();
            _incrementalPitch = new Dictionary<SoundName, float>();
        }

        public void PlaySound(SoundName soundName, float timeToFadeIn = 1.25f)
        {
            if(_soundMap.TryGetSound(soundName, out var soundInfo))
            {
                if (soundInfo.IsCicleLoop)
                {
                    if (_cycleRunning.ContainsKey(soundName))
                        return;
                    _coroutineRunner.StartCoroutine(PlayCycleMusicLoop(soundInfo, timeToFadeIn: timeToFadeIn));
                }
                else
                {
                    var audioClip = GetAudioClip(soundInfo, out _);

                    ConfigureSoundSource(soundInfo, _soundSourcePlayOneShot);

                    PlaySound(_soundSourcePlayOneShot.AudioSource, audioClip, true);
                }
            }
        }

        public bool StopPlaySound(SoundName soundName, UnityAction onComplete = null, float timeToFade = 0.5f)
        {
            if (!_cycleRunning.TryGetValue(soundName, out var soundSource))
            {
                return false;
            }

            void OnCompleteFade()
            {
                soundSource.AudioSource.Stop();
                _cycleRunning.Remove(soundName);
                onComplete?.Invoke();
            }

            _coroutineRunner.StartCoroutine(FadeSound(soundSource, 1f, 0f, timeToFade, OnCompleteFade));
            return true;

        }

        public void ChangeTheme(SoundName[] soundsToStop, SoundName soundToStart, float timeToFadeOut = 0.5f, float timeToFadeIn = 1f)
        {
            for (int i = 0; i < soundsToStop.Length; i++)
            {
                if (StopPlaySound(soundsToStop[i], () => { PlaySound(soundToStart, timeToFadeIn); }))
                    return;
            }
            PlaySound(soundToStart, timeToFadeIn);
        }

        public void StopPlaySounds(SoundName[] soundsToStop, float timeToFade = 0.5f)
        {
            for (int i = 0; i < soundsToStop.Length; i++)
            {
                if (StopPlaySound(soundsToStop[i], null, timeToFade))
                    return;
            }
        }

        public void MuteSound(bool mute)
        {
            _soundMap.MuteSounds(mute);
            if (mute)
            {
                _soundSourceCycle.AudioSource.Pause();
            }
            else
            {
                _soundSourceCycle.AudioSource.UnPause();
            }
        }

        private IEnumerator PlayCycleMusicLoop(SoundInfo soundInfo, int newIndex = -1, float timeToFadeIn = 1.25f)
        {
            _cycleRunning.TryAdd(soundInfo.SoundName, _soundSourceCycle);
            AudioClip audioClip;
            if (newIndex == -1)
            {
                audioClip = GetAudioClip(soundInfo, out newIndex);
            }
            else
            {
                newIndex = newIndex >= soundInfo.AudioClipList.Count ? 0 : newIndex;
                audioClip = soundInfo.AudioClipList[newIndex];
            }

            ConfigureSoundSource(soundInfo, _soundSourceCycle);

            PlaySound(_soundSourceCycle.AudioSource, audioClip, false);

            _coroutineRunner.StartCoroutine(FadeSound(_soundSourceCycle, 0f, 1f, timeToFadeIn));

            yield return new WaitForEndOfFrame();

            while(audioClip.length > _soundSourceCycle.AudioSource.time)
            {
                yield return null;
                if (audioClip != _soundSourceCycle.AudioSource.clip)
                    yield break;
            }

            _coroutineRunner.StartCoroutine(PlayCycleMusicLoop(soundInfo, newIndex+1, timeToFadeIn));
        }

        private AudioClip GetAudioClip(SoundInfo soundInfo, out int index)
        {
            index = Random.Range(0, soundInfo.AudioClipList.Count);
            return soundInfo.AudioClipList[index];
        }

        private void ConfigureSoundSource(SoundInfo soundInfo, SoundSource soundSource)
        {
            soundSource.AudioSource.outputAudioMixerGroup = soundInfo.AudioMixerGroup;
            soundSource.AudioSource.volume = soundInfo.IsCicleLoop ? 0 : soundSource.AudioSource.volume;
            soundSource.AudioSource.pitch = 1f;
            if(soundInfo.PitchChange > 0f)
            {
                if (soundInfo.IsPitchIncremental)
                {
                    if(!_incrementalPitch.TryGetValue(soundInfo.SoundName, out var currentPitch))
                    {
                        currentPitch = 1 - soundInfo.PitchChange - 0.05f;
                    }
                    currentPitch = Mathf.Clamp(currentPitch + 0.05f, 1 - soundInfo.PitchChange, 1 + soundInfo.PitchChange);
                    soundSource.AudioSource.pitch = currentPitch;
                    _incrementalPitch[soundInfo.SoundName] = currentPitch;
                }
                else
                {
                    var randomVal = Random.Range(soundInfo.PitchChange * -1, soundInfo.PitchChange) + 1f;
                    soundSource.AudioSource.pitch = randomVal;
                }
            }
        }

        public void ResetIncrementalPitch(SoundName soundName)
        {
            _incrementalPitch.Remove(soundName);
        }

        private void PlaySound(AudioSource audioSource, AudioClip audioClip, bool isOneShot)
        {
#if UNITY_EDITOR
            audioSource.transform.name = audioClip.name;
#endif
            if (isOneShot)
            {
                audioSource.volume = 1f;
                audioSource.PlayOneShot(audioClip);
                return;
            }
            audioSource.clip = audioClip;
            audioSource.Play();
        }


        private IEnumerator FadeSound(SoundSource soundSource, float from, float to, float time, UnityAction onComplete = null)
        {
            var audioSource = soundSource.AudioSource;
            var audioClip = audioSource.clip;

            audioSource.volume = from;
            float passedTime = 0;
            while(passedTime <= time && audioClip.length > audioSource.time)
            {
                passedTime += Time.deltaTime;
                audioSource.volume = Mathf.Lerp(from, to, passedTime / time);
                yield return null;
            }
            audioSource.volume = to;
            onComplete?.Invoke();
        }
    }
}
