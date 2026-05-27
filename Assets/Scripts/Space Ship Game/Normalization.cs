using UnityEngine;

public class Normalization
{
    public static Vector3 NormalizeVector3(Vector3 data, Vector3 min, Vector3 max)
    {
        float x = NormalizeFloat(data.x, min.x, max.x);
        float y = NormalizeFloat(data.y, min.y, max.y);
        float z = NormalizeFloat(data.z, min.z, max.z);
        return new Vector3(x, y, z);
    }
    
    public static Vector2 NormalizeVector2(Vector2 data, Vector2 min, Vector2 max)
    {
        float x = NormalizeFloat(data.x, min.x, max.x);
        float y = NormalizeFloat(data.y, min.y, max.y);
        return new Vector2(x, y);
    }

    public static float NormalizeFloat(float data, float min, float max)
    {
        return (data - min) / (max - min);
    }
}