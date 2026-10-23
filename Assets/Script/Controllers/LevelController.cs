using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour, IDynamicExceptionProvider
{
    [SerializeField] private string _levelToLoadName;

    private void Awake() =>
        ((IDynamicExceptionProvider)this).ValidateSerializeFieldsSet(this);
    
    public void LoadLevel() =>
        SceneManager.LoadScene(_levelToLoadName);
}
