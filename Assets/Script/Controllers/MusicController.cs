using UnityEngine;

public class MusicController : MonoBehaviour, ICustomUnityScript
{
    [Component(InitializeIfMissing = true)]private AudioSource _audioSource;
    [SerializeField] private AudioClip _musicData;

    private void Awake()
    {
        ((ICustomUnityScript)this).InitializeAndValidateFields(this);

        _audioSource.spatialBlend = Constants._2DSound;
        _audioSource.clip = _musicData;
        _audioSource.playOnAwake = true;
    }

    private void Update()
    {
        if (!_audioSource.isPlaying)
            _audioSource.Play();
    }
}
