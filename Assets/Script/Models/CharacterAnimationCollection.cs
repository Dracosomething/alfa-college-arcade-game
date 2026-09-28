using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class CharacterAnimationCollection : IEnumerable<KeyValuePair<AnimationType, string>>
{
    private Dictionary<AnimationType, string> _internalDictionary;

    public string this[AnimationType key]
    {
        get => _internalDictionary[key];
        set => _internalDictionary[key] = value;
    }
    
    public CharacterAnimationCollection()
    {
        _internalDictionary = new Dictionary<AnimationType, string>();
    }
    
    public CharacterAnimationCollection(int size)
    {
        _internalDictionary = new Dictionary<AnimationType, string>(size);
    }
    
    public IEnumerable<string> Values() => _internalDictionary.Values;
    
    public void Add(KeyValuePair<AnimationType, string> toAddEntry) =>
        _internalDictionary[toAddEntry.Key] = toAddEntry.Value;

    public bool TryGet(AnimationType key, out string value) =>
        _internalDictionary.TryGetValue(key, out value);
    
    public IEnumerator<KeyValuePair<AnimationType, string>> GetEnumerator() =>
        _internalDictionary.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}