using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayingSoundEffectCache : MonoBehaviour
{
    // Key is a string representing the GameObject name and
    // the value is a list where the elements are the sound effect names
    private Dictionary<string, List<string>> _internalDictionary = new();

    public void AddSoundEffect(string soundEffectOwnerName, string soundEffectName)
    {
        // We call TryAdd so that we don't overwrite the current value of key
        _internalDictionary.TryAdd(soundEffectOwnerName, new List<string>());
        
        _internalDictionary[soundEffectOwnerName].Add(soundEffectName);
    }

    public void RemoveSoundEffect(string soundEffectOwnerName, string soundEffectName)
    {
        if (!_internalDictionary.ContainsKey(soundEffectOwnerName))
            return;

        _internalDictionary[soundEffectOwnerName].Remove(soundEffectName);
    }

    public void RemoveUnusedGameObjects() =>
        _internalDictionary = _internalDictionary
            // We filter out all entries that are empty or null
            .Where(gameObjectNameSoundEffectListPair => 
                gameObjectNameSoundEffectListPair.Value != null && gameObjectNameSoundEffectListPair.Value.Count > 0)
            // We parse it back to a dictionary.
            .ToDictionary(gameObjectNameSoundEffectListPair => gameObjectNameSoundEffectListPair.Key,
                gameObjectNameSoundEffectListPair => gameObjectNameSoundEffectListPair.Value);

    public bool IsGameObjectPlayingSoundEffect(string soundEffectOwnerName, string soundEffect) =>
        _internalDictionary.TryGetValue(soundEffectOwnerName, out List<string> soundEffects) &&
        soundEffects.Contains(soundEffect);
}