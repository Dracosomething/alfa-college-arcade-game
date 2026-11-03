using UnityEngine;
using TMPro;

public class PlayerLifeGUIController : MonoBehaviour
{
    private PlayerLifeController _playerLifeController;
    [Component] private TextMeshProUGUI _textMesh;
    
    private void Start()
    {
        if (!SceneHelper.TryFindGameObjectWithTagInScene("Player", out var playerGameObject))
            throw new CouldNotFindGameObjectException("Player");

        if (!playerGameObject.TryGetComponent<PlayerLifeController>(out _playerLifeController))
            throw new MissingComponentException("PlayerLifeController not found on player.");
    }

    private void Update() =>
        _textMesh.text = $"{_playerLifeController.CurrentLives}/{_playerLifeController.MaximumLives}";
}
