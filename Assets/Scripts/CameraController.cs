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

    [SerializeField] private GameObject FixCameraBtn;
    [SerializeField] private GameObject FreeCameraBtn;
    [SerializeField] private GameObject FixTargetBtn;
    [SerializeField] private GameObject ChangeTargetBtn;

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
    private bool zoomBool;
    private bool moveBool;
    private Vector2 v1;
    private Vector2 v2;


    private Vector3 previousPosition;
    private Vector3 direction;

    private void Start()
    {
        cam = Camera.main;
        //targetPos = new Vector3( (TG.tableObj.numRows / 2) - 0.5f, 0.8f, (TG.tableObj.numCols / 2) - 0.5f);
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
    public void ChangeTurn()
    {
        canRotate = false;
        curTarget = null;
        canTarget = false;
        ChangeTargetBtn.SetActive(false);
        FixCameraBtn.SetActive(false);
        FixTargetBtn.SetActive(false);
        FreeCameraBtn.SetActive(true);
    }
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
        if (Input.GetMouseButtonDown(0) && Input.touchCount <= 1 )
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

        if (Input.GetMouseButton(0) && Input.touchCount <=1)
        {
            if (!moveBool) { previousPosition = cam.ScreenToViewportPoint(Input.mousePosition); moveBool = true; }
            else 
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
        }


        if (Input.mouseScrollDelta.y > 0.0f || Input.mouseScrollDelta.y < 0.0f)
        {
            Debug.Log(Input.mouseScrollDelta.y);
            scrollData = Input.mouseScrollDelta.y * -1;
            currentFov = cam.fieldOfView;
            currentFov += scrollData * zoomFactor;
            currentFov = Mathf.Clamp(currentFov, minDistanceFromTarget, maxDistanceFromTarget);
            cam.fieldOfView = currentFov;
        }

        if (Input.touchCount == 2)
        {
            if(moveBool) moveBool = false;
            Touch t0 = Input.GetTouch(0);
            Touch t1 = Input.GetTouch(1);
            
            if (zoomBool)
            {
                //Vector2 prevT0 = t0.position - t0.deltaPosition;
                //Vector2 prevT1 = t1.position - t1.deltaPosition;

                float prevDeltaMag = (v1 - v2).magnitude;
                float curDeltaMag = (t0.position - t1.position).magnitude;

                float deltaMagnitudeDiff = prevDeltaMag - curDeltaMag;

                cam.fieldOfView += deltaMagnitudeDiff * zoomFactor * 0.05f;
                cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, minDistanceFromTarget, maxDistanceFromTarget);
            }
            else
            {
                zoomBool = true;
            }
            v1 = t0.position;
            v2 = t1.position;
        }
        else zoomBool = false;

        if(curTarget!=null)
            lastTargetPosition = curTarget.transform.position;

    }
}
