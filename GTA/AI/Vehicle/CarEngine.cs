using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarEngine : MonoBehaviour
{
    public Transform path;
    public float maxSteeringAngle = 60f;
    public WheelCollider frontWheelLeft;
    public WheelCollider frontWheelRight;

    private List<Transform> nodes;
    private int currentNode = 0;

    // Start is called before the first frame update
    void Start()
    {
        Transform[] pathTransforms = path.GetComponentsInChildren<Transform>();
        nodes = new List<Transform>();

        for (int i = 0; i < pathTransforms.Length; i++)
        {
            if (pathTransforms[i] != path.transform)
                nodes.Add(pathTransforms[i]);
        }
    }

    void FixedUpdate()
    {
        ApplySteer();
        Drive();
        CheckWaypointDistance();
    }

    void ApplySteer()
    {
        Vector3 relativeVector = transform.InverseTransformPoint(nodes[currentNode].position);
        float newSteeringAngle = (relativeVector.x / relativeVector.magnitude) * maxSteeringAngle;
        frontWheelLeft.steerAngle = newSteeringAngle;
        frontWheelRight.steerAngle = newSteeringAngle;
    }

    void Drive()
    {
        frontWheelLeft.motorTorque = 50;
        frontWheelRight.motorTorque = 50;
    }

    void CheckWaypointDistance()
    {
        if (Vector3.Distance(transform.position, nodes[currentNode].position) < 0.5f)
        {
            if (currentNode == nodes.Count - 1)
                currentNode = 0;
            else
                currentNode++;
        }
    }
}
