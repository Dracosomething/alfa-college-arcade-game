using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(PlayingSoundEffectCache))]
public class SoundEffectController : MonoBehaviour
{
    private PlayingSoundEffectCache _soundEffectCache;

    private void Awake() =>
        _soundEffectCache = GetComponent<PlayingSoundEffectCache>();

    public void PlaySound(string soundEffectName, in GameObject soundEffectOwner)
    {
        // If the game object is already playing the same sound effect don't play it again
        if (HasAudioSourceForSoundEffect(soundEffectName, in soundEffectOwner))
            return;
      
        var audioClip = Resources.Load<AudioClip>(Constants.PathToSoundAssets + soundEffectName);
        
        _soundEffectCache.AddSoundEffect(soundEffectOwner, soundEffectName);
        
        var soundEffectWrapper = this.AddComponent<SoundEffectWrapper>();
        soundEffectWrapper.SoundEffectOwnerName = soundEffectOwner.name;
        soundEffectWrapper.SoundEffectName = soundEffectName;
        
        soundEffectWrapper.AudioSource = soundEffectOwner.AddComponent<AudioSource>();
        soundEffectWrapper.AudioSource.clip = audioClip;
        soundEffectWrapper.AudioSource.spatialBlend = Constants._2DSound;
        
        soundEffectWrapper.AudioSource.Play();
    }

    public bool IsPlayingSoundEffect(string soundEffectName, GameObject soundEffectOwner) =>
        HasAudioSourceForSoundEffect(soundEffectName, in soundEffectOwner);

    private void Update() =>
        _soundEffectCache.RemoveUnusedGameObjects();

    private bool HasAudioSourceForSoundEffect(string soundEffectName, in GameObject gameObject) =>
        _soundEffectCache.IsGameObjectPlayingSoundEffect(gameObject, soundEffectName);
}
