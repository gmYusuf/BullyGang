using HmsPlugin;
using HuaweiMobileServices.Nearby;
using HuaweiMobileServices.Nearby.Discovery;
using HuaweiMobileServices.Nearby.Transfer;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UIElements;
 
public class GameManager :   HMSSingleton<GameManager>
{
    public GameObject playerLeft, playerRight;
    Animator playerAnimator;

    public bool PlayerSide = false;
 
    public void BroadCasting()
    {
        HMSNearbyServiceManager.Instance.SendFilesInner(NearbyServer.Instance.nearbyManagerListener);
        playerLeft = GameObject.Find("Deer L");
        PlayerSide = false;
    }
    public void DoStartScan()
    { 
        HMSNearbyServiceManager.Instance.OnScanResult(NearbyClient.Instance.nearbyManagerListener);
        playerRight = GameObject.Find("Deer R");
        PlayerSide = true;
    }

    public void StopBroadCasting()
    {
        HMSNearbyServiceManager.Instance.StopBroadCasting();
    }
    public void StopScanning()
    {
        HMSNearbyServiceManager.Instance.StopScanning();
    }
    public void DisconnectAllConnection()
    {
        HMSNearbyServiceManager.Instance.DisconnectAllConnection();
    }
    public void FightTrigger()
    {
        if (PlayerSide)
        {
            RightFightTrigger();
        }
        else
        {
            LeftFightTrigger();
        }

    }
    public void LeftFightTrigger()
    {
        playerAnimator = playerRight.GetComponent<Animator>();
        playerAnimator.SetTrigger("fight");
    }
    public void RightFightTrigger()
    {
        playerAnimator = playerLeft.GetComponent<Animator>();
        playerAnimator.SetTrigger("fight");
    }

    private void Start()
    {
 
 
    }
 
}
