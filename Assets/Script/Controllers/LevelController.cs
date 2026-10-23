using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour
{
    [RequiredSerializeField] [SerializeField] private string _levelToLoadName;
    
    public void LoadLevel() =>
        SceneManager.LoadScene(_levelToLoadName);
}
