using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private PlayerController playerController;

    void Start()
    {
        playerController = GetComponentInParent<PlayerController>();
    }

    public void AnimationEvent_Shoot()
    {
        if (playerController != null)
        {
            //playerController.AnimationEvent_Shoot();
        }
    }
}
