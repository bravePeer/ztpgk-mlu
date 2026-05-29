//using UnityEngine;

//public class RaceCarControl : MonoBehaviour
//{
//    // Start is called once before the first execution of Update after the MonoBehaviour is created
//    void Start()
//    {

//    }

//    // Update is called once per frame
//    void Update()
//    {

//    }
//}


using System;
using System.Numerics;
using Unity.Mathematics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;
using Vector2 = UnityEngine.Vector2;

public class RaceCarControl : MonoBehaviour
{
    [Header("Settings")]
    [Range(0.0f, 5.0f)][SerializeField] private float _moveSpeed = 5.0f;
    [SerializeField] private float _maxSpeed = 40;

    [Space]
    [SerializeField] private Vector2 _turnRange = new Vector2(-7, 7);
    [SerializeField] private Vector2 _throttle = new Vector2(-1, 1);

    [SerializeField] public Rigidbody _Rigidbody = null;

    [Header("Sensors")]
    [SerializeField] public Transform _distSensor;
    [SerializeField] public Transform _distSensorLeft;
    [SerializeField] public Transform _distSensorRight;
    private Vector3 leftDir;
    private Vector3 leftPos;
    private Vector3 rightDir;
    private Vector3 rightPos;
    private Vector3 centerDir;
    private Vector3 centerPos;



    private float throttleInput;
    private float turnInput;

    private void Start()
    {
        leftDir = _distSensorLeft.forward;
        leftPos = _distSensorLeft.position;
        rightDir = _distSensorRight.forward;
        rightPos = _distSensorRight.position;
        centerDir = _distSensor.forward;
        centerPos = _distSensor.position;
    }

    public void Update()
    {
        leftDir = _distSensorLeft.forward;
        leftPos = _distSensorLeft.position;
        rightDir = _distSensorRight.forward;
        rightPos = _distSensorRight.position;
        centerDir = _distSensor.forward;
        centerPos = _distSensor.position;
        getInput();
        //Move(turnInput, throttleInput);
        //ApplySidewaysFriction();
        showSensorRays();
    }
    private void FixedUpdate()
    {
        Move(turnInput, throttleInput);
        ApplySidewaysFriction();
    }

    public void resetPosition()
    {
        gameObject.transform.localPosition = new Vector3(0.0f, 5.0f, 0.0f);
        gameObject.transform.localRotation = new UnityEngine.Quaternion(0, 0, 0, 1);
    }
    //public Vector2 NormalizedPosition
    //{
    //    get
    //    {
    //        return Normalization.NormalizeVector2(
    //               new Vector2(transform.localPosition.x, transform.localPosition.z),
    //               new Vector2(_xAxisRange.x, _zAxisRange.x),
    //               new Vector2(_xAxisRange.y, _zAxisRange.y)
    //           );
    //    }
    //}
    private void getInput()
    {
        turnInput = Input.GetAxis("Horizontal");
        throttleInput = Input.GetAxis("Vertical");
    }
    public void Move(float turn, float throttle)
    {
        float forwardSpeed = Vector3.Dot(_Rigidbody.linearVelocity, transform.forward);

        // Poruszanie
        if (_Rigidbody.linearVelocity.magnitude < _maxSpeed) _Rigidbody.AddRelativeForce(Vector3.forward * Math.Clamp(throttle, -_throttle.y, _throttle.y) * 10f * _moveSpeed, ForceMode.Acceleration);
        

        // Skręcanie
        if (Mathf.Abs(turn) > 0.1f)
        {
            //// Gracz skręca - dodajemy siłę
            //float turnSpeed = Convert.ToInt32(throttle!=0) * turn * forwardSpeed * (_moveSpeed /* * 0.1f*/);
            //turnSpeed = Mathf.Clamp(turnSpeed, -_turnRange.y, _turnRange.y);
            //_Rigidbody.AddRelativeTorque(Vector3.up * turnSpeed * 10, ForceMode.Acceleration);

            // Gracz skręca - dodajemy siłę
            float turnSpeed = /*Convert.ToInt32(throttle!=0) **/ Math.Clamp(turn, _turnRange.x, _turnRange.y) * forwardSpeed * (_moveSpeed * 0.7f);
            //turnSpeed = turnSpeed, -_turnRange.y, _turnRange.y);
            _Rigidbody.AddRelativeTorque(Vector3.up * turnSpeed, ForceMode.Acceleration);
        }
        else
        {
            // Gracz puścił kierownicę - aktywnie wygaszamy rotację (tzw. Counter-Torque)
            // Im większa wartość (np. 5.0f), tym szybciej auto "prostuje" tor jazdy
            _Rigidbody.angularVelocity = Vector3.Lerp(_Rigidbody.angularVelocity, Vector3.zero, Time.fixedDeltaTime * 5.0f);
        }
    }
    [Header("Ustawienia Przyczepności")]
    public float gripForce = 10f; // Jak mocno auto trzyma się bocznie (0 = lód, 20+ = wyścigówka)

    private void ApplySidewaysFriction()
    {
        // 1. Obliczamy prędkość boczną (relative sideways velocity)
        // transform.right to oś "bok-bok" naszej kostki
        Vector3 sidewaysVelocity = transform.right * Vector3.Dot(_Rigidbody.linearVelocity, transform.right);

        // 2. Nakładamy siłę przeciwną do prędkości bocznej
        // To "zabija" ruch w bok, symulując tarcie opon
        _Rigidbody.AddForce(-sidewaysVelocity * gripForce, ForceMode.Acceleration);
    }

    //public void SetPosition(Vector2 position)
    //{
    //    transform.localPosition = new Vector3(
    //            Mathf.Clamp(position.x, _xAxisRange.x, _xAxisRange.y),
    //            0.0f,
    //            Mathf.Clamp(position.y, _zAxisRange.x, _zAxisRange.y)
    //    );
    //}

    public float[] distanceToWall()//znormalizowane
    {
        
        float[] distance = new float[3];
        float maxDistance = 50f;

        // Tworzymy tablicę promieni, aby łatwo iterować po nich w pętli
        Ray[] rays = new Ray[]
        {
            new Ray(leftPos, leftDir),
            new Ray(rightPos, rightDir),
            new Ray(centerPos, centerDir)
        };

        for (int i = 0; i < rays.Length; i++)
        {
            RaycastHit hit;
            // Jeśli trafimy, zapisz odległość, w przeciwnym wypadku daj maxDistance
            distance[i] = Physics.Raycast(rays[i], out hit, maxDistance) ? hit.distance : maxDistance;
            distance[i] = distance[i] / maxDistance;
        }
        return distance;
    }
    public float getVelocity()//znormalizowane
    {
        return _Rigidbody.linearVelocity.magnitude/_maxSpeed;
    }
    public bool isGoingBackwards()
    {
        Vector3 localVelocity = transform.InverseTransformDirection(_Rigidbody.linearVelocity);
        return(localVelocity.z < 0.0f);
    }
    public void showSensorRays()
    {
        
        Debug.DrawRay(leftPos, leftDir*50);
        Debug.DrawRay(rightPos, rightDir*50);
    }
}
