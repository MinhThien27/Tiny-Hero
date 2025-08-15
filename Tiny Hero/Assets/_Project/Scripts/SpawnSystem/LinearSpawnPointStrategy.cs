using UnityEngine;

public class LinearSpawnPointStrategy : ISpawnPointStrategy
{
    private Transform[] spawnPoints;
    private int currentIndex = 0;
    public LinearSpawnPointStrategy(Transform[] spawnPoints)
    {
        this.spawnPoints = spawnPoints;
    }
    public Transform NextSpawnPoint()
    {
        if (spawnPoints.Length == 0) return null;

        Transform spawnPoint = spawnPoints[currentIndex];
        currentIndex = (currentIndex + 1) % spawnPoints.Length;
        return spawnPoint;
    }
}