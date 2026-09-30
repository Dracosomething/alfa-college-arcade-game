using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class PlayingSoundEffectCache : MonoBehaviour
{
    // Key is a string representing the GameObject name and
    // the value is a list where the elements are the sound effect names
    private Dictionary<string, List<string>> _internalDictionary = new();

    public void AddSoundEffect(GameObject key, string soundEffectName)
    {
        // We call TryAdd so that we don't overwrite the current value of key
        _internalDictionary.TryAdd(key.name, new List<string>());
        
        _internalDictionary[key.name].Add(soundEffectName);
    }

    public void RemoveSoundEffect(string key, string soundEffectName)
    {
        if (!_internalDictionary.ContainsKey(key))
            return;

        _internalDictionary[key].Remove(soundEffectName);
    }

    public void RemoveUnusedGameObjects() =>
        _internalDictionary = _internalDictionary
            // We filter out all entries that are empty or null
            .Where(gameObjectNameSoundEffectListPair => gameObjectNameSoundEffectListPair.Value != null &&
                                                        gameObjectNameSoundEffectListPair.Value.Count > 0)
            // We parse it back to a dictionary.
            .ToDictionary(gameObjectNameSoundEffectListPair => gameObjectNameSoundEffectListPair.Key,
                gameObjectNameSoundEffectListPair => gameObjectNameSoundEffectListPair.Value);

    public bool IsGameObjectPlayingSoundEffect(GameObject key, string soundEffect) =>
        _internalDictionary.TryGetValue(key.name, out List<string> soundEffects) &&
        soundEffects.Contains(soundEffect);

    public override string ToString()
    {
        StringBuilder stringBuilder = new("[ ");
        foreach (KeyValuePair<string, List<string>> gameObjectNameSoundEffectListPair in _internalDictionary)
        {
            stringBuilder
                .Append("{ ")
                .Append(gameObjectNameSoundEffectListPair.Key)
                .Append(", ")
                .Append(CollectionHelper.ConvertCollectionToProperlyFormattedString(gameObjectNameSoundEffectListPair
                    .Value))
                .Append(" }, ");
        }

        return stringBuilder.Append(" ]").ToString();
    }
}