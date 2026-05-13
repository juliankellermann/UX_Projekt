using UnityEngine;
using System.Collections;

public class RobotPatrol : MonoBehaviour
{
    [Header("Bewegung")]
    public Transform[] waypoints;
    private int currentPoint = 0;
    public float speed = 2f;

    [Header("Event 1: Paket (Arm)")]
    public GameObject eventMarker; 
    public float minWaitTime = 5f;
    public float maxWaitTime = 10f;
    
    [Header("Event 2: Festgefahrener Roboter (Roaming)")]
    public GameObject roamingMarker; 
    public float roamingMinWait = 8f; 
    public float roamingMaxWait = 15f;

    void Start()
    {

        if (eventMarker != null) eventMarker.SetActive(false);
        if (roamingMarker != null) roamingMarker.SetActive(false);


        StartCoroutine(EventRoutinePackagel());
        StartCoroutine(EventRoutineRoaming());
    }

    void Update()
    {

        if (waypoints.Length == 0) return;

        Transform target = waypoints[currentPoint];
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        transform.LookAt(target);

        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            currentPoint = (currentPoint + 1) % waypoints.Length;
        }
    }

 
    IEnumerator EventRoutinePackagel()
    {
        while (true) 
        {
            float waitTime = Random.Range(minWaitTime, maxWaitTime);
            yield return new WaitForSeconds(waitTime);

            if (eventMarker != null)
            {
                eventMarker.SetActive(true);
                
            }

            
            yield return new WaitForSeconds(20f); 
            
            
        }
    }

    
    IEnumerator EventRoutineRoaming()
    {
        while (true)
        {
            
            float waitTime = Random.Range(roamingMinWait, roamingMaxWait);
            yield return new WaitForSeconds(waitTime);

            if (roamingMarker != null)
            {
                roamingMarker.SetActive(true);
                
            }

            yield return new WaitForSeconds(25f);
            
        }
    }
}