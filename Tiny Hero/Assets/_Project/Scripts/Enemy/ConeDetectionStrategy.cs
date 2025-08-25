using UnityEngine;

public class ConeDetectionStrategy : IDetectionStrategy
{
    readonly float detectionAngle;
    readonly float detectionRadius;
    readonly float innerDetectionRadius;

    public ConeDetectionStrategy(float detectionAngle, float detectionRadius, float innerDetectionRadius)
    {
        this.detectionAngle = detectionAngle;
        this.detectionRadius = detectionRadius;
        this.innerDetectionRadius = innerDetectionRadius;
    }
    public bool Execute(Transform player, Transform detector, CountdownTimer countdownTimer)
    {
        if (countdownTimer.IsRunning) 
            return false;

        var directionToPlayer = player.position - detector.position;
        var angleToPlayer = Vector3.Angle(directionToPlayer, detector.forward);

        //If the player is not within the detection cone or too far away, return false
        if ((!(angleToPlayer < detectionAngle / 2f) || !(directionToPlayer.magnitude < detectionRadius))
            && !(directionToPlayer.magnitude < innerDetectionRadius)) 
            return false;

        countdownTimer.Start();
        return true;
    }
}
