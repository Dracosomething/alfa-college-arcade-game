using UnityEngine;

public class SoundEffectController : MonoBehaviour
{
    public void PlaySound(string soundEffectName, in GameObject soundEffectOwner)
    {
        var audioClip = Resources.Load<AudioClip>(Constants.PathToSoundAssets + soundEffectName);

        var audioSource = soundEffectOwner.AddComponent<AudioSource>();
        audioSource.clip = audioClip;
        audioSource.spatialBlend = Constants._2DSound;
        audioSource.Play();
        Object.Destroy(audioSource, audioClip.length);
    }
}
