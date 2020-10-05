using UnityEngine;

[CreateAssetMenu(fileName = "LineSettings", menuName = "Create Line Settings", order = 0)]
public class LineSettings : ScriptableObject 
{
    public string lineTagName = "Line";
    
    public Color startColor = Color.white;

    public Color endColor = Color.white;

    public float startWidth = 0.01f;

    public float endWidth = 0.01f;

    public float distanceFromCamera = 0.3f;

    public Material defaultMaterial;

    public int cornerVertices = 5;

    public int endCapVertices = 5;

    [Range(0, 1.0f)]
    public float minDistanceBeforeNewPoint = 0.001f;

    [Header("Tolerance Options")]
    public bool allowSimplification = false;

    public float tolerance = 0.001f;
    
    public float applySimplifyAfterPoints = 20.0f;

    public bool allowMultiTouch = true;
}