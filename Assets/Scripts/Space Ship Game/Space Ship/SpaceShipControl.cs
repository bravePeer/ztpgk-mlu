using UnityEngine;

public class SpaceShipControl : MonoBehaviour
{
    [Header("Settings")]
    [Range(0.0f, 5.0f)] [SerializeField] private float _moveSpeed = 1.0f;

    [Space]
    [SerializeField] private Vector2 _xAxisRange = new Vector2(-7, 7);
    [SerializeField] private Vector2 _zAxisRange = new Vector2(-7, 7);

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

    public void Move(float xDirection, float zDirection)
    {
        transform.localPosition += new Vector3(xDirection, 0, zDirection) * Time.deltaTime * _moveSpeed;
    }

    public void SetPosition(Vector2 position)
    {
        transform.localPosition = new Vector3(
                Mathf.Clamp(position.x, _xAxisRange.x, _xAxisRange.y),
                0.0f,
                Mathf.Clamp(position.y, _zAxisRange.x, _zAxisRange.y)
        );
    }
}
