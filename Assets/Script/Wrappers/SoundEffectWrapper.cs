using UnityEngine;

[RequireComponent(typeof(AudioSource), typeof(PlayingSoundEffectCache))]
public class SoundEffectWrapper : MonoBehaviour
{
    private PlayingSoundEffectCache _soundEffectCache;
    public AudioSource AudioSource;
    public string SoundEffectOwnerName;
    public string SoundEffectName;

    private void Awake()
    {
        if (!(TryGetComponent<PlayingSoundEffectCache>(out _soundEffectCache) &&
              TryGetComponent<AudioSource>(out AudioSource)))
            throw new MissingComponentException("Missing PlayingSoundEffectCache or AudioSource components!");
    }

    private void Update()
    {
        const float AudioSourceDestroyDelay = 0.1f;
        
        if (AudioSource.isPlaying)
            return;
        
        Destroy(AudioSource, AudioSourceDestroyDelay);
        DestroyImmediate(this);
    }

    private void OnDestroy() =>
        _soundEffectCache.RemoveSoundEffect(SoundEffectOwnerName, SoundEffectName);
}