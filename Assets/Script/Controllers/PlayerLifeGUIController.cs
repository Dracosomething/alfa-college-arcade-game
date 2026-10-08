using System;
using UnityEngine;
using TMPro;

public class PlayerLifeGUIController : MonoBehaviour
{
    private PlayerLifeController _playerLifeController;
    private TextMeshProUGUI _textMesh;
    
    private void Awake()
    {
        if (!SceneHelper.TryFindGameObjectWithTagInScene("Player", out var playerGameObject))
            throw new CouldNotFindGameObjectException("Player");

        if (!playerGameObject.TryGetComponent<PlayerLifeController>(out _playerLifeController))
            throw new MissingComponentException("PlayerLifeController not found on player.");

        if (!TryGetComponent<TextMeshProUGUI>(out _textMesh))
            throw new MissingComponentException("TextMeshPro not found on object.");
    }

    private void Update()
    {
        _textMesh.text = $"{_playerLifeController.CurrentLives}/{_playerLifeController.MaximumLives}";
    }
}
