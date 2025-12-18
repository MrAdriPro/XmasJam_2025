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
            playerController.ShootAnimationEvent();
        }
    }

    public void AnimationEvent_FixatedTrue()
    {
        playerController.altFixedFlip(true);
    }

    public void AnimationEvent_FixatedFalse()
    {
        playerController.altFixedFlip(false);
    }
}
