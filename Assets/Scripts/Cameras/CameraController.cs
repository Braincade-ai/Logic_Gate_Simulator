using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    public float moveSpeed = 2;
    public float zoomSpeed=5;
    private Camera cam;
    public FixedJoystick joy;

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
        cam.orthographicSize = 3;
    }

    // Update is called once per frame
    void Update()
    {
        cam.orthographicSize += Input.mouseScrollDelta.y * zoomSpeed;
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, 1, 50);
      

        float i = joy.Horizontal;
        float j = joy.Vertical;

        transform.position += new Vector3(i, j) * moveSpeed * Time.deltaTime;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        transform.position += new Vector3(h, v) * moveSpeed * Time.deltaTime;
    }
}
