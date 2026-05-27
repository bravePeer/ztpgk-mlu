using Grpc.Core;
using System;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class RaceCarAgent : Agent
{

    [Header("Space Ship References")]
    private RaceCarControl _carControl;
    private RaceCarState _carState;

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
    [SerializeField] private int _maxIterations = 2200;
    [SerializeField] private int _currentIterations = 0;


    public override void OnEpisodeBegin()
    {
        activeLocalCheckpoints.Clear();

        foreach (Transform cp in allLocalCheckpoints)
        {
            cp.gameObject.SetActive(true); // W³¹czamy z powrotem na scenie
            activeLocalCheckpoints.Add(cp); // Dodajemy z powrotem do aktywnej listy
        }

        // Reset pozycji agenta...
        //transform.localPosition = new Vector3(0, 0.5f, 0);

        // ZnajdŸ najbli¿szy na sam start
        UpdateNearestCheckpoint();

        _carState.ResetPoints();
        _currentIterations = 0;
        _carControl.resetPosition();
        //List<Vector2> positions = GenerateVariousPoints(5, 1.5f);
        //_shipControl.SetPosition(positions[0]);
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
        //if (_currentIterations > _maxIterations || false)
        //{
        //    FinishEpisode(false);
        //    return;
        //}

        //int moveX = actions.DiscreteActions[0] - 1;
        //int moveZ = actions.DiscreteActions[1] - 1;
        float turn = actions.ContinuousActions[0];
        float throttle = actions.ContinuousActions[1];
        _carControl.Move(turn, throttle);

        //Debug.Log("NAGRODA: " + GetCumulativeReward()+/*" distanes: " + _carControl.distanceToWall()[0]+", "+ _carControl.distanceToWall()[1] + ", "+ _carControl.distanceToWall()[2] + ", " + "|"+_carControl.isGoingBackwards()*/);
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


        // Current score [1]
        //sensor.AddObservation(_carState.NormalizedPoints);

        // 2 x Valid target [2] + 2 x Invalid target [2] 
        // [8] Observations
        //try
        //{
        //    sensor.AddObservation(_validTargets[0].NormalizedPosition);
        //    sensor.AddObservation(_validTargets[1].NormalizedPosition);

        //    sensor.AddObservation(_invalidTargets[0].NormalizedPosition);
        //    sensor.AddObservation(_invalidTargets[1].NormalizedPosition);
        //}
        //catch (IndexOutOfRangeException ex)
        //{
        //    Debug.LogError(ex.Message);
        //}
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

            // 1. Zamiast Destroy, WY£¥CZAMY obiekt. Agent ju¿ w niego nie wejdzie.
            other.gameObject.SetActive(false);

            // 2. USUWAMY go z listy aktywnych checkpointów
            activeLocalCheckpoints.Remove(other.transform);

            // 3. Aktualizujemy cel (znajdzie kolejny najbli¿szy z tych, co zosta³y)
            UpdateNearestCheckpoint();
        }

    //// Collect valid target
    //if (other.CompareTag()
    //{
    //    SetReward(_collectValidTargetPoints);
    //    validTarget.SetPosition(GenerateVariousPoints(1, 0.0f)[0]);
    //    _shipState.AddPoints(1);
    //}

    //// Collect invalid target
    //if (other.TryGetComponent<VirusTarget>(out VirusTarget invalidTarget))
    //{
    //    SetReward(_collectInvalidTargetPoints);
    //    invalidTarget.SetPosition(GenerateVariousPoints(1, 0.0f)[0]);
    //    _shipState.AddPoints(-1);
    //}
    }
    private void OnCollisionEnter(Collision collision)
    {
        int walls = LayerMask.NameToLayer("Walls");
        // Sprawdzamy, czy warstwa opuszczaj¹cego obiektu zgadza siê z naszym ID
        if (collision.gameObject.layer == walls)
        {
            //AddReward(_wallHitReward);
            //AddReward(_wallHitReward);
            FinishEpisode(false);
        }
        return;
        //else
        //{
        //    AddReward(_wallNotHitReward);
        //}
    }
    private void FinishEpisode(bool wonEpisode)
    {
        _carState.ResetPoints();

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
    //public override void Heuristic(in ActionBuffers actionsOut)
    //{ }

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
