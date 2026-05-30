using System;
using UnityEngine;

public class RaceCarControl : MonoBehaviour
{
    [Header("Wheels")]
    public WheelCollider fl;
    public WheelCollider fr;
    public WheelCollider rr;
    public WheelCollider rl;

    [Header("Physics Materials")]
    public PhysicsMaterial dryMaterial;
    public PhysicsMaterial wetMaterial;
    public PhysicsMaterial iceMaterial;

    [Header("Friction Stiffness Settings")]
    public float dryStiffness = 1.0f;
    public float wetStiffness = 0.7f;
    public float iceStiffness = 0.25f;

    [Header("Settings")]
    [SerializeField] private float _maxSpeed = 40f;
    [SerializeField] public Rigidbody _Rigidbody = null;

    [Header("Sensors")]
    [SerializeField] public Transform _distSensor;
    [SerializeField] public Transform _distSensorLeft;
    [SerializeField] public Transform _distSensorRight;

    private void Start()
    {
        if (_Rigidbody == null) _Rigidbody = GetComponent<Rigidbody>();
    }

    public void Update()
    {
        showSensorRays();
        getInput();
    }
    void getInput()
    {
        float turn = Input.GetAxis("Horizontal");
        float throttle = Input.GetAxis("Vertical");
        //Move(turn, throttle);
    }
    // Ta metoda jest teraz jedynym miejscem, które porusza samochodem!
    public void Move(float turn, float throttle)
    {
        // 1. Poruszanie i skręcanie (koła)
        fl.motorTorque = throttle * 1000f;
        fr.motorTorque = throttle * 1000f;
        rr.motorTorque = throttle * 1000f;
        rl.motorTorque = throttle * 1000f;

        fl.steerAngle = turn * 30f;
        fr.steerAngle = turn * 30f;

        // 2. Aktualizacja tarcia na podstawie nawierzchni
        UpdateWheelFriction(fl);
        UpdateWheelFriction(fr);
        UpdateWheelFriction(rr);
        UpdateWheelFriction(rl);
    }

    void UpdateWheelFriction(WheelCollider wheel)
    {
        WheelHit hit;
        if (wheel.GetGroundHit(out hit))
        {
            PhysicsMaterial surfaceMat = hit.collider.sharedMaterial;
            if (surfaceMat != null)
            {
                if (surfaceMat == dryMaterial) SetWheelStiffness(wheel, dryStiffness);
                else if (surfaceMat == wetMaterial) SetWheelStiffness(wheel, wetStiffness);
                else if (surfaceMat == iceMaterial) SetWheelStiffness(wheel, iceStiffness);
                else SetWheelStiffness(wheel, 1.0f);
            }
        }
    }

    void SetWheelStiffness(WheelCollider wheel, float stiffness)
    {
        WheelFrictionCurve forwardFriction = wheel.forwardFriction;
        forwardFriction.stiffness = stiffness;
        wheel.forwardFriction = forwardFriction;

        WheelFrictionCurve sidewaysFriction = wheel.sidewaysFriction;
        sidewaysFriction.stiffness = stiffness;
        wheel.sidewaysFriction = sidewaysFriction;
    }

    public void resetPosition()
    {
        gameObject.transform.localPosition = new Vector3(0.0f, 5.0f, 0.0f);
        gameObject.transform.localRotation = Quaternion.identity;
        if (_Rigidbody != null)
        {
            _Rigidbody.linearVelocity = Vector3.zero;
            _Rigidbody.angularVelocity = Vector3.zero;
        }
    }

    public float[] distanceToWall()
    {
        float[] distance = new float[3];
        float maxDistance = 50f;

        Ray[] rays = new Ray[]
        {
            new Ray(_distSensorLeft.position, _distSensorLeft.forward),
            new Ray(_distSensorRight.position, _distSensorRight.forward),
            new Ray(_distSensor.position, _distSensor.forward)
        };

        for (int i = 0; i < rays.Length; i++)
        {
            RaycastHit hit;
            distance[i] = Physics.Raycast(rays[i], out hit, maxDistance) ? hit.distance : maxDistance;
            distance[i] = distance[i] / maxDistance;
        }
        return distance;
    }

    public float getVelocity()
    {
        return _Rigidbody.linearVelocity.magnitude / _maxSpeed;
    }

    public bool isGoingBackwards()
    {
        Vector3 localVelocity = transform.InverseTransformDirection(_Rigidbody.linearVelocity);
        return (localVelocity.z < -0.1f);
    }

    public void showSensorRays()
    {
        Debug.DrawRay(_distSensorLeft.position, _distSensorLeft.forward * 50, Color.red);
        Debug.DrawRay(_distSensorRight.position, _distSensorRight.forward * 50, Color.red);
        Debug.DrawRay(_distSensor.position, _distSensor.forward * 50, Color.blue);
    }
}