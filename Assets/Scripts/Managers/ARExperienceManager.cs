using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARPlaneManager))]
public class ARExperienceManager : MonoBehaviour
{
    [SerializeField]
    private UnityEvent OnInitialized = null;

    [SerializeField]
    private UnityEvent OnRetarted = null;

    private ARPlaneManager arPlaneManager = null;

    private bool Initialized { get; set; }
    
    void Awake() 
    {
        arPlaneManager = GetComponent<ARPlaneManager>();
        arPlaneManager.planesChanged += PlanesChanged;

        #if UNITY_EDITOR
            OnInitialized?.Invoke();
            Initialized = true;
            arPlaneManager.enabled = false;
        #endif
    }

    void PlanesChanged(ARPlanesChangedEventArgs args)
    {
        if(!Initialized)
        {
            Activate();
        }
    }

    private void Activate()
    {
        ARDebugManager.Instance.LogInfo("Activate Experience");
        OnInitialized?.Invoke();
        Initialized = true;
        arPlaneManager.enabled = false;
    }

    public void Restart()
    {
        ARDebugManager.Instance.LogInfo("Restart Experience");
        OnRetarted?.Invoke();
        Initialized = false;
        arPlaneManager.enabled = true;
    }
}
