using UnityEngine;

public class RoamingRobotController : MonoBehaviour
{
    [Header("Einstellungen")]
    public float rotationSpeed = 45f;
    public float moveSpeed = 3.0f;
    
    public float zoneLeftThreshold = 0.2f;
    public float zoneRightThreshold = 0.8f;

    [Header("Ziele")]
    public Transform targetWaypoint;
    public GameObject successPanel;  
    
    private float accumulatedRotation = 0f;
    private bool hasReachedAngle = false;
    private bool hasFinished = false;

    void OnEnable()
    {
        accumulatedRotation = 0f;
        hasReachedAngle = false;
        hasFinished = false;
        if(successPanel) successPanel.SetActive(false);
    }

    void Update()
    {
        if (hasFinished) return; 

        if (!hasReachedAngle) HandleRotation();
        else HandleMovement();
    }

    void HandleRotation()
    {
        float rawX = SkywriterInputManager.Instance.InputX;

        if (rawX < zoneLeftThreshold)
        {
            float rotAmount = -rotationSpeed * Time.deltaTime;
            transform.Rotate(0, rotAmount, 0);
            accumulatedRotation += rotAmount;
        }
        else if (rawX > zoneRightThreshold)
        {
            float rotAmount = rotationSpeed * Time.deltaTime;
            transform.Rotate(0, rotAmount, 0);
            accumulatedRotation += rotAmount;
        }

        if (accumulatedRotation >= 90f)
        {
            hasReachedAngle = true;
        }
    }

    void HandleMovement()
{
    transform.position = Vector3.MoveTowards(transform.position, targetWaypoint.position, moveSpeed * Time.deltaTime);

    float distance = Vector3.Distance(transform.position, targetWaypoint.position);
    
    if (distance < 0.5f)
    {
        FinishLevel();
    }
}

    void FinishLevel()
    {
        hasFinished = true;
        if (successPanel) successPanel.SetActive(true);
        
        GameManager.Instance.LevelCompleted();
    }
}