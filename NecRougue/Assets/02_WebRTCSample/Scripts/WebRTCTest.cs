using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;
using UniRx;

public class ReceiveData<T> where T : struct
{
    public string FromId;
    public T Command;
    public string Data;
    public override string ToString()
    {
        return $"{FromId} , {Command} , {Data}";
    }
}
public class WebRTCTest : MonoBehaviour
{
    public enum NetworkCommand
    {
        Chat,
        UserName,
        SelectButton,
    }
    
    void Start()
    {
        _subject.Subscribe(r =>
        {
            Debug.Log(r.ToString());
        }).AddTo(this);
        Receive("/EAVE/2/2/WEVAWEF/REGERGA");
        Receive("wewew/EAVE/1/VERFGE/WEVAWEF/REGERGA");
        
    }
    //自分のID
    private string _selfId;
    //アプリバージョン(端末で異なる可能性があるのでここで決め打ち)
    private string _identifer = "doka";
    private string _appVersion = "0.1";
    //接続中のユーザーID 自分も含まれる
    public List<string> _connectionList;
    public IObservable<ReceiveData<NetworkCommand>> OnReceive => _subject;
    private Subject<ReceiveData<NetworkCommand>> _subject = new Subject<ReceiveData<NetworkCommand>>();
    
    //基本ブロードキャスト
    public void Send(NetworkCommand command, string data,string toUser = "")
    {
        // identifer/version/from/to/command/dataの順
        string sendData = $"{_identifer}/{_appVersion}/{_selfId}/{toUser}/{(int) command}/{data}";
    }
    //データ取得
    public void Receive(string data)
    {
        var r = new Regex(@"(.*?)/(.*?)/(.*?)/(.*?)/(.*?)/(.*)");
        var matches = r.Matches(data);
   
        List<string> result = new List<string>();
        foreach (Match match in matches)
        {
            var ignoreFirst = true;
            foreach (Group matchGroup in match.Groups)
            {
                if (ignoreFirst)
                {
                    ignoreFirst = false;
                    continue;
                }
                Debug.Log(matchGroup.Value);
                result.Add(matchGroup.Value);
            }
            Debug.Log("NEXT");
        }
        //整合性チェック
        if (result.Count < 6)
        {
             Debug.Log("Invalid Data");
             return;
        }
        //対応アプリケーションチェック
        if (result[0] != _identifer)
        {
            //違うアプリケーションの通信が混ざっている
            Debug.Log("Invalid Identifer");
            return;
        }
        //バージョンチェック
        if (result[1] != _appVersion)
        {
            //バージョンが異なる
            Debug.Log("Invalid Version");
            return;
        }
        //コマンドをパース
        var command = 0;
        try
        {
            command = int.Parse(result[2]);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        //通知
        _subject.OnNext(new ReceiveData<NetworkCommand>()
        {
            FromId = result[0],
            Command = (NetworkCommand) command,
            Data = result[3],
        });

    }

    private void OnDestroy()
    {
        _subject.Dispose();
    }
}
