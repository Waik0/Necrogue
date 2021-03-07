﻿using UnityEngine;
using UnityEngine.UI;
using Steamworks;
using System.Collections;
using System.Collections.Generic;

public class SteamCore : MonoBehaviour {

    protected Callback<LobbyCreated_t> Callback_lobbyCreated;
    protected Callback<LobbyMatchList_t> Callback_lobbyList;
    protected Callback<LobbyEnter_t> Callback_lobbyEnter;
    protected Callback<LobbyDataUpdate_t> Callback_lobbyInfo;

    ulong current_lobbyID;
    List<CSteamID> lobbyIDS;

    // Use this for initialization
    void Start () {

        lobbyIDS = new List<CSteamID>();
        Callback_lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        Callback_lobbyList = Callback<LobbyMatchList_t>.Create(OnGetLobbiesList);
        Callback_lobbyEnter = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
        Callback_lobbyInfo = Callback<LobbyDataUpdate_t>.Create(OnGetLobbyInfo);

        // if (SteamAPI.Init())
        //     Debug.Log("Steam API init -- SUCCESS!");
        // else
        //     Debug.Log("Steam API init -- failure ...");
    }

    void Update()
    {
        //SteamAPI.RunCallbacks();

        // Command - Create new lobby
        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("Trying to create lobby ...");
            SteamAPICall_t try_toHost = SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, 8);
        }

        // Command - List lobbies
        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("Trying to get list of available lobbies ...");
            SteamAPICall_t try_getList = SteamMatchmaking.RequestLobbyList();
        }

        // Command - Join lobby at index 0 (testing purposes)
        if (Input.GetKeyDown(KeyCode.J))
        {
            Debug.Log("Trying to join FIRST listed lobby ...");
            SteamAPICall_t try_joinLobby = SteamMatchmaking.JoinLobby(SteamMatchmaking.GetLobbyByIndex(0));
        }

        // Command - List lobby members
        if (Input.GetKeyDown(KeyCode.Q))
        {
            int numPlayers = SteamMatchmaking.GetNumLobbyMembers((CSteamID)current_lobbyID);

            Debug.Log("\t Number of players currently in lobby : " + numPlayers);
            for (int i = 0; i < numPlayers; i++)
            {
                Debug.Log("\t Player(" + i + ") == " + SteamFriends.GetFriendPersonaName(SteamMatchmaking.GetLobbyMemberByIndex((CSteamID)current_lobbyID, i)));
            }
        }
    }

    void OnLobbyCreated(LobbyCreated_t result)
    {
        if (result.m_eResult == EResult.k_EResultOK)
            Debug.Log("Lobby created -- SUCCESS!");
        else
            Debug.Log("Lobby created -- failure ...");

        string personalName = SteamFriends.GetPersonaName();
        SteamMatchmaking.SetLobbyData((CSteamID)result.m_ulSteamIDLobby, "name", personalName+"'s game");
    }

    void OnGetLobbiesList(LobbyMatchList_t result)
    {
        Debug.Log("Found " + result.m_nLobbiesMatching + " lobbies!");
        for(int i=0; i< result.m_nLobbiesMatching; i++)
        {
            CSteamID lobbyID = SteamMatchmaking.GetLobbyByIndex(i);
            lobbyIDS.Add(lobbyID);
            SteamMatchmaking.RequestLobbyData(lobbyID);
        }
    }

    void OnGetLobbyInfo(LobbyDataUpdate_t result)
    {
        for(int i=0; i<lobbyIDS.Count; i++)
        {
            if (lobbyIDS[i].m_SteamID == result.m_ulSteamIDLobby)
            {
                Debug.Log("Lobby " + i+" :: " +SteamMatchmaking.GetLobbyData((CSteamID)lobbyIDS[i].m_SteamID, "name"));
                return;
            }
        }
       
    }

    void OnLobbyEntered(LobbyEnter_t result)
    {
        current_lobbyID = result.m_ulSteamIDLobby;

        if (result.m_EChatRoomEnterResponse == 1)
            Debug.Log("Lobby joined!");
        else
            Debug.Log("Failed to join lobby.");
    }

}