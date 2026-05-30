using Grpc.Core;
using System;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class RaceCarAgent : Agent
{

    [Header("Race Car References")]
    private RaceCarControl _carControl;
    private RaceCarState _carState;
    [Header("Floor and physics materials")]
    [SerializeField] private PhysicsMaterial _dryMaterial;
    [SerializeField] private PhysicsMaterial _wetMaterial;
    [SerializeField] private PhysicsMaterial _iceMaterial;
    [SerializeField] private GameObject _floor;
    int randomMat=0;
    [Header("Rewards")]
    [SerializeField] private float _winReward = 10.0f;
    [SerializeField] private float _loseReward = -10.0f;
    //[SerializeField] private float _timeReward = 0.000f;
    //[SerializeField] private float _wallHitReward = 0.000f;
    //[SerializeField] private float _wallNotHitReward = 0.000f;
    //[SerializeField] private float _speedReward = 0.000f;
    //[SerializeField] private float _goingBackwardsReward = -0.5f;
    [SerializeField] private float _timeReward = 0;//-0.001f;
    [SerializeField] private float _wallHitReward = -1.0f;
    [SerializeField] private float _wallNotHitReward = 1f;
    [SerializeField] private float _speedReward = 0.001f;
    [SerializeField] private float _goingBackwardsReward = -0.5f;
    [SerializeField] private float _checkpointCheckReward = 1.0f;

    [Header("Iterations")]
    [SerializeField] private int _maxIterations = 3000;
    [SerializeField] private int _currentIterations = 0;

    private float turn =0, throttle = 0;
    public override void OnEpisodeBegin()
    {
        resetLevel();
    }

    public void resetLevel()
    {
        activeLocalCheckpoints.Clear();

        foreach (Transform cp in allLocalCheckpoints)
        {
            cp.gameObject.SetActive(true); // W³¹czamy z powrotem na scenie
            activeLocalCheckpoints.Add(cp); // Dodajemy z powrotem do aktywnej listy
        }

        UpdateNearestCheckpoint();
        _currentIterations = 0;
        _carControl.resetPosition();

        randomMat = UnityEngine.Random.Range(0, 3);
        if (randomMat == 0)
        {
            _floor.GetComponent<Collider>().sharedMaterial = _dryMaterial;
        }
        else if (randomMat == 1)
        {
            _floor.GetComponent<Collider>().sharedMaterial = _wetMaterial;
        }
        else
        {
            _floor.GetComponent<Collider>().sharedMaterial = _iceMaterial;
        }
    }


    public override void OnActionReceived(ActionBuffers actions)
    {
        //SetReward(_wallNotHitReward);
        if (_carState.CollectedPoints < 0)
        {
            FinishEpisode(false);
            return;
        }
        AddReward(_speedReward * _carControl.getVelocity());
        //Debug.Log(_carControl.getVelocity());
        if (_carControl.isGoingBackwards()) AddReward(_goingBackwardsReward);

        AddReward(_timeReward);

        _currentIterations++;
        if (_currentIterations > _maxIterations)
        {
            FinishEpisode(false);
            return;
        }

        turn = actions.ContinuousActions[0];
        throttle = actions.ContinuousActions[1];
    }

    // Observations
    public override void CollectObservations(VectorSensor sensor)
    {
        // Space ship position [2]
        float[] sensorData = _carControl.distanceToWall();
        sensor.AddObservation(sensorData[0]);
        sensor.AddObservation(sensorData[1]);
        sensor.AddObservation(sensorData[2]);
        sensor.AddObservation(_carControl.getVelocity());
        sensor.AddObservation(_carControl.isGoingBackwards());
        sensor.AddObservation(getDirectionToCheckpoint());
        sensor.AddObservation(transform.forward);
        sensor.AddObservation(randomMat==0);
        sensor.AddObservation(randomMat==1);
        sensor.AddObservation(randomMat==2);


    }

    private void OnTriggerEnter(Collider other)
    {
        int finish = LayerMask.NameToLayer("Finish");
        int checkpoint = LayerMask.NameToLayer("Checkpoint");

        if (other.gameObject.layer == finish)
        {
            Debug.Log("finisz");
            // SetReward(_winReward);
            FinishEpisode(true); // Zak³adam, ¿e to Twoja w³asna metoda wywo³uj¹ca EndEpisode()
            return;
        }
        else if (other.gameObject.layer == checkpoint)
        {
            AddReward(_checkpointCheckReward);

            other.gameObject.SetActive(false);

            activeLocalCheckpoints.Remove(other.transform);

            UpdateNearestCheckpoint();
        }


    }
    private void OnCollisionEnter(Collision collision)
    {
        int walls = LayerMask.NameToLayer("Walls");
        if (collision.gameObject.layer == walls)
        {
            FinishEpisode(false);
        }
        return;
    }
    private void FinishEpisode(bool wonEpisode)
    {
        if (wonEpisode)
        {
            //_colorNotifier.OnEpisodeWon();
            AddReward(_winReward);
        }
        else
        {
            //_colorNotifier.OnEpisodeLost();
            AddReward(_loseReward);
        }
        Debug.Log("Agent " + gameObject.name + " zakoñczy³ epizod!");
        Debug.Log("NAGRODA: " + GetCumulativeReward());
        //_labelDisplayer.DisplayRewards(GetCumulativeReward());
        EndEpisode();
    }

    public Vector3 nearestCheckpointPos;

    // G³ówna lista - tu trzymamy wszystkie checkpointy (nigdy z niej nie usuwamy)
    private List<Transform> allLocalCheckpoints = new List<Transform>();

    // Aktywna lista dla obecnego epizodu (z niej bêdziemy usuwaæ zdobyte)
    private List<Transform> activeLocalCheckpoints = new List<Transform>();
    // Lista, w której przechowamy checkpointy TYLKO z naszego œrodowiska
    private List<Transform> localCheckpoints = new List<Transform>();
    private void FindLocalCheckpoints()
    {
        // Zauwa¿y³em, ¿e w Twoim kodzie warstwa nazywa siê "Checkpoint" (liczba pojedyncza)
        int layerIndex = LayerMask.NameToLayer("Checkpoint");

        Transform environmentParent = transform.parent;
        // Wa¿ne: Dodajemy 'true' jako argument, aby szuka³o równie¿ ukrytych (wy³¹czonych) obiektów
        Transform[] allChildren = environmentParent.GetComponentsInChildren<Transform>(true);

        foreach (Transform child in allChildren)
        {
            if (child.gameObject.layer == layerIndex)
            {
                allLocalCheckpoints.Add(child);
            }
        }
        Debug.Log("CHECKPOINTY: "+allLocalCheckpoints.Count);
    }

    // Tê metodê mo¿esz wywo³ywaæ w OnActionReceived lub Update
    public void UpdateNearestCheckpoint()
    {
        if (activeLocalCheckpoints.Count == 0) return;

        float minSqrDistance = Mathf.Infinity;
        Transform nearest = null;

        foreach (Transform cp in activeLocalCheckpoints)
        {
            float sqrDistance = (cp.position - transform.position).sqrMagnitude;
            if (sqrDistance < minSqrDistance)
            {
                minSqrDistance = sqrDistance;
                nearest = cp;
            }
        }

        if (nearest != null) nearestCheckpointPos = nearest.localPosition;
    }

    public Vector3 getDirectionToCheckpoint()
    {
        return Vector3.Normalize((nearestCheckpointPos - transform.localPosition));
    }



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        _carControl = GetComponent<RaceCarControl>();
        _carState = GetComponent<RaceCarState>();
        FindLocalCheckpoints();
        UpdateNearestCheckpoint();
    }

    // Update is called once per frame
    void Update()
    {
        _carControl.showSensorRays();
        //_carControl.Move(0.0f, 1.0f);
    }

    private void FixedUpdate()
    {
        _carControl.Move(turn, throttle);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // Prze³¹czamy siê na ContinuousActions, bo metoda Move oczekuje floatów
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;

        // --- 1. OBS£UGA SKRÊCANIA (turn -> indeks 0) ---
        float turnInput = 0f;
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            turnInput = 1f;  // Skrêt w prawo
        }
        else if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            turnInput = -1f; // Skrêt w lewo
        }
        continuousActions[0] = turnInput;

        // --- 2. OBS£UGA GAZU/HAMULCA (throttle -> indeks 1) ---
        float throttleInput = 0f;
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            throttleInput = 1f;  // Gaz do przodu
        }
        else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            throttleInput = -1f; // Wsteczny / Hamowanie
        }
        continuousActions[1] = throttleInput;
    }
}
