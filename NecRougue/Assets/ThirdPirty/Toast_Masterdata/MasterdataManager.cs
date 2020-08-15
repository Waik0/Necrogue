using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Toast;
using System.Text.RegularExpressions;
//stateless
//Exception : 初期化してください =>InitMasterdataAsync
public class MasterdataManager : SingletonMonoBehaviour<MasterdataManager>
{
    private bool _isInit = false;
    //todo キャッシュ機構
    //現状たぶんGCされるのでは？
    public static T Get<T>(int id) where T : class, IMasterRecord
    {
        return MasterTable<T>.Instance.Get(id);
        /*
        if (typeof(T) == typeof(MstBulletRecord))
        {
            return (T)(object)MstBulletRecordTable.First(_ => _.id == id);
        }
        return null;*/
    }

    public static T[] Records<T>() where T : class, IMasterRecord
    {
        return MasterTable<T>.Instance.Records.Records;
    }
    /*
    public static MstBulletRecord[] MstBulletRecordTable = {
        new MstBulletRecord(){id = 10,gravy = -20f,gravx = 0.95f,rad = 10,weight = 1f}
    };*/
    /*
    protected override void Awake()
    {
        base.Awake();
        InitMasterdata(FindMasterdata());
    }
    */

    //initialize
    protected override void Awake()
    {

        base.Awake();
        InitMasterdata();//Todo:データがでかくなってきたら非同期で別スクリプトから読むようにする。
    }
    void InitRecord<T>() where T : class, IMasterRecord
    {
        Attribute[] attributes = Attribute.GetCustomAttributes(typeof(T), typeof(MasterPath));
        //object[] attributes = typeof(T).GetCustomAttributes(typeof(MasterPath),false);
        if (attributes == null || attributes.Length == 0)
        {
            throw new InvalidOperationException("The provided object is not serializable");

        }
        MasterPath path = attributes[0] as MasterPath;
        if (path == null)
        {
            return;
        }
        Debug.Log("Load Master : " + typeof(T));
        MasterTable<T>.Instance.Init(path.Path);
        Debug.Log(MasterTable<T>.Instance.Records.Records.Length);
    }
    static Type[] GetInterfaces<T>()
    {
        return Assembly.GetExecutingAssembly().GetTypes().Where(c => c.GetInterfaces().Any(t => t == typeof(T))).ToArray();
    }
    Type[] FindMasterdata()
    {
        var masterDataList = GetInterfaces<IMasterRecord>();
        Debug.Log(masterDataList.Length);
        return masterDataList;
    }
    public IEnumerator InitMasterdataAsync()
    {
        if (_isInit)
        {
            yield break;
        }
        _isInit = true;
        var masterDataList = FindMasterdata();
        Debug.Log("Listup Masterdata Class.");
        yield return null;
        var method = typeof(MasterdataManager).GetMethod("InitRecord", BindingFlags.Public |
                                                          BindingFlags.NonPublic |
                                                          BindingFlags.Instance |
                                                          BindingFlags.Static |
                                                          BindingFlags.DeclaredOnly);
        if (method == null)
        {
            Debug.Log("null");
            yield break;

        }
        yield return null;
        foreach (var type in masterDataList)
        {
            var generic = method.MakeGenericMethod(type);
            generic.Invoke(this, null);
            yield return null;
            // InitRecord<>();
        }
        Debug.Log("Initialized.");
    }

    public void InitMasterdata()
    {
        if (_isInit)
        {
            return;
        }
        _isInit = true;
        var masterDataList = FindMasterdata();
        Debug.Log("Listup Masterdata Class.");
        var method = typeof(MasterdataManager).GetMethod("InitRecord", BindingFlags.Public |
                                                                       BindingFlags.NonPublic |
                                                                       BindingFlags.Instance |
                                                                       BindingFlags.Static |
                                                                       BindingFlags.DeclaredOnly);
        if (method == null)
        {
            Debug.Log("null");

        }
        foreach (var type in masterDataList)
        {
            var generic = method.MakeGenericMethod(type);
            generic.Invoke(this, null);
        }
        Debug.Log("Initialized.");

    }
}
public class MasterTable<T> where T : class, IMasterRecord
{
    public Master<T> Records { get; private set; } //静的に確保
    public static MasterTable<T> _instance;
    public static MasterTable<T> Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new MasterTable<T>();
            }

            return _instance;
        }
    }

    public T Get(int id)
    {
        return Records.Records.FirstOrDefault(_ => _.Id == id);
    }
    public void Init(string path)
    {
        //StreamingAssets使用の場合
#if false
        string assetPath = Application.streamingAssetsPath + path;
        Debug.Log("[Master] load path : " + assetPath);
        string text = System.IO.File.ReadAllText(assetPath);
        Records = JsonUtility.FromJson<Master<T>>(text);
        if (Records == null)
        {
            Records = new Master<T>() { Records = new T[0] };
        }
#endif
#if true
        var ext = Regex.Match(path, "[^.]+$").Value;
        path = Regex.Replace(path, "(.*)Resources/", "");
        path = Regex.Replace(path, "." + ext + "$", "");
        path = Regex.Replace(path, "^/", "");
        Debug.Log("[Master] load from resources : " + path);

        string text = Resources.Load<TextAsset>(path).ToString();
        //var data = JsonUtility.FromJson<MasterRoot<T>>( text );
        Records = JsonUtility.FromJson<Master<T>>(text);
        //Records = JsonUtility.FromJson<Master<T>>("{\"Records\":" + text + "}");
        if (Records == null)
        {
            Records = new Master<T>() { Records = new T[0] };
        }
#endif
        //Debug.Log(Records.Records);
    }
}

public interface IMasterRecord
{
    int Id { get; }
}

[Serializable]
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class MasterPath : Attribute
{
    public string Path;

    public MasterPath(string path)
    {
        Path = path;
    }
}

// [Serializable]
// public class MasterMeta
// {
//     public MasterColumType[] Colums;
// }
//
// [SerializeField]
// public class MasterColumType
// {
//     public string Name;
//     public string Type;
// }
// [Serializable]
// public class MasterRoot<T> where T : class, IMasterRecord
// {
//     public MasterMeta Meta;
//     public T[] Records;
// }
[Serializable]
public class Master<T> where T : class, IMasterRecord
{
    public T[] Records;
}
[Serializable]
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class MasterDescription : Attribute
{
    public string Msg;

    public MasterDescription(string path)
    {
        Msg = path;
    }
}

