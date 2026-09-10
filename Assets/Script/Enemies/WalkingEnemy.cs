using System.Linq;
using UnityEngine;

public class WalkingEnemy : Enemy
{
    public Transform[] PatrolPoints;

    private void FixedUpdate()
    {
        DrawDebug();
    }
    
    public override void Update()
    {
        base.Update();
        Patrol();
    }

    private void Patrol()
    {   // We check if there are any patrol points
        if (PatrolPoints.Length == 0) 
            return;
        Transform targetPoint = PatrolPoints.First();
        var targetPosition = (Vector2)targetPoint.position;
        
        RigidBody.position = Vector2.MoveTowards(
            current: RigidBody.position, 
            target: targetPosition,
            maxDistanceDelta: Speed * Time.fixedDeltaTime);
        
        float distanceToTargetPosition = Vector2.Distance(RigidBody.position, targetPosition);
        
        if (distanceToTargetPosition <= 1)
        {
            for (var i = 0; i < PatrolPoints.Length - 1; i++)
            {
                PatrolPoints[i] = PatrolPoints[i + 1];
            }
            PatrolPoints[PatrolPoints.Length - 1] = targetPoint;
        }
    }

    private void DrawDebug()
    {
        for (int i = 0; i < PatrolPoints.Length; i++)
        {
            Debug.DrawLine(transform.position, PatrolPoints[i].position, Color.green);
        }
        Debug.DrawRay(PatrolPoints[0].position, Vector2.up * 2, Color.red);
    }
}
