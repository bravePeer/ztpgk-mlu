using UnityEngine;

public class SpaceShipState : MonoBehaviour
{
    [Header("State")]
    [SerializeField] private int _collectedPoints;
    [SerializeField] private int _maxPoints;
    
    public int CollectedPoints { get => _collectedPoints; private set => _collectedPoints = value; }
    public int MaxPoints { get => _maxPoints; private set => _maxPoints = value; }
    public float NormalizedPoints { get => Normalization.NormalizeFloat(CollectedPoints, 0.0f, MaxPoints); }

    public void AddPoints(int points)
    {
        _collectedPoints += points; 
    }

    public void ResetPoints()
    {
        _collectedPoints = 0;
    }

    public bool IsPointsMax()
    {
        return _collectedPoints >= _maxPoints;
    }
}
