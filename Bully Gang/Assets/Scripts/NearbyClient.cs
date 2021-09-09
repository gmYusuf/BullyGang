using HmsPlugin;
using HuaweiMobileServices.Nearby;
using HuaweiMobileServices.Nearby.Discovery;
using HuaweiMobileServices.Nearby.Transfer;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class NearbyClient : HMSSingleton<NearbyClient>
{
    public GameObject player;

    private string playerPositionX = "0";
    private string serverPosition = "no"; 
    public NearbyManagerListener nearbyManagerListener;
    private string serverEndPointID;
    private int IsServerOrClient = 0;
    private bool IsConnected = false;
    // Next update in second
    private double nextUpdate = 0.5;
    public Action<string, ScanEndpointInfo> OnFound { get; set; }
    public Action<string, ConnectInfo> OnEstablish { get; set; }
    public Action<string, ConnectResult> OnResult { get; set; }
    public string PlayerPositionX { get => playerPositionX; set => playerPositionX = value; }
    public string ServerPosition { get => serverPosition; set => serverPosition = value; }
    private NearbyManagerDataListener dataCallBack;
    private NearbyManagerConnectListener connectCallBack;
    private string clientEndPointID;

    // Start is called before the first frame update
    void Start()
    {
        ScanEndpointCallback test;
        nearbyManagerListener = new NearbyManagerListener(this);
    }
 

    // Update is called once per frame
    void Update()
    {
        // If the next update is reached
        if (Time.time >= nextUpdate && IsConnected)
        {
            //Debug.Log(Time.time + ">=" + nextUpdate);
            // Change the next update (current second+1)
            nextUpdate = Mathf.FloorToInt(Time.time) + 0.5;
            PlayerPositionX = CrossPlatformInputManager.GetAxis("Horizontal").ToString();
            // Call your fonction
            UpdateEverySecond();
        }
    }
    // Update is called once per second
    void UpdateEverySecond()
    {
        if (IsServerOrClient == -1)
            HMSNearbyServiceManager.Instance.SendData(clientEndPointID, PlayerPositionX);
        else if (IsServerOrClient == 1)
            HMSNearbyServiceManager.Instance.SendData(serverEndPointID, PlayerPositionX);

    }
    public void AcceptConnectionRequest(string endpointId, ConnectInfo connectInfo)
    {
        Debug.Log("[HMS] NearbyManager AcceptConnectionRequest");

        dataCallBack = new NearbyManagerDataListener(this);
        HMSNearbyServiceManager.Instance.AcceptConnectionRequest(endpointId, connectInfo, dataCallBack);
    }
    public void InitiateConnection(string endpointId, ScanEndpointInfo scanEndpointInfo)
    {
        Debug.Log("[HMS] NearbyManager InitiateConnection");

        connectCallBack = new NearbyManagerConnectListener(this);
        HMSNearbyServiceManager.Instance.InitiateConnection(endpointId, scanEndpointInfo, connectCallBack);
    }
    public class NearbyManagerListener : IScanEndpointCallback
    {
        private readonly NearbyClient nearbyManagerObject;
 
        public NearbyManagerListener(NearbyClient nearbyManager)
        {
            Debug.Log("[HMS] NearbyManager NearbyManagerListener");

            nearbyManagerObject = nearbyManager;
        }

        public void onFound(string endpointId, ScanEndpointInfo discoveryEndpointInfo)
        {
            Debug.Log("[HMS] NearbyManager onFound");

            nearbyManagerObject.serverEndPointID = endpointId;
            nearbyManagerObject.IsServerOrClient = 1; // client
            nearbyManagerObject.InitiateConnection(endpointId, discoveryEndpointInfo);
            Debug.Log("[HMS] NearbyManager onFound2");

            nearbyManagerObject.OnFound?.Invoke(endpointId, discoveryEndpointInfo);
            Debug.Log("[HMS] NearbyManager onFound3");

        }

        public void onLost(string endpointId)
        {
            Debug.Log("[HMS] NearbyManager onLost");
            throw new System.NotImplementedException();
        }
    }
    public class NearbyManagerConnectListener : IConnectCallBack
    {
        private readonly NearbyClient nearbyManagerObject;
         public NearbyManagerConnectListener(NearbyClient nearbyManager)
        {
            Debug.Log("[HMS] NearbyManager NearbyManagerListener");

            nearbyManagerObject = nearbyManager;
        }
        public void onEstablish(string endpointId, ConnectInfo connectionInfo)
        {
            // Authenticating the Connection
            nearbyManagerObject.AcceptConnectionRequest(endpointId, connectionInfo);
            nearbyManagerObject.OnEstablish?.Invoke(endpointId, connectionInfo);
            Debug.Log("[HMS] NearbyManager onEstablish. Client accept connection from " + endpointId);
        }

        public void onResult(string endpointId, ConnectResult resultObject)
        {
            Debug.Log("[HMS] NearbyManager onResult");

            if (resultObject.Status.StatusCode == StatusCode.STATUS_SUCCESS)
            {
                /* The connection was established successfully, we can exchange data. */
                Debug.Log("[HMS] NearbyManager Connection Established. STATUS SUCCESS");
                HMSNearbyServiceManager.Instance.StopScanning();
                nearbyManagerObject.ServerConnected();
            }
            else if (resultObject.Status.StatusCode == StatusCode.STATUS_CONNECT_REJECTED)
            {
                Debug.Log("[HMS] NearbyManager Connection Established. STATUS REJECTED");
            }
            else
            {
                Debug.Log("[HMS] NearbyManager Connection Established.  status code" + resultObject.Status.StatusCode);
            }
            nearbyManagerObject.OnResult?.Invoke(endpointId, resultObject);
            nearbyManagerObject.serverEndPointID = endpointId;
            nearbyManagerObject.IsServerOrClient = 1; // client
        }

        public void onDisconnected(string p0)
        {
            throw new NotImplementedException();
        }
    }
    public class NearbyManagerDataListener : IDataCallback
    {
        private readonly NearbyClient nearbyManagerObject;
        private static string receivedMessage = "Receive Success";

        public NearbyManagerDataListener(NearbyClient nearbyManager)
        {
            Debug.Log("[HMS] NearbyManager NearbyManagerDataListener");

            nearbyManagerObject = nearbyManager;
        }

        public void onReceived(string endpointId, Data dataReceived)
        {
            if (dataReceived.DataType == Data.Type.BYTES)
            {
                string msg = System.Text.Encoding.UTF8.GetString(dataReceived.AsBytes);
                if (msg.Equals(receivedMessage))
                {
                    Debug.Log("[HMS] NearbyManager Received ACK. Send next.");
                }
                Debug.Log("[HMS] NearbyManager Received ACK. Send next." + msg);
                nearbyManagerObject.ServerPosition = msg;
            }
        }

        public void onTransferUpdate(string endpointId, TransferStateUpdate update)
        {
            Debug.Log("[HMS] NearbyManager onTransferUpdate");
        }
    }

    private void ServerConnected()
    {
        //HMSNearbyServiceManager.Instance.SendData(PlayerPositionX);
        Debug.Log("[HMS] NearbyManager Server Connected Send Position " + PlayerPositionX);
        GameObject broadCastButton = GameObject.Find("Server");
        GameObject scanButton = GameObject.Find("Connect");
        broadCastButton.SetActive(false);
        scanButton.SetActive(false);
        IsConnected = true;
        Debug.Log("[HMS] NearbyManager Button Hide ");

    }

}
