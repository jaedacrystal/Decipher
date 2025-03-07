using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Vector3 offset = new Vector3(0, 0, -10);
    private float smoothTime = 0.25f;
    Vector3 camVelocity = Vector3.zero;
    public float zoom;
    public float minZoom = 4;
    public float maxZoom = 6;
    public float zoomVelocity = 0;

    [SerializeField] Transform target;
    [SerializeField] Camera cam;

    private void Start()
    {
        cam.orthographicSize = zoom;
    }

    void Update()
    {
        Vector3 targetPos = target.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref camVelocity, smoothTime);

        float camScroll = Input.GetAxisRaw("Mouse ScrollWheel");
        zoom -= camScroll * 4;
        zoom = Mathf.Clamp(zoom, minZoom, maxZoom);
        cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, zoom, ref zoomVelocity, smoothTime);
    }
}
