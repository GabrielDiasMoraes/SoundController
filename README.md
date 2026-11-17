# SoundController

A small Unity utility for playing sound effects and looping background music through a data-driven `SoundMap` asset. It pools its `AudioSource` objects, fades music in and out, can randomize or step the pitch of repeated sounds, and mutes everything through an `AudioMixer` parameter.

## Requirements

- Unity 2021.3 (developed on 2021.3.5f1)
- An `AudioMixer` with an exposed float parameter named `MusicVolume` (used by `MuteSound`)

## Install

Copy these folders from `SoundController/Assets/DailySolutions/` into your project's `Assets/`, keeping the `.meta` files:

- `SoundController`
- `PoolUtility` (bundled dependency)
- `CoroutineRunner` (bundled dependency)

## Usage

```csharp
using DailySolutions.SoundController.Scripts;
using DailySolutions.CoroutineRunner.Scripts.Interfaces;

// 1. Create a SoundMap asset (Create > Data > SoundMap) and fill in its sounds and mixer.
// 2. Make a prefab with a SoundSource component and an AudioSource, and assign the AudioSource.
// 3. Add the CoroutineRunner component to a GameObject and pass it in (see the note below).

ISoundController sounds = new SoundController(soundMap, soundSourcePrefab, coroutineRunner);

sounds.PlaySound(SoundName.ButtonClick);
sounds.PlaySound(SoundName.BackgroundMusicMain, timeToFadeIn: 1.25f);
sounds.ChangeTheme(new[] { SoundName.BackgroundMusicMain }, SoundName.BackgroundMusicGame);
sounds.StopPlaySound(SoundName.BackgroundMusicGame, onComplete: () => Debug.Log("stopped"));
sounds.MuteSound(true);
```

## API (`ISoundController`)

| Method | What it does |
| --- | --- |
| `PlaySound(SoundName, float timeToFadeIn = 1.25f)` | Plays a one-shot, or starts a fading, looping clip cycle if the sound is marked `IsCicleLoop`. |
| `StopPlaySound(SoundName, UnityAction onComplete = null, float timeToFade = 0.5f)` | Fades out a running loop. Returns `false` if it was not playing. |
| `ChangeTheme(SoundName[] soundsToStop, SoundName soundToStart, float timeToFadeOut, float timeToFadeIn)` | Stops the running loop among `soundsToStop`, then starts `soundToStart`. |
| `StopPlaySounds(SoundName[] soundsToStop, float timeToFade = 0.5f)` | Fades out the first running loop in the list. |
| `MuteSound(bool)` | Sets the mixer's `MusicVolume` to the mute or normal value and pauses or resumes the loop source. |
| `ResetIncrementalPitch(SoundName)` | Restarts the incremental pitch sequence for a sound. |

Each `SoundInfo` entry in the `SoundMap` has a clip list (picked at random, or cycled in order for loops), a mixer group, `IsCicleLoop`, `PitchChange` (0 to 1) and `IsPitchIncremental`.

## Bundled dependencies

- `PoolUtility`: a simple `AudioSource` object pool (`PoolableItem`, `PoolUtility`).
- `CoroutineRunner`: a `MonoBehaviour` that implements `ICoroutineRunner`. Add it to a GameObject and pass that component to `SoundController`.

## Status

Work in progress and not used in any shipped project. The `SoundName` enum and the static `SoundsToStop*` arrays in `SoundMap` are specific to the original game: edit them for your own sounds.

## License

MIT, see [LICENSE](LICENSE).
