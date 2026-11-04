using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(PlayingSoundEffectCache))]
public class SoundEffectController : MonoBehaviour
{
    [Component] private PlayingSoundEffectCache _soundEffectCache;
    private readonly System.Random _random = new();

    public void PlaySound(string soundEffectName, string soundEffectOwnerName, bool allowMultipleOfSoundEffect = false)
    {
        // If the game object is already playing the same sound effect don't play it again
        if (!allowMultipleOfSoundEffect && HasAudioSourceForSoundEffect(soundEffectName, soundEffectOwnerName))
            return;
        
        var audioClip = Resources.Load<AudioClip>(Constants.PathToSoundAssets + soundEffectName);

        if (allowMultipleOfSoundEffect)
        {
            var randomByteString = new byte[16];
            _random.NextBytes(randomByteString); // We fill the array with random data

            soundEffectName += randomByteString.ToHexString(); // We append the random data to our sound effect name.
        }
        
        _soundEffectCache.AddSoundEffect(soundEffectOwnerName, soundEffectName);
        
        var soundEffectWrapper = this.AddComponent<SoundEffectWrapper>();
        soundEffectWrapper.Initialize(soundEffectOwnerName, soundEffectName, audioClip);
    }

    private void Update() =>
        _soundEffectCache.RemoveUnusedGameObjects();

    private bool HasAudioSourceForSoundEffect(string soundEffectName, string soundEffectOwnerName) =>
        _soundEffectCache.IsGameObjectPlayingSoundEffect(soundEffectOwnerName, soundEffectName);
}
