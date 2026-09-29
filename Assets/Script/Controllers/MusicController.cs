using Unity.VisualScripting;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    private AudioSource _audioSource;
    [SerializeField] private AudioClip _musicData;

    private void Awake()
    {
        _audioSource = this.AddComponent<AudioSource>();

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
