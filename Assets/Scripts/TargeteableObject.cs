using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargeteableObject : MonoBehaviour
{
    private CameraController cameraController;

    private void Start()
    {
        cameraController = FindObjectOfType<CameraController>();
    }

    private void OnMouseDown()
    {
        if (Input.GetMouseButton(0))
        {
            cameraController.ChangeTarget(gameObject);
        }
    }
}
