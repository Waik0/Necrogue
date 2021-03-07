using System;
using System.Collections;
using System.Collections.Generic;
using Steamworks;
using UnityEngine;

public class SteamMatchingManager : MonoBehaviour
{
    protected static SteamMatchingManager _instance;
    protected static SteamMatchingManager Instance {
        get {
            if (_instance == null) {
                _instance = new GameObject("SteamMatchingManager").AddComponent<SteamMatchingManager>();
                return _instance;
            }
            else {
                return _instance;
            }
        }
    }
    protected Callback<LobbyCreated_t> _callbackLobbyCreated;
    protected Callback<LobbyEnter_t> _callbackLobbyEnter;

    void Awake()
    {
        DontDestroyOnLoad(this);
    }
    //ロビー作成コールバック
    public static void RegisterCallback_LobbyCreated(Callback<LobbyCreated_t>.DispatchDelegate onCreateLobby)
    {
        Instance._callbackLobbyCreated?.Dispose();
        Instance._callbackLobbyCreated = Callback<LobbyCreated_t>.Create(onCreateLobby);
    }
    public static void UnRegisterCallback_LobbyCreated()
    {
        Instance._callbackLobbyCreated.Dispose();
    }
    //入室コールバック
    public static void RegisterCallback_LobbyEnter(Callback<LobbyEnter_t>.DispatchDelegate onEnterLobby)
    {
        Instance._callbackLobbyEnter?.Dispose();
        Instance._callbackLobbyEnter = Callback<LobbyEnter_t>.Create(onEnterLobby);
    }
    public static void UnRegisterCallback_LobbyEnter()
    {
        Instance._callbackLobbyEnter.Dispose();
    }
    public static void CreateLobby(int maxNum)
    {
        Debug.Log("ロビー作成");
        var result = SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, maxNum);
    }

    public static List<CSteamID> GetLobbyMember(CSteamID lobbyId)
    {
        var userSteamIds = new List<CSteamID>();
        var count = SteamMatchmaking.GetNumLobbyMembers(lobbyId);
        for (var i = 0; i < count; i++)
        {
            userSteamIds.Add(SteamMatchmaking.GetLobbyMemberByIndex(lobbyId,i));
        }
        return userSteamIds;
    }
#if false
    public static void GetLobbyData(string lobbyId)
    {
        SteamMatchmaking.GetLobbyDataCount()
        SteamMatchmaking.GetLobbyData()
    }
#endif
}
