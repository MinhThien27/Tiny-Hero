using UnityEngine;

public class CollectableSpawnManager : EnititySpawnManager
{
    [SerializeField] private CollectibleData[] collectibleData;
    [SerializeField] private float spawnInterval = 1f;

    EntitySpawner<Collectible> spawner;
    CountdownTimer spawnTimer;

    int counter = 0;

    protected override void Awake()
    {
        base.Awake();
        spawner = new EntitySpawner<Collectible>(new EntityFactory<Collectible>(collectibleData), spawnPointStrategy);

        spawnTimer = new CountdownTimer(spawnInterval);
        spawnTimer.OnTimerStopped += () =>
        {
            if (counter >= spawnPoints.Length)
            {
                spawnTimer.Stop();
                return;
            }
            Spawn();
            counter++;
            spawnTimer.Start();
        };
    }
    private void Start() => spawnTimer.Start();

    private void Update() => spawnTimer.Tick(Time.deltaTime);

    public override void Spawn() => spawner.SpawnEntity();
}