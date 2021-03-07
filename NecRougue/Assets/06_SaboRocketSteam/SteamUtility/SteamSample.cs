using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
public class SteamSample : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (SteamManager.Initialized)
        {
            string name = SteamFriends.GetPersonaName();
            
            var friendCount = SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagImmediate);
            Debug.Log(name);
            Debug.Log(friendCount);
        }

        //var matching = new SteamMatchingBase();
        SteamMatchingManager.CreateLobby(7);
        SteamMatchingManager.RegisterCallback_LobbyCreated(   res =>
            {
                Debug.Log("CreateLobby");
                Debug.Log(res.m_eResult.ToString());
                Debug.Log(res.m_ulSteamIDLobby);
            });
        SteamMatchingManager.RegisterCallback_LobbyEnter(res =>
        {
            Debug.Log("EnterLobby");
            Debug.Log(res.m_ulSteamIDLobby);
            var members = SteamMatchingManager.GetLobbyMember(new CSteamID(res.m_ulSteamIDLobby));
            foreach (var cSteamID in members)
            {
                Debug.Log(cSteamID.ToString());
            }
        });
        StartCoroutine(Matching());
    }

    IEnumerator Matching()
    {
        while (true)
        {
            //SteamMatchmaking.GetLobbyMemberData
            yield return new WaitForSeconds(2);
        }
        yield return null;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
