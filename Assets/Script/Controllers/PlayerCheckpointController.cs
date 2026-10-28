using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerCheckpointController : MonoBehaviour
{
    [Component] private Rigidbody2D _playerRigidbody;
    [RequiredSerializeField] [SerializeField] private Transform _startCheckpoint;
    private Transform _currentCheckpoint;

    private void Awake() =>
        SetCheckpoint(_startCheckpoint);

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
