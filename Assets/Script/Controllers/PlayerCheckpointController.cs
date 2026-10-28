using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerCheckpointController : MonoBehaviour
{
    [Component] private Rigidbody2D _playerRigidbody;
    [RequiredSerializeField] [SerializeField] private Transform _startCheckpoint;
    private Vector3 _currentCheckpoint;

    private void Awake() =>
        SetCheckpoint(_startCheckpoint.position);

    public void SetCheckpoint(Vector3 position) =>
        _currentCheckpoint = position;

    public void GoToLastCheckpoint()
    {
        transform.position = _currentCheckpoint;
        _playerRigidbody.linearVelocity = Vector2.zero;
    }

    public void ResetLevel()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}
