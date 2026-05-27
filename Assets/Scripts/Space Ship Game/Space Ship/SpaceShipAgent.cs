using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(SpaceShipControl))]
[RequireComponent(typeof(SpaceShipState))]
public class CarAgent : Agent
{
    [Header("Space Ship References")]
    private SpaceShipControl _shipControl;
    private SpaceShipState _shipState;

    [Header("Targets")]
    [SerializeField] private Target[] _validTargets = new Target[2];
    [SerializeField] private Target[] _invalidTargets = new Target[2];

    [Header("References")]
    [SerializeField] private EpisodeResultColorNotifier _colorNotifier;
    [SerializeField] private SpaceShipLabelDisplayer _labelDisplayer;

    [Header("Rewards")]
    [SerializeField] private float _winPoints = 1.0f;
    [SerializeField] private float _losePoints = -1.0f;

    [Space]
    [SerializeField] private float _collectValidTargetPoints = 0.2f;
    [SerializeField] private float _collectInvalidTargetPoints = -0.2f;

    [Space]
    [SerializeField] private float _timeReward = -0.005f;
    [SerializeField] private int _maxIterations = 1000;
    private int _currentIterations = 0;

    [Header("Settings")]
    [SerializeField] private Vector2 _xAxisRange = new Vector2(-7, 7);
    [SerializeField] private Vector2 _zAxisRange = new Vector2(-7, 7);


    // Setup methods
    private void Start()
    {
        _shipControl = GetComponent<SpaceShipControl>();
        _shipState = GetComponent<SpaceShipState>();
    }

    public override void OnEpisodeBegin()
    {
        _shipState.ResetPoints();
        _currentIterations = 0;

        List<Vector2> positions = GenerateVariousPoints(5, 1.5f);
        _shipControl.SetPosition(positions[0]);
        _validTargets[0].SetPosition(positions[1]);
        _validTargets[1].SetPosition(positions[2]);
        _invalidTargets[0].SetPosition(positions[3]);
        _invalidTargets[1].SetPosition(positions[4]);
    }

    private List<Vector2> GenerateVariousPoints(int numberOfPoints, float distance)
    {
        List<Vector2> points = new List<Vector2>();
        while (points.Count < numberOfPoints)
        {
            float x = UnityEngine.Random.Range(_xAxisRange.x, _xAxisRange.y);
            float y = UnityEngine.Random.Range(_zAxisRange.x, _zAxisRange.y);
            Vector2 newPoint = new Vector2(x, y);

            bool toClose = false;
            foreach (Vector2 point in points)
            {
                if (Vector2.Distance(point, newPoint) < distance)
                {
                    toClose = true;
                    break;
                }
            }

            if (!toClose)
            {
                points.Add(newPoint);
            }
        }

        return points;
    }


    // Observations
    public override void CollectObservations(VectorSensor sensor)
    {
        // Space ship position [2]
        sensor.AddObservation(_shipControl.NormalizedPosition);

        // Current score [1]
        sensor.AddObservation(_shipState.NormalizedPoints);

        // 2 x Valid target [2] + 2 x Invalid target [2] 
        // [8] Observations
        try
        {
            sensor.AddObservation(_validTargets[0].NormalizedPosition);
            sensor.AddObservation(_validTargets[1].NormalizedPosition);

            sensor.AddObservation(_invalidTargets[0].NormalizedPosition);
            sensor.AddObservation(_invalidTargets[1].NormalizedPosition);
        }
        catch (IndexOutOfRangeException ex)
        {
            Debug.LogError(ex.Message);
        }
    }


    // Actions
    public override void OnActionReceived(ActionBuffers actions)
    {
        AddReward(_timeReward);

        _currentIterations++;
        if (_currentIterations > _maxIterations)
        {
            FinishEpisode(false);
        }

        int moveX = actions.DiscreteActions[0] - 1;
        int moveZ = actions.DiscreteActions[1] - 1;
        _shipControl.Move(moveX, moveZ);
    }


    // Rewards
    private void OnTriggerEnter(Collider other)
    {
        // Collect valid target
        if (other.TryGetComponent<PointTarget>(out PointTarget validTarget))
        {
            SetReward(_collectValidTargetPoints);
            validTarget.SetPosition(GenerateVariousPoints(1, 0.0f)[0]);
            _shipState.AddPoints(1);
        }

        // Collect invalid target
        if (other.TryGetComponent<VirusTarget>(out VirusTarget invalidTarget))
        {
            SetReward(_collectInvalidTargetPoints);
            invalidTarget.SetPosition(GenerateVariousPoints(1, 0.0f)[0]);
            _shipState.AddPoints(-1);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Fell from platform
        if (other.TryGetComponent<Wall>(out Wall wall))
        {
            SetReward(_losePoints);
            FinishEpisode(false);
        }
    }

    private void Update()
    {
        // Won episode
        if (_shipState.IsPointsMax())
        {
            FinishEpisode(true);
        }

        // Fail episode
        if (_shipState.CollectedPoints < 0)
        {
            FinishEpisode(false);
        }
    }

    private void FinishEpisode(bool wonEpisode)
    {
        _shipState.ResetPoints();

        if (wonEpisode)
        {
            _colorNotifier.OnEpisodeWon();
            AddReward(_winPoints);
        }
        else
        {
            _colorNotifier.OnEpisodeLost();
            AddReward(_losePoints);
        }

        _labelDisplayer.DisplayRewards(GetCumulativeReward());
        EndEpisode();
    }


    // Testing
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
        if (Input.GetKey(KeyCode.RightArrow))
        {
            discreteActions[0] = 2;
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            discreteActions[0] = 0;
        }
        else
        {
            discreteActions[0] = 1;
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            discreteActions[1] = 2;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            discreteActions[1] = 0;
        }
        else
        {
            discreteActions[1] = 1;
        }
    }
}
