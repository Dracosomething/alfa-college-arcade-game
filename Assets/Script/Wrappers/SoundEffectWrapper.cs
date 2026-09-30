using UnityEngine;

public class SoundEffectWrapper : MonoBehaviour
{
    private PlayingSoundEffectCache _soundEffectCache;
    public AudioSource AudioSource;
    public string SoundEffectOwnerName;
    public string SoundEffectName;
    
    private void Awake() =>
        _soundEffectCache = GetComponent<PlayingSoundEffectCache>();

    private void Update()
    {
        if (AudioSource.isPlaying)
            return;
        
        DestroyImmediate(AudioSource);
        DestroyImmediate(this);
    }

    private void OnDestroy() =>
        _soundEffectCache.RemoveSoundEffect(SoundEffectOwnerName, SoundEffectName);
}