using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckpointController : MonoBehaviour, IDynamicExceptionProvider
{
    [Component] private Rigidbody2D _playerRigidbody;
    private Transform _currentCheckpoint;

    private void Awake() =>
        ((IDynamicExceptionProvider)this).InitializeComponents(this);

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
