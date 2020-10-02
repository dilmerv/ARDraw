using System.Collections.Generic;
using DilmerGames.Core.Singletons;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARAnchorManager))]
public class ARDrawManager : Singleton<ARDrawManager>
{
    [SerializeField] 
    private float distanceFromCamera = 0.3f;

    [SerializeField]
    private Material defaultColorMaterial;

    [SerializeField]
    private int cornerVertices = 5;

    [SerializeField]
    private int endCapVertices = 5;

    [Header("Tolerance Options")]
    [SerializeField]
    private bool allowSimplification = false;

    [SerializeField]
    private float tolerance = 0.001f;
    [SerializeField] 
    private float applySimplifyAfterPoints = 20.0f;

    [SerializeField, Range(0, 1.0f)]
    private float minDistanceBeforeNewPoint = 0.01f;

    [SerializeField]
    private UnityEvent OnDraw;

    [SerializeField]
    private ARAnchorManager anchorManager;

    [SerializeField] 
    private Camera arCamera;

    [SerializeField]
    private Color defaultColor = Color.white;

    private Color randomStartColor = Color.white;
    private Color randomEndColor = Color.white;

    [SerializeField]
    private float lineWidth = 0.05f;

    private LineRenderer prevLineRender;
    private LineRenderer currentLineRender;

    private List<ARAnchor> anchors = new List<ARAnchor>();

    private List<LineRenderer> lines = new List<LineRenderer>();

    private int positionCount = 0; // 2 by default

    private Vector3 prevPointDistance = Vector3.zero;

    private bool CanDraw { get; set; }

    void Update ()
    {
        #if !UNITY_EDITOR    
        if (Input.touchCount > 0)
            DrawOnTouch();
        #else
        if(Input.GetMouseButton(0))
            DrawOnMouse();
        else
        {
            prevLineRender = null;
        }
        #endif
        
	}

    public void AllowDraw(bool isAllow)
    {
        CanDraw = isAllow;
    }

    private void SetLineSettings(LineRenderer currentLineRenderer)
    {
        currentLineRenderer.startWidth = lineWidth;
        currentLineRenderer.endWidth = lineWidth;
        currentLineRenderer.numCornerVertices = cornerVertices;
        currentLineRenderer.numCapVertices = endCapVertices;
        if(allowSimplification) currentLineRenderer.Simplify(tolerance);
        currentLineRenderer.startColor = randomStartColor;
        currentLineRenderer.endColor = randomEndColor;
    }

    void DrawOnTouch()
    {
        if(!CanDraw) return;

        Touch touch = Input.GetTouch(0);
        Vector3 touchPosition = arCamera.ScreenToWorldPoint(new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, distanceFromCamera));

        if(touch.phase == TouchPhase.Began)
        {
            OnDraw?.Invoke();
            
            ARAnchor anchor = anchorManager.AddAnchor(new Pose(touchPosition, Quaternion.identity));
            if (anchor == null) 
                Debug.LogError("Error creating reference point");
            else 
            {
                anchors.Add(anchor);
                ARDebugManager.Instance.LogInfo($"Anchor created & total of {anchors.Count} anchor(s)");
            }
            AddNewLineRenderer(anchor,touchPosition);
        }
        else 
        {
            UpdateLine(touchPosition);
        }
    }

    void DrawOnMouse()
    {
        if(!CanDraw) return;

        Vector3 mousePosition = arCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distanceFromCamera));

        if(Input.GetMouseButton(0))
        {
            OnDraw?.Invoke();

            if(prevLineRender == null)
            {
                AddNewLineRenderer(null, mousePosition);
            }
            else 
            {
                UpdateLine(mousePosition);
            }
        }
    }

    void UpdateLine(Vector3 touchPosition)
    {   
        if(prevPointDistance == null)
            prevPointDistance = touchPosition;

        if(prevPointDistance != null && Mathf.Abs(Vector3.Distance(prevPointDistance, touchPosition)) >= minDistanceBeforeNewPoint)
        {
            prevPointDistance = touchPosition;
            AddPoint(prevPointDistance);
        }
    }

    void AddPoint(Vector3 position)
    {
        positionCount++;
        currentLineRender.positionCount = positionCount;
        // index 0 positionCount must be - 1
        currentLineRender.SetPosition(positionCount - 1, position);
        // applies simplification if reminder is 0
        if(currentLineRender.positionCount % applySimplifyAfterPoints == 0 && allowSimplification)
        {
            currentLineRender.Simplify(tolerance);
        }
    }

    void AddNewLineRenderer(ARAnchor arAnchor, Vector3 touchPosition)
    {
        positionCount = 2;
        GameObject go = new GameObject($"LineRenderer_{lines.Count}");
        
        go.transform.parent = arAnchor?.transform ?? transform;
        go.transform.position = touchPosition;
        go.tag = "Line";
        LineRenderer goLineRenderer = go.AddComponent<LineRenderer>();
        goLineRenderer.startWidth = lineWidth;
        goLineRenderer.endWidth = lineWidth;
        goLineRenderer.material = defaultColorMaterial;
        goLineRenderer.useWorldSpace = true;
        goLineRenderer.positionCount = positionCount;
        goLineRenderer.numCapVertices = 90;
        goLineRenderer.SetPosition(0, touchPosition);
        goLineRenderer.SetPosition(1, touchPosition);

        SetLineSettings(goLineRenderer);

        currentLineRender = goLineRenderer;

        prevLineRender = currentLineRender;
        
        lines.Add(goLineRenderer);

        ARDebugManager.Instance.LogInfo($"New line renderer created");
    }

    GameObject[] GetAllLinesInScene()
    {
        return GameObject.FindGameObjectsWithTag("Line");
    }

    public void ClearLines()
    {
        GameObject[] lines = GetAllLinesInScene();
        foreach (GameObject currentLine in lines)
        {
            LineRenderer line = currentLine.GetComponent<LineRenderer>();
            Destroy(currentLine);
        }
    }

    private Color GetRandomColor() => Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
}