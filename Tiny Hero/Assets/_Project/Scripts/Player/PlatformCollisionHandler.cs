using KBCore.Refs;
using System.Threading;
using UnityEngine;
public class PlatformCollisionHandler : MonoBehaviour
{
    Transform platform; //the moving platform where player step on

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("MovingPlatform"))
        {
            //If contact normal is pointing up, we've collided with the top of platform
            ContactPoint contact = collision.GetContact(0);
            //Check if player not on the top of platform, return
            if (contact.normal.y < 0.5f) return;

            platform = collision.transform;
            transform.SetParent(platform);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("MovingPlatform"))
        {
            transform.SetParent(null);
            platform = null;
        }
    }
}
