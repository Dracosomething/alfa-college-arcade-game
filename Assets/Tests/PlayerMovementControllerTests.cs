using System.Collections;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TestTools;

public class PlayerMovementControllerTests : InputTestFixture
{
    private GameObject player;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        player = GameObject.Find("Player");
    }
    
    [UnityTest]
    public IEnumerator MovementLeftJoystickTest()
    {
        var controller = InputSystem.AddDevice<Gamepad>();
        var playerRigidBody = player.GetComponent<Rigidbody2D>();
        var currentVelocity = playerRigidBody.linearVelocityX;

        Move(controller.leftStick, new Vector2(-1, 0));

        Debug.Log(playerRigidBody.linearVelocityX);
        Assert.IsTrue(playerRigidBody.linearVelocityX < currentVelocity);
    }

    [Test]
    public void MovementLeftKeyboardTest()
    {
        var keyboard = InputSystem.AddDevice<Keyboard>();
        var playerRigidBody = player.GetComponent<Rigidbody2D>();
        var currentVelocity = playerRigidBody.linearVelocityX;
        
        PressAndRelease(keyboard.aKey);

        Debug.Log(playerRigidBody.linearVelocityX);
        Assert.IsTrue(playerRigidBody.linearVelocityX < currentVelocity);
    }
    
    
    [Test]
    public void MovementRightJoystickTest()
    {
        var controller = InputSystem.AddDevice<Gamepad>();
        var playerRigidBody = player.GetComponent<Rigidbody2D>();
        var currentVelocity = playerRigidBody.linearVelocityX;
        
        Move(controller.leftStick, new Vector2(1, 0));

        Debug.Log(playerRigidBody.linearVelocityX);
        Assert.IsTrue(playerRigidBody.linearVelocityX < currentVelocity);
    }

    [Test]
    public void MovementRightKeyboardTest()
    {
        var keyboard = InputSystem.AddDevice<Keyboard>();
        var playerRigidBody = player.GetComponent<Rigidbody2D>();
        var currentVelocity = playerRigidBody.linearVelocityX;
        
        PressAndRelease(keyboard.dKey);

        Debug.Log(playerRigidBody.linearVelocityX);
        Assert.IsTrue(playerRigidBody.linearVelocityX < currentVelocity);
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        //EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
    }
}
