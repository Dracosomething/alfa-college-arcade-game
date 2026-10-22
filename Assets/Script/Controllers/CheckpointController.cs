using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckpointController : MonoBehaviour, ICustomUnityScript
{
    private Transform _currentCheckpoint;
    [Component] private Rigidbody2D _playerRigidbody;

    private void Awake() =>
        ((ICustomUnityScript)this).InitializeComponents(this);

    public void SetCheckpoint(Transform position) =>
        _currentCheckpoint = position;

    public void GoToLastCheckpoint()
    {
            transform.position = _currentCheckpoint.position;
            _playerRigidbody.linearVelocity = Vector2.zero;
    }

    public void ResetLevel()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}
