using UnityEngine;

[RequireComponent(typeof(AudioSource), typeof(PlayingSoundEffectCache))]
public class SoundEffectWrapper : MonoBehaviour, IDynamicExceptionProvider
{
    [Component] private PlayingSoundEffectCache _soundEffectCache;
    [Component] public AudioSource AudioSource;
    public string SoundEffectOwnerName;
    public string SoundEffectName;

    private void Awake() =>
        ((IDynamicExceptionProvider)this).InitializeComponents(this);

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