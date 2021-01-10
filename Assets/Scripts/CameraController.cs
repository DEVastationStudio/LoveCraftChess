using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    /*
    public float maxZoom;
    public float minZoom;
    public float zoomMultiplierPC;
    public float zoomMultiplierMobile;
    */
    public Camera cam;
    public TableGenerator TG;
    [SerializeField] private Vector3 targetPos;
    [SerializeField] private float topAngle;
    [SerializeField] private float bottomAngle;
    [SerializeField] private float distanceFromTarget;
    [SerializeField] private float maxDistanceFromTarget;
    [SerializeField] private float minDistanceFromTarget;
    [SerializeField] private float localDistance;
    [SerializeField] private float zoomFactor;
    public float scrollData;
    public float currentFov;
    public bool canRotate;
    public bool canTarget;
    public bool isOverButtons;
    private bool canMove;
    private GameObject ZeroZeroPos;
    private Vector3 lastTargetPosition;
    //public bool blockCamera;
    public float ResetFov = 75f;

    //cam things
    [SerializeField] private GameObject[] cameraResetPositions;
    private Vector3[] cameraPositions = new Vector3[2];
    private Quaternion[] cameraRotations = new Quaternion[2];
    private GameObject[] cameraTargets = new GameObject[2];
    private GameObject curTarget;


    private Vector3 previousPosition;
    private Vector3 direction;

    private void Start()
    {
        cam = Camera.main;
        targetPos = new Vector3( (TG.tableObj.numRows / 2) - 0.5f, 0.8f, (TG.tableObj.numCols / 2) - 0.5f);
        ZeroZeroPos = new GameObject();
        ZeroZeroPos.transform.position = new Vector3((TG.tableObj.numRows / 2) - 0.5f, 0.8f, (TG.tableObj.numCols / 2) - 0.5f);
        for (int i = 0; i < 2; i++) 
        {
            cameraTargets[i] = ZeroZeroPos;
            cameraPositions[i] = cameraResetPositions[i].transform.position;
            currentFov = ResetFov;
            cam.fieldOfView = currentFov;
        }
        //TB.tableObj.numRows;
    }

    private void FixAngles(float angle)
    {
        Vector3 pastAngle = cam.transform.eulerAngles;
        Vector3 newDirection = (Quaternion.Euler(angle, pastAngle.y, 0) * Vector3.forward).normalized;
        cam.transform.position = targetPos/*.transform.position*/ + newDirection * -(distanceFromTarget + localDistance);
        cam.transform.LookAt(targetPos/*.transform*/);
    }

    public void CanMoveCamera(bool _canMove) { canRotate = _canMove; }
    public void CanTargetObjects(bool _canTarget) { canTarget = _canTarget; }

    public void FreeCamera() { if (curTarget != null) curTarget = null; }

    public void ResetTarget()
    {
        int player = TG.GetLocalPlayer() - 1;
        canTarget = true;
        ChangeTarget(ZeroZeroPos);
        canTarget = false;
        cam.transform.position = cameraResetPositions[player].transform.position;
        cam.transform.rotation = cameraResetPositions[player].transform.rotation;
        currentFov = ResetFov;
        cam.fieldOfView = currentFov;
        return;
    }

    public void ChangeTarget(GameObject _tar)
    {
        if (!canTarget) return;
        curTarget = _tar;
        Debug.Log("Target change");
        targetPos = _tar.transform.position;

        //Not the best implementation, angle could have been clamped in step 2 instead of fixing it later in step 4
        //direction = previousPosition - cam.ScreenToViewportPoint(previousPosition);
        direction = Vector3.zero;
        //Step 1: moves cam to target 
        cam.transform.position = targetPos/*.transform.position*/;

        //Step 2: cam rotates over x and y axis
        cam.transform.Rotate(new Vector3(1, 0, 0), direction.y * 180);
        cam.transform.Rotate(new Vector3(0, 1, 0), -direction.x * 180, Space.World);

        //Step 3: Translates the cam from the target to a certain distance
        cam.transform.Translate(new Vector3(0, 0, -(distanceFromTarget + localDistance)));

        //Step 4: Transform is fixed between the limits
        //previousPosition = cam.ScreenToViewportPoint(Input.mousePosition);
        if (cam.transform.eulerAngles.x < bottomAngle || cam.transform.eulerAngles.x > topAngle)
        {

            if (direction.y < 0)
            {
                FixAngles(bottomAngle);
            }
            else if (direction.y > 0)
            {
                FixAngles(topAngle);
            }
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0) && isOverButtons) isOverButtons = false;

        if (!canRotate || isOverButtons) return;
        if (Input.GetMouseButtonDown(0))
        {
            previousPosition = cam.ScreenToViewportPoint(Input.mousePosition);
        }

        if (curTarget != null && lastTargetPosition != curTarget.transform.position)
        {
            if (canTarget)
                ChangeTarget(curTarget);
            else
            {
                canTarget = true;
                ChangeTarget(curTarget);
                canTarget = false;
            }
        }

        if (Input.GetMouseButton(0) || Input.touchCount == 1)
        {
            //Not the best implementation, angle could have been clamped in step 2 instead of fixing it later in step 4
            direction = previousPosition - cam.ScreenToViewportPoint(Input.mousePosition);

            //Step 1: moves camera to target 
            cam.transform.position = targetPos/*.transform.position*/;

            //Step 2: camera rotates over x and y axis
            cam.transform.Rotate(new Vector3(1, 0, 0), direction.y * 180);
            cam.transform.Rotate(new Vector3(0, 1, 0), -direction.x * 180, Space.World);

            //Step 3: Translates the camera from the target to a certain distance
            cam.transform.Translate(new Vector3(0, 0, -(distanceFromTarget + localDistance)));

            //Step 4: Transform is fixed between the limits
            previousPosition = cam.ScreenToViewportPoint(Input.mousePosition);
            if (cam.transform.eulerAngles.x < bottomAngle || cam.transform.eulerAngles.x > topAngle)
            {

                if (direction.y < 0)
                {
                    FixAngles(bottomAngle);
                }
                else if (direction.y > 0)
                {
                    FixAngles(topAngle);
                }
            }
        }


        if (Input.mouseScrollDelta.y > 0.0f || Input.mouseScrollDelta.y < 0.0f || Input.touchCount == 2)
        {
            Debug.Log(Input.mouseScrollDelta.y);
            scrollData = Input.mouseScrollDelta.y * -1;
            currentFov = cam.fieldOfView;
            currentFov += scrollData * zoomFactor;
            currentFov = Mathf.Clamp(currentFov, minDistanceFromTarget, maxDistanceFromTarget);
            cam.fieldOfView = currentFov;
        }

        if(curTarget!=null)
            lastTargetPosition = curTarget.transform.position;

    }
}
