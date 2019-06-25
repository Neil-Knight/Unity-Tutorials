using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject FollowObject;
    public float MoveSpeed = 120f;
    public float clampAngle = 80f;
    public float inputSensitivity = 150f;

    Vector3 FollowPosition;
    GameObject CameraObject;
    GameObject PlayerObject;
    float mouseX, mouseY;
    float smoothX, smoothY;
    float inputX, inputZ;
    float finalInputX, finalInputZ;
    float rotX, rotY;
    GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        Vector3 rot = transform.localRotation.eulerAngles;
        rotY = rot.y;
        rotX = rot.x;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        inputX = Input.GetAxis("RightStickHorizontal");
        inputZ = Input.GetAxis("RightStickVertical");
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");
        finalInputX = inputX + mouseX;
        finalInputZ = inputZ + mouseY;

        rotY += finalInputX * inputSensitivity * Time.deltaTime;
        rotX += finalInputZ * inputSensitivity * Time.deltaTime;
        rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);
        Quaternion localRotation = Quaternion.Euler(rotX, rotY, 0f);
        transform.rotation = localRotation;
    }

    private void LateUpdate()
    {
        CameraUpdate();
    }

    void CameraUpdate()
    {
        Transform target = FollowObject.transform;
        float step = MoveSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, target.position, step);
    }
}
