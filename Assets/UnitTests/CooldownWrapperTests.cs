using System.Collections;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

public class CooldownWrapperTests
{
    [Test]
    public void StartCooldown_ThenCooldownActiveTest()
    {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets\\UnitTests\\CooldownObject.prefab");
        var instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
        var cooldownWrapper = instance.GetComponent<CooldownWrapper>();
        
        cooldownWrapper.StartCooldown();
        
        Assert.True(cooldownWrapper.IsCooldownActive());
    }

    [UnityTest]
    public IEnumerator CooldownStop_After2SecondsTest()
    {
        var prefab = GameObject.Find("CooldownObject");
        var cooldownWrapper = prefab.GetComponent<CooldownWrapper>();
        
        cooldownWrapper.StartCooldown();
        
        yield return new WaitForSeconds(2);
        
        Assert.False(cooldownWrapper.IsCooldownActive());
    }
}
