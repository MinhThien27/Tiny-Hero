using UnityEngine;

public class PlayerSaveable : MonoBehaviour, ISaveable<PlayerSaveData>
{
    public PlayerSaveData CaptureState()
    {
        var health = GetComponent<PlayerHealth>();
        return new PlayerSaveData
        {
            position = transform.position,
            hp = health.CurrentHealth,
            gold = GameManager.Instance.Score
        };
    }

    public void RestoreState(PlayerSaveData data)
    {
        transform.position = data.position;
        GetComponent<PlayerHealth>().SetHealth(data.hp, false);
        GameManager.Instance.SetScore(data.gold);
    }
}
