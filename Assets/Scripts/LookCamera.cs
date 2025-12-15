using UnityEngine;

public class LookCamera : MonoBehaviour
{
    private void Update()
    {
        transform.LookAt(Camera.main.transform);
    }
}
