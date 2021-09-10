using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControls : MonoBehaviour
{
    public GameObject GameManager;
    public float CameraSpeed = .2f;
    public float ZoomSpeed = .2f;

    // Start is called before the first frame update
    void Start()
    {
        //GameManager.GetComponent<WorldGeneration>().
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKey("d"))
        {
            transform.SetPositionAndRotation(new Vector3(transform.position.x + CameraSpeed * GetComponent<Camera>().orthographicSize / 10, transform.position.y, transform.position.z), transform.rotation);
        }
        if (Input.GetKey("a"))
        {
            transform.SetPositionAndRotation(new Vector3(transform.position.x - CameraSpeed * GetComponent<Camera>().orthographicSize / 10, transform.position.y, transform.position.z), transform.rotation);
        }
        if (Input.GetKey("w"))
        {
            transform.SetPositionAndRotation(new Vector3(transform.position.x, transform.position.y + CameraSpeed * GetComponent<Camera>().orthographicSize / 10, transform.position.z), transform.rotation);
        }
        if (Input.GetKey("s"))
        {
            transform.SetPositionAndRotation(new Vector3(transform.position.x, transform.position.y - CameraSpeed * GetComponent<Camera>().orthographicSize / 10, transform.position.z), transform.rotation);
        }


        
    }

    void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0f) // Zoom In
        {
            GetComponent<Camera>().orthographicSize -= GetComponent<Camera>().orthographicSize / 10;
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0f) // Zoom Out
        {
            GetComponent<Camera>().orthographicSize += GetComponent<Camera>().orthographicSize / 10;
        }
    }
}
