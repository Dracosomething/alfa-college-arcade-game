using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour
{
    [SerializeField] private string _levelToLoadName;

    private void Awake()
    {
        if (string.IsNullOrEmpty(_levelToLoadName))
            throw new StringNullOrEmptyException($"{nameof(_levelToLoadName)} does not have a value.");
    }

    public void LoadLevel() =>
        SceneManager.LoadScene(_levelToLoadName);
}
