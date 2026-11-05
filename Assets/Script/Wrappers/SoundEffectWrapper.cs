using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(AudioSource), typeof(PlayingSoundEffectCache))]
public class SoundEffectWrapper : MonoBehaviour
{
    [Component] private PlayingSoundEffectCache _soundEffectCache;
    [Component(InitializeIfMissing = true)] private AudioSource _audioSource;
    [RequiredSerializeField, SerializeField] private string _soundEffectOwnerName;
    [RequiredSerializeField, SerializeField] private string _soundEffectName;

    public void Initialize(string soundEffectOwnerName, string soundEffectName, AudioClip soundEffect)
    {
        if (!this.TryGetComponent<PlayingSoundEffectCache>(out _soundEffectCache))
            throw new MissingComponentException("SoundEffectWrapper needs a PlayingSoundEffectCache component.");
        
        _audioSource = this.AddComponent<AudioSource>();
        _audioSource.spatialBlend = Constants._2DSound;
        _audioSource.clip = soundEffect;
        
        _soundEffectOwnerName = soundEffectOwnerName;
        _soundEffectName = soundEffectName;
        
        _audioSource.Play();
    }

    private void Update()
    {
        const float AudioSourceDestroyDelay = 0.1f;
        
        if (_audioSource.isPlaying)
            return;
        
        _soundEffectCache.RemoveSoundEffect(_soundEffectOwnerName, _soundEffectName);
        Destroy(_audioSource, AudioSourceDestroyDelay);
        DestroyImmediate(this);
    }
}