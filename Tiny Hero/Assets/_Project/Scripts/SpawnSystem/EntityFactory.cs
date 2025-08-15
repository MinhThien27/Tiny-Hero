using UnityEngine;

public class EntityFactory<T> : IEntityFactory<T> where T : Entity
{
    private readonly T _prefab;
    private EntityData[] data;
    public EntityFactory(EntityData[] entityData)
    {
        this.data = entityData;
    }
    public T CreateEntity(Transform spawnPoint)
    {
        EntityData entityData = data[Random.Range(0, data.Length)];
        GameObject entity = Object.Instantiate(entityData.prefab, spawnPoint.position, spawnPoint.rotation);
        return entity.GetComponent<T>();
    }
}