using UnityEngine;

public class MusicController : MonoBehaviour
{
    [Component(InitializeIfMissing = true)]private AudioSource _audioSource;
    [RequiredSerializeField] [SerializeField] private AudioClip _musicData;

    private void Awake()
    {
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
