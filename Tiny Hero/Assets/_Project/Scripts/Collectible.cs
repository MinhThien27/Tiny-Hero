using UnityEngine;

public class Collectible: Entity
{
    [SerializeField] int score = 10; //Can using factory pattern in future
    [SerializeField] IntEventChannel scoreChannel;
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            scoreChannel.Invoke(score);
            Destroy(gameObject);
        }
    }
}
