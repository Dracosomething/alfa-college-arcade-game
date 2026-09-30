using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckpointController : MonoBehaviour
{
    private Transform _currentCheckpoint;
    private Rigidbody2D _playerRigidbody;

    private void Start()
    {
        if (!TryGetComponent<Rigidbody2D>(out _playerRigidbody))
            throw new MissingComponentException("No Rigidbody2D found on player.");
    }

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
