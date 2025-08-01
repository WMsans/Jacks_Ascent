using UnityEngine;

public class SuckerJug : MonoBehaviour, IClimbable, IHarmable
{
    public int hitsToRelease = 3;
    private int currentHits;
    private bool isSucking = false;
    private Transform suckedHand = null;
    private ClimbingController climbingController;

    public float CurrentHp => currentHits;

    private void Start()
    {
        climbingController = FindFirstObjectByType<ClimbingController>();
        currentHits = hitsToRelease;
    }

    public void OnGrab(Transform hand)
    {
        if (!isSucking)
        {
            Debug.Log("SuckerJug: Grabbed by " + hand.name);
            isSucking = true;
            suckedHand = hand;
            currentHits = hitsToRelease;
        }
    }

    public void OnRelease()
    {
        // When sucking, the player cannot release voluntarily
        if (!isSucking)
        {
            suckedHand = null;
        }
    }

    public void OnHarm(float damage)
    {
        if (isSucking)
        {
            currentHits-= (int)damage;
            Debug.Log("SuckerJug: Hit! Remaining hits: " + currentHits);
            if (currentHits <= 0)
            {
                Debug.Log("SuckerJug: Released!");
                isSucking = false;
                // We must tell the climbing controller to release our hand
                if(climbingController != null)
                {
                    climbingController.ForceRelease(suckedHand);
                }
                suckedHand = null;
            }
        }
    }


    public bool IsHolding(Transform hand)
    {
        return isSucking && suckedHand == hand;
    }

    /// <summary>
    /// Prevents release while the jug is sucking the player's hand.
    /// </summary>
    public bool CanRelease()
    {
        return !isSucking;
    }
}