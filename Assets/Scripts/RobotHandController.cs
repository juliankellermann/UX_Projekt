using UnityEngine;

public class RobotHandController : MonoBehaviour
{
    [Header("Steuerung & Geschwindigkeit")]
    public float moveSpeed = 0.5f;   
    public float liftSpeed = 0.01f;
    public float gripSpeed = 3.0f; 
    public float deadZone = 0.15f; // Gilt jetzt für Skywriter-Sensibilität

    [Header("Finger-Setup (Objekte zuweisen)")]
    public Transform fingerLeft;
    public Transform fingerRight;
    public Transform thumbBase;
    
    public Transform fingerLeftTip;
    public Transform fingerRightTip;
    public Transform thumbTip;

    [Header("Rotationseinstellungen")]
    public Vector3 fingerCloseOffset = new Vector3(0, 45, 0); 
    public Vector3 thumbCloseOffset = new Vector3(0, -45, 0);
    public float tipRotationMultiplier = 1.2f;

    private Quaternion startRotLeft, startRotRight, startRotThumb;
    private Quaternion startRotLeftTip, startRotRightTip, startRotThumbTip;

    [Header("Greif-Logik")]
    public Transform grabPoint;
    public float grabRadius = 0.15f; 
    public LayerMask packageLayer;
    private Transform heldObject;
    private Rigidbody rb;
    
    // Speichert den aktuellen Zustand (0 = offen, 1 = geschlossen)
    private float currentGrip = 0f; 

    // Cooldown Timer
    private float grabCooldown = 0f; 

    [Header("Begrenzung (Welt-Koordinaten relative zum Start)")]
    public Vector3 relativeMin = new Vector3(-3, -1, -5); 
    public Vector3 relativeMax = new Vector3(3, 3, 5);
    private Vector3 startPosition;

    void Awake()
    {
        startPosition = transform.position;
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        if (fingerLeft) startRotLeft = fingerLeft.localRotation;
        if (fingerRight) startRotRight = fingerRight.localRotation;
        if (thumbBase) startRotThumb = thumbBase.localRotation;

        if (fingerLeftTip) startRotLeftTip = fingerLeftTip.localRotation;
        if (fingerRightTip) startRotRightTip = fingerRightTip.localRotation;
        if (thumbTip) startRotThumbTip = thumbTip.localRotation;
    }

    void Update()
    {
        // Cooldown runterzählen
        if (grabCooldown > 0f) grabCooldown -= Time.deltaTime;


        float rawX = 0.5f;
        float rawY = 0.5f;
        float rawZ = 0.5f;
        bool isTapped = false;

        // Versuche Daten vom Skywriter zu holen
        if (SkywriterInputManager.Instance != null)
        {
            rawX = SkywriterInputManager.Instance.InputX; // 0 bis 1
            rawY = SkywriterInputManager.Instance.InputY; // 0 bis 1
            rawZ = SkywriterInputManager.Instance.InputZ; // 0 bis 1
            isTapped = SkywriterInputManager.Instance.IsTapped;
        }

        float inputHorizontal = (rawX - 0.5f) * 2f; 
        float inputVertical = (rawY - 0.5f) * 2f;
        float inputLift = (rawZ - 0.5f) * 2f;


        if (Mathf.Abs(inputHorizontal) < deadZone) inputHorizontal = 0f;
        if (Mathf.Abs(inputVertical) < deadZone) inputVertical = 0f;
        if (Mathf.Abs(inputLift) < deadZone) inputLift = 0f;



        Vector3 moveDir = Vector3.zero;
        moveDir += Vector3.right * inputVertical * moveSpeed;   // Y-Input steuert Vor/Zurück
        moveDir += Vector3.back * inputHorizontal * moveSpeed;  // X-Input steuert Links/Rechts
        moveDir += Vector3.up * inputLift * liftSpeed;          // Z-Input steuert Höhe

        transform.Translate(moveDir * Time.deltaTime, Space.World);
        ApplyBounds();


        bool isCarrying = (heldObject != null);

        if (isCarrying)
        {

            currentGrip = 1.0f; 

            
            if (isTapped || Input.GetKeyDown(KeyCode.Space)) 
            {
                TryDrop();
            }
        }
        else
        {
            
            if (inputHorizontal < -0.1f)
            {
                currentGrip += Time.deltaTime * gripSpeed;
            }
            
            else if (inputHorizontal > 0.1f)
            {
                currentGrip -= Time.deltaTime * gripSpeed;
            }
            
            currentGrip = Mathf.Clamp01(currentGrip);

            
            if (currentGrip > 0.9f && grabCooldown <= 0f) 
            {
                TryGrab();
            }
        }

        AnimateFingers(currentGrip);
    }

    void AnimateFingers(float strength)
    {
        // --- Linker Finger ---
        Quaternion fingerRot = Quaternion.Euler(fingerCloseOffset * strength);
        Quaternion fingerTipRot = Quaternion.Euler(fingerCloseOffset * strength * tipRotationMultiplier);

        if (fingerLeft) fingerLeft.localRotation = startRotLeft * fingerRot;
        if (fingerLeftTip) fingerLeftTip.localRotation = startRotLeftTip * fingerTipRot;

        // --- Rechter Finger ---
        if (fingerRight)
        {
            Quaternion fingerRotRight = Quaternion.Euler(new Vector3(fingerCloseOffset.x, -fingerCloseOffset.y, fingerCloseOffset.z) * strength);
            fingerRight.localRotation = startRotRight * fingerRotRight;
        }
        if (fingerRightTip)
        {
            Quaternion fingerTipRotRight = Quaternion.Euler(new Vector3(fingerCloseOffset.x, -fingerCloseOffset.y, fingerCloseOffset.z) * strength * tipRotationMultiplier);
            fingerRightTip.localRotation = startRotRightTip * fingerTipRotRight;
        }

        // --- Daumen ---
        Quaternion thumbRot = Quaternion.Euler(thumbCloseOffset * strength);
        Quaternion thumbTipRot = Quaternion.Euler(thumbCloseOffset * strength * tipRotationMultiplier);

        if (thumbBase) thumbBase.localRotation = startRotThumb * thumbRot;
        if (thumbTip) thumbTip.localRotation = startRotThumbTip * thumbTipRot;
    }

    void TryGrab()
    {
        Collider[] hits = Physics.OverlapSphere(grabPoint.position, grabRadius, packageLayer);
        if (hits.Length > 0)
        {
            heldObject = hits[0].transform;
            if(heldObject.GetComponent<Rigidbody>()) heldObject.GetComponent<Rigidbody>().isKinematic = true;
            heldObject.SetParent(grabPoint);
        }
    }

    void TryDrop()
    {
        if (heldObject == null) return;
        
        heldObject.SetParent(null);
        if(heldObject.GetComponent<Rigidbody>()) heldObject.GetComponent<Rigidbody>().isKinematic = false;
        heldObject = null;

        grabCooldown = 1.0f;
        currentGrip = 0.0f; 
    }

    void ApplyBounds()
    {
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, startPosition.x + relativeMin.x, startPosition.x + relativeMax.x);
        pos.y = Mathf.Clamp(pos.y, startPosition.y + relativeMin.y, startPosition.y + relativeMax.y);
        pos.z = Mathf.Clamp(pos.z, startPosition.z + relativeMin.z, startPosition.z + relativeMax.z);
        transform.position = pos;
    }

    void OnDrawGizmos()
    {
        if (grabPoint != null)
        {
            Gizmos.color = (grabCooldown > 0) ? Color.red : Color.yellow; 
            Gizmos.DrawWireSphere(grabPoint.position, grabRadius);
        }
    }
}