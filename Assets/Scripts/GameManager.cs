using UnityEngine;
using UnityEngine.SceneManagement; 
using System.Collections; 

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Kameras & Objekte")]
    public Camera camera2D;         
    public Camera camera3D;         
    public GameObject roamingRobotObject; 

    [Header("Marker Einstellungen")]
    public GameObject markerArm;     
    public GameObject markerArm2;    
    public GameObject markerRoaming; 
    
    
    public float timeUntilMarkersAppear = 3.0f; 

    [Header("Controller")]
    public CursorController2D cursorScript;
    public RobotHandController handScript;
    
    private bool resetTimerStarted = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        ResetTo2DView();
        
        
        if (markerArm) markerArm.SetActive(false);
        if (markerArm2) markerArm2.SetActive(false);
        if (markerRoaming) markerRoaming.SetActive(false);

        
        StartCoroutine(SpawnMarkersRoutine());
    }

   
    IEnumerator SpawnMarkersRoutine()
    {
        yield return new WaitForSeconds(timeUntilMarkersAppear);
        
        if (markerArm) markerArm.SetActive(true);
        if (markerArm2) markerArm2.SetActive(true);
        if (markerRoaming) markerRoaming.SetActive(true);
        
        
    }

void ResetTo2DView()
    {
        camera2D.gameObject.SetActive(true);
        camera3D.gameObject.SetActive(false);
        if(roamingRobotObject) roamingRobotObject.SetActive(false);

        // ÄNDERUNG: Statt .enabled = true nutzen wir SetActive(true)
        if(cursorScript) cursorScript.gameObject.SetActive(true); 

        if(handScript) handScript.enabled = false;
        
        resetTimerStarted = false;
    }
    public void StartEventSequence(GameObject clickedObject)
    {
        
        if (clickedObject == markerArm || clickedObject == markerArm2) 
        {
            cursorScript.enabled = false;
            StartArmEvent();
        }
        else if (clickedObject == markerRoaming) 
        {
            cursorScript.enabled = false;
            StartRoamingEvent();
        }
        
    }

    void StartArmEvent()
    {
        
        if(markerArm) markerArm.SetActive(false);
        if(markerArm2) markerArm2.SetActive(false);

        camera2D.gameObject.SetActive(false);
        camera3D.gameObject.SetActive(true);
        handScript.enabled = true;
    }

    void StartRoamingEvent()
    {
        if(markerRoaming) markerRoaming.SetActive(false);

        camera2D.gameObject.SetActive(false);
        if(roamingRobotObject) roamingRobotObject.SetActive(true); 
    }

    public void LevelCompleted()
    {
        if (resetTimerStarted) return; 
        
        resetTimerStarted = true;
        Debug.Log("Level geschafft! Neustart in 10 Sekunden...");
        StartCoroutine(ResetGameRoutine());
    }

    IEnumerator ResetGameRoutine()
    {
        yield return new WaitForSeconds(10f); 
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}