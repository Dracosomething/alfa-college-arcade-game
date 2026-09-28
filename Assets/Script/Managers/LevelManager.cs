using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private string levelToLoadName;

    private void Awake()
    {
        if (string.IsNullOrEmpty(levelToLoadName))
            throw new StringNullOrEmptyException($"{nameof(levelToLoadName)} does not have a value.");
    }

    public void LoadLevel() =>
        SceneManager.LoadScene(levelToLoadName);
}
