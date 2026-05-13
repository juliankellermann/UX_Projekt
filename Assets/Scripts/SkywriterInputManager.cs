using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Globalization;

public class SkywriterInputManager : MonoBehaviour
{
    public static SkywriterInputManager Instance;

    
    public float InputX { get; private set; } = 0.5f;
    public float InputY { get; private set; } = 0.5f;
    public float InputZ { get; private set; } = 0.5f;
    public bool IsTapped { get; private set; } = false;


    [Header("Debug Einstellungen")]
    public bool showDebugLogs = true; 

    
    private float targetX = 0.5f;
    private float targetY = 0.5f;
    private float targetZ = 0.5f;

    public float smoothingSpeed = 10.0f; 

    UdpClient client;
    Thread receiveThread;
    public int port = 5005;
    private bool isRunning = true;
    private bool tapReceivedInThread = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
        
        receiveThread = new Thread(ReceiveData);
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }

    void Update()
    {
        
        float dt = Time.deltaTime;
        InputX = Mathf.Lerp(InputX, targetX, dt * smoothingSpeed);
        InputY = Mathf.Lerp(InputY, targetY, dt * smoothingSpeed);
        InputZ = Mathf.Lerp(InputZ, targetZ, dt * smoothingSpeed);

       
        if (tapReceivedInThread)
        {
            tapReceivedInThread = false;
            IsTapped = true;
            CancelInvoke("ResetTap");
            Invoke("ResetTap", 0.1f);
        }
    }

    void ResetTap() => IsTapped = false;

    private void ReceiveData()
    {
        
        try 
        { 
            client = new UdpClient(port); 
        } 
        catch (System.Exception e) 
        { 
            Debug.LogError("UDP Start Fehler: " + e.Message); 
            return;
        }

        while (isRunning)
        {
            try
            {
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = client.Receive(ref anyIP);
                string text = Encoding.UTF8.GetString(data);
                ParseData(text.Trim());
            }
            catch (System.Exception e) 
            { 
                if(!(e is ThreadAbortException)) Debug.LogWarning("UDP: " + e.Message); 
            }
        }
    }

    void ParseData(string text)
    {
        try
        {
            string[] parts = text.Split(',');
            if (parts.Length == 0) return;

            string command = parts[0];

            if (command == "MOVE" && parts.Length >= 4)
            {
                targetX = float.Parse(parts[1], CultureInfo.InvariantCulture);
                targetY = float.Parse(parts[2], CultureInfo.InvariantCulture);
                targetZ = float.Parse(parts[3], CultureInfo.InvariantCulture);

                
                if (showDebugLogs)
                {
                    
                    Debug.Log($"UDP Empfangen -> X: {targetX} | Y: {targetY} | Z: {targetZ}");
                }
                
            }
            else if (command == "TAP")
            {
                tapReceivedInThread = true;
                
                if (showDebugLogs)
                {
                    Debug.Log("UDP Empfangen -> TAP");
                }
            }
        }
        catch (System.Exception ex) 
        { 
            if (showDebugLogs) Debug.LogWarning("Parse Error: " + ex.Message);
        }
    }

    void OnDestroy()
    {
        isRunning = false;
        if (client != null) client.Close();
        if (receiveThread != null && receiveThread.IsAlive) receiveThread.Abort();
    }
}