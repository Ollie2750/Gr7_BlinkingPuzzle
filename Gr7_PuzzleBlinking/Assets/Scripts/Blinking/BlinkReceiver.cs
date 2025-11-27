using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class BlinkReceiver : MonoBehaviour
{
    [Header("Network Settings")]
    [SerializeField] private int port = 5065;

    [Header("Events")]
    public UnityEngine.Events.UnityEvent OnBlinkStart;
    public UnityEngine.Events.UnityEvent OnBlinkEnd;
    public UnityEngine.Events.UnityEvent OnLongBlink;

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;

    private UdpClient udpClient;
    private Thread receiveThread;
    private bool isRunning = false;

    // Latest data
    private string lastMessage = "";
    private bool hasNewMessage = false;
    private float lastBlinkDuration = 0f;

    [SerializeField] private TimeTravel timeTravel;
    [SerializeField] private TimeFreezeAbility timeFreezeAbility;

    private ClientNetworkTransform clientNetworkTransform;

    void Start()
    {
        clientNetworkTransform = GetComponent<ClientNetworkTransform>();

        if (!clientNetworkTransform.IsOwner) return;

        StartUDPListener();
    }

    void StartUDPListener()
    {
        try
        {
            udpClient = new UdpClient(port);
            receiveThread = new Thread(new ThreadStart(ReceiveData));
            receiveThread.IsBackground = true;
            receiveThread.Start();
            isRunning = true;

            if (showDebugLogs)
                Debug.Log($"UDP Listener started on port {port}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to start UDP listener: {e.Message}");
        }
    }

    void ReceiveData()
    {
        while (isRunning)
        {
            try
            {
                IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = udpClient.Receive(ref remoteEndPoint);
                string message = Encoding.UTF8.GetString(data);

                lock (this)
                {
                    lastMessage = message;
                    hasNewMessage = true;
                }
            }
            catch (Exception e)
            {
                if (isRunning)
                    Debug.LogError($"Error receiving data: {e.Message}");
            }
        }
    }

    void Update()
    {
        if (!clientNetworkTransform.IsOwner) return;

        // Process messages on the main thread
        if (hasNewMessage)
        {
            string message;
            lock (this)
            {
                message = lastMessage;
                hasNewMessage = false;
            }

            ProcessMessage(message);
        }
        if (!timeTravel || !timeFreezeAbility)
        {
            timeTravel = GetComponent<TimeTravel>();
            Debug.Log("TimeTravel component assigned in Update");
            timeFreezeAbility = GetComponent<TimeFreezeAbility>();
            Debug.Log("TimeFreezeAbility component assigned in Update");
        }
    }

    void ProcessMessage(string message)
    {
        if (showDebugLogs)
            Debug.Log($"Received: {message}");

        if (message == "BLINK_START")
        {
            OnBlinkStart?.Invoke();
            if (showDebugLogs)
                Debug.Log("Blink started!");
        }
        else if (message.StartsWith("BLINK_END:"))
        {
            string[] parts = message.Split(':');
            if (parts.Length > 1 && float.TryParse(parts[1], out float duration))
            {
                lastBlinkDuration = duration;
                OnBlinkEnd?.Invoke();

                if (showDebugLogs)
                    Debug.Log($"Blink ended. Duration: {duration}s");
            }
        }
        else if (message.StartsWith("LONG_BLINK:"))
        {
            string[] parts = message.Split(':');
            if (parts.Length > 1 && float.TryParse(parts[1], out float duration))
            {
                lastBlinkDuration = duration;
                OnLongBlink?.Invoke();
                ActivateAbility();

                if (showDebugLogs)
                    Debug.Log($"Long blink detected! Duration: {duration}s");
            }
        }
    }

    void ActivateAbility()
    {
        if (timeTravel != null && timeTravel.enabled)
        {
            Debug.Log("TimeTravel is assigned and enabled");
            timeTravel.ActivateTimeTravel();
        }
        else if (timeFreezeAbility != null && timeFreezeAbility.enabled)
        {
            Debug.Log("TimeFreezeAbility is assigned and enabled");
            timeFreezeAbility.ActivateFreeze();
        }
        else
        {
            Debug.LogWarning("No enabled ability assigned!");
        }
    }

    public float GetLastBlinkDuration()
    {
        return lastBlinkDuration;
    }

    void OnApplicationQuit()
    {
        if (!clientNetworkTransform.IsOwner) return;


        isRunning = false;

        if (receiveThread != null && receiveThread.IsAlive)
        {
            receiveThread.Abort();
        }

        if (udpClient != null)
        {
            udpClient.Close();
        }
    }

    void OnDestroy()
    {
        if (!clientNetworkTransform.IsOwner) return;


        OnApplicationQuit();
    }
}