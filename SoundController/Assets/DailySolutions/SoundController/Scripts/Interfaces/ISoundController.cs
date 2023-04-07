using DailySolutions.SoundController.Scripts;
using UnityEngine.Events;

public interface ISoundController
{
    public void PlaySound(SoundName soundName, float timeToFadeIn = 1.25f);

    public bool StopPlaySound(SoundName soundName, UnityAction onComplete = null, float timeToFade = 0.5f);

    public void ChangeTheme(SoundName[] soundsToStop, SoundName soundToStart, float timeToFadeOut = 0.5f,
        float timeToFadeIn = 1f);

    public void StopPlaySounds(SoundName[] soundsToStop, float timeToFade = 0.5f);
    
    public void MuteSound(bool mute);

    public void ResetIncrementalPitch(SoundName soundName);
    
}
