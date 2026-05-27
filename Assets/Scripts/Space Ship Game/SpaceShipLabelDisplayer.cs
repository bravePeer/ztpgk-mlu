using TMPro;
using UnityEngine;

public class SpaceShipLabelDisplayer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpaceShipState _shipState;
    [SerializeField] private TMP_Text _scoreLabel;
    [SerializeField] private TMP_Text _rewardsLabel;

    private void Update()
    {
        _scoreLabel.text = $"{_shipState.CollectedPoints}/{_shipState.MaxPoints}";
    }

    public void DisplayRewards(float rewards)
    {
        _rewardsLabel.text = rewards.ToString();
    }
}
