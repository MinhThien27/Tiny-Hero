using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RandomSpawnPointStrategy : ISpawnPointStrategy
{
    private Transform[] spawnPoints;
    private List<Transform> unusedSpawnPoints = new List<Transform>();

    public RandomSpawnPointStrategy(Transform[] spawnPoints)
    {
        this.spawnPoints = spawnPoints;
        unusedSpawnPoints = new List<Transform>(spawnPoints);
    }
    public Transform NextSpawnPoint()
    {
        if (spawnPoints.Length == 0) return null;

        if (!unusedSpawnPoints.Any())
        {
            unusedSpawnPoints = new List<Transform>(spawnPoints);
        }

        int randomIndex = Random.Range(0, unusedSpawnPoints.Count);
        var spawnPoint = unusedSpawnPoints[randomIndex];
        unusedSpawnPoints.RemoveAt(randomIndex);
        return spawnPoint;
    }
}