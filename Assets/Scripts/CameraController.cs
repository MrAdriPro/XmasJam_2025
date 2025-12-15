using System.Net.Sockets;
using Unity.VisualScripting;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    private Vector3 offset;

    void Start()
    {
        offset = target.position - transform.position;
    }
    private void FixedUpdate()
    {
        FollowCamera();
    }
    private void FollowCamera()
    {

        transform.position = target.position - offset;
    }
}
