using UnityEngine.Assertions;
using UnityEngine.TestTools;

public class PlayerMovementControllerTests
{
    /// <summary>
    /// We check if the player exists
    /// </summary>
    [UnityTest]
    public void PlayerExists() =>
        Assert.IsTrue(SceneHelper.TryFindGameObjectInScene(Constants.PlayerGameObjectName, out var _));

    [UnityTest]
    public void PlayerHasMovementController()
    {
        _ = SceneHelper.TryFindGameObjectInScene(Constants.PlayerGameObjectName, out var player);
        
        Assert.IsTrue( player.TryGetComponent<PlayerMovementController>(out _));
    }
    
    [UnityTest]
    public void CharacterMovementKeyboard()
    {
        
    }
}
