using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCollector : MonoBehaviour
{
    private const int TotalSpheres = 7;
    private int spheresCollected = 0;
    [SerializeField] private LayerMask collectibleLayerMask;
    [SerializeField] private WinUI winUI; 
    private Collider capsuleCollider;

    void Start()
    {
        Transform capsule = transform.Find("Capsule");
        if (capsule != null)
        {
            capsuleCollider = capsule.GetComponent<Collider>();
            if (capsuleCollider == null)
            {
                Debug.LogError("Capsule child no Collider");
            }
        }
        else
        {
            Debug.LogError("Capsule child not found");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && capsuleCollider != null)   //check for E key press to collect spheres
        {
            CollectSphere();
        }
    }


    private void CollectSphere()  //attempts to collect a sphere within the Capsule's trigger
    {
        Collider[] hits = Physics.OverlapSphere(capsuleCollider.bounds.center, capsuleCollider.bounds.extents.magnitude, collectibleLayerMask);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Collectible"))   //check if the hit is a collectible
            {
                Collectible collectible = hit.GetComponent<Collectible>();  //collect the sphere
                if (collectible != null)
                {
                    collectible.PickUp();
                    spheresCollected++;
                    if (spheresCollected >= TotalSpheres)                        //check for win condition
                    {
                        WinGame();
                    }
                }
            }
        }
    }

    #region WINNER
    private void WinGame()
    {
        //show the Win UI
        if (winUI != null)
        {
            winUI.ShowWin();
        }
    }
    #endregion 
}