using UnityEngine;

public class Collectible : MonoBehaviour
{
    // Called when the sphere is picked up
    public void PickUp()
    {
        // Deactivate the sphere (removes it from the scene)
        gameObject.SetActive(false);
    }
}
