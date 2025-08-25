using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [SerializeField] private ItemSO itemData;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var player = other.GetComponent<PlayerController>();
            if (player != null && itemData != null)
            {
                bool isApplied = itemData.ApplyEffect(player);
                if (isApplied)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
