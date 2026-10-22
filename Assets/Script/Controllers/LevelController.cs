using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour, ICustomUnityScript
{
    [SerializeField] private string _levelToLoadName;

    private void Awake() =>
        ((ICustomUnityScript)this).ValidateSerializeFieldsSet(this);
    
    public void LoadLevel() =>
        SceneManager.LoadScene(_levelToLoadName);
}
