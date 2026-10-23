using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(PlayingSoundEffectCache))]
public class SoundEffectController : MonoBehaviour, ICustomUnityScript
{
    private PlayingSoundEffectCache _soundEffectCache;
    private System.Random _random;
    
    private void Awake()
    {
        if (!TryGetComponent<PlayingSoundEffectCache>(out _soundEffectCache))
            throw new MissingComponentException("Missing a PlayingSoundEffectCache component.");

        _random = new System.Random();
    }

    public void PlaySound(string soundEffectName, string soundEffectOwnerName, bool allowMultipleOfSoundEffect = false)
    {
        // If the game object is already playing the same sound effect don't play it again
        if (!allowMultipleOfSoundEffect && HasAudioSourceForSoundEffect(soundEffectName, soundEffectOwnerName))
            return;
        
        var audioClip = Resources.Load<AudioClip>(Constants.PathToSoundAssets + soundEffectName);

        if (allowMultipleOfSoundEffect)
        {
            byte[] randomByteString = new byte[16];
            _random.NextBytes(randomByteString); // We fill the array with random data

            soundEffectName += randomByteString.ToHexString(); // We append the random data to our sound effect name.
        }
        
        _soundEffectCache.AddSoundEffect(soundEffectOwnerName, soundEffectName);
        
        var soundEffectWrapper = this.AddComponent<SoundEffectWrapper>();
        soundEffectWrapper.SoundEffectOwnerName = soundEffectOwnerName;
        soundEffectWrapper.SoundEffectName = soundEffectName;
        
        soundEffectWrapper.AudioSource.clip = audioClip;
        soundEffectWrapper.AudioSource.spatialBlend = Constants._2DSound;
        
        soundEffectWrapper.AudioSource.Play();
    }

    /// <summary>
    /// This does not function properly when there are multiple of 1 sound effect.
    /// </summary>
    public bool IsPlayingSoundEffect(string soundEffectName, string soundEffectOwner) =>
        HasAudioSourceForSoundEffect(soundEffectName, soundEffectOwner);

    private void Update() =>
        _soundEffectCache.RemoveUnusedGameObjects();

    private bool HasAudioSourceForSoundEffect(string soundEffectName, string soundEffectOwnerName) =>
        _soundEffectCache.IsGameObjectPlayingSoundEffect(soundEffectOwnerName, soundEffectName);
}
