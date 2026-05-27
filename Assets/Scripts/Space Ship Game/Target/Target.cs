using UnityEngine;

public class Target : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _defaultHeight;
    [SerializeField] private Vector2 _xAxisRange;
    [SerializeField] private Vector2 _zAxisRange;

    public Vector2 NormalizedPosition
    {
        get
        {
            return Normalization.NormalizeVector2(
                    new Vector2(transform.localPosition.x, transform.localPosition.z),
                    new Vector2(_xAxisRange.x, _zAxisRange.x),
                    new Vector2(_xAxisRange.y, _zAxisRange.y)
                );
        }
    }

    public void SetPosition(Vector2 position)
    {
        transform.localPosition = new Vector3(position.x, _defaultHeight, position.y);
    }
}
