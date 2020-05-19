using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class AbilityEditor : EditorWindow
{
    private class AbilityDisp
    {
        public MstAbilityRecord Record;

        
        public void View()
        {
           
            GUILayout.BeginHorizontal();
            try
            {
                Record.id = int.Parse(GUILayout.TextField(Record.id.ToString(),GUILayout.Width(30)));
            }
            catch (Exception e)
            {
                Record.id = 1;
            }
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            var changed = false;
            Record.name = GUILayout.TextField(Record.name,GUILayout.Width(100));
            try
            {
                var before = Record.param1;
                Record.param1 = int.Parse(GUILayout.TextField(Record.param1.ToString(), GUILayout.Width(30)));
                if (before != Record.param1)
                {
                    changed = true;
                }
            }
            catch (Exception e)
            {
                Record.param1 = 0;
                changed = true;
            }
            try
            {
                var before = Record.param2;
                Record.param2 = int.Parse(GUILayout.TextField(Record.param2.ToString(), GUILayout.Width(30)));
                if (before != Record.param1)
                {
                    changed = true;
                }
            }
            catch (Exception e)
            {
                Record.param2 = 0;
                changed = true;
            }
          
            changed |= ViewTimingType<AbilityTimingType>(ref Record.timingType);
            changed |= ViewTimingType<AbilityEffectConditionType>(ref Record.conditionType);
            changed |= ViewTimingType<AbilityEffectTargetType>(ref Record.targetType);
            changed |= ViewTimingType<AbilityEffectType>(ref Record.effectType);
            GUILayout.EndHorizontal();
           
            GUILayout.BeginHorizontal();
            if (changed)
            {
                //Debug.Log(changed);
                if (_aet.ContainsKey(Record.effectType))
                         Record.description = _aet[Record.effectType](Record);
            }
            // if (GUILayout.Button("更新",GUILayout.Width(30)))
            // {
            //     if (_aet.ContainsKey(Record.effectType))
            //         Record.description = _aet[Record.effectType](Record);
            // }
            GUILayout.Label(String.Format(Record.description,Record.param1));
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            
            
            //ViewTimingType<AbilityTimingType>(ref Record.timingType,tt);
            GUILayout.EndHorizontal();
            
        }

        bool ViewTimingType<T>(ref T val) where T : struct 
        {
            var valueList = new List<int>();
            foreach (int value in Enum.GetValues(val.GetType()))
            {
                valueList.Add(value);
            }

            var index = 0;
            if (valueList.Contains((int)(object)val))
            {
                index = valueList.IndexOf((int)(object)val);
                                   
            }

            string[] labelList = Enum.GetNames(val.GetType());//dic.Values.Select(_=>_(Record)).ToArray();
                var nval = (T)(object)valueList[
                EditorGUILayout.Popup(index,labelList,
                    GUILayout.Width(150))
                ];
                if ((int)(object)val != (int)(object)nval)
                {
                    val = nval;
                    return true;
                }

                return false;
        }
    }

    //private int _max;//要素数
    Dictionary<string, List<string>> Data = new Dictionary<string,  List<string>>();
    private List<AbilityDisp> _disps = new List<AbilityDisp>();
    [MenuItem("Necrougue/AbilityEditor")]
    static void Open()
    {
        var window = CreateInstance<AbilityEditor>();
        window.minSize = new Vector2(600,400);
        window.Show();
    }

    static string CT(MstAbilityRecord rec)
    {
        if (_aect.ContainsKey(rec.conditionType))
        {
            return _aect[rec.conditionType](rec);
        }

        return rec.conditionType.ToString();
    }
    static string AETT(MstAbilityRecord rec)
    {
        if (_aett.ContainsKey(rec.targetType))
        {
            return _aett[rec.targetType](rec);
        }

        return rec.targetType.ToString();
    }
    static string AECT(MstAbilityRecord rec)
    {
        if (_aect.ContainsKey(rec.conditionType))
        {
            return _aect[rec.conditionType](rec);
        }

        return rec.conditionType.ToString();
    }
    static string ATT(MstAbilityRecord rec)
    {
        if (_att.ContainsKey(rec.timingType))
        {
            return _att[rec.timingType](rec);
        }

        return rec.timingType.ToString();
    }
    public static Dictionary<AbilityEffectTargetType, Func<MstAbilityRecord,string>> _aett = new Dictionary<AbilityEffectTargetType, Func<MstAbilityRecord,string>>()
    {
        { AbilityEffectTargetType.None , a=>"無効"},
        { AbilityEffectTargetType.MySelf ,a=> "自分自身" },
        { AbilityEffectTargetType.MySide ,a=> "自分の両隣" },
        { AbilityEffectTargetType.MyLeft , a=>"自分の左隣" },
        { AbilityEffectTargetType.Action , a=>$"そのモンスター" },
        { AbilityEffectTargetType.DefenderSide , a=>"攻撃を受けたモンスターの両隣"},
        { AbilityEffectTargetType.AllyAll ,a=> "味方モンスターすべて" },
        { AbilityEffectTargetType.EnemyAll ,a=> "敵モンスターすべて" },
        { AbilityEffectTargetType.AllyLeftmost , a=>"味方の左端" },
        { AbilityEffectTargetType.EnemyLeftmost , a=>$"敵の左端" },
        { AbilityEffectTargetType.All , a=>$"すべて" },
        { AbilityEffectTargetType.Random , a=>$"いずれかのモンスター" },
        { AbilityEffectTargetType.RandomAlly , a=>$"味方のいずれかのモンスター" },
        { AbilityEffectTargetType.RandomEnemy , a=>$"敵のいずれかのモンスター" },
        { AbilityEffectTargetType.Defender , a=>$"攻撃を受けたモンスター" },
        //{ AbilityEffectTargetType.EnemyLeftmost , a=>$"敵の左端" },
    };
    public static Dictionary<AbilityEffectConditionType, Func<MstAbilityRecord,string>> _aect = new Dictionary<AbilityEffectConditionType, Func<MstAbilityRecord,string>>()
    {
        { AbilityEffectConditionType.None,a=>"無効" },
        { AbilityEffectConditionType.Self,a=>"このモンスター" },
        { AbilityEffectConditionType.Ally,a=>"味方モンスター" },
        { AbilityEffectConditionType.Enemy,a=>"敵モンスター" },
        { AbilityEffectConditionType.AllyRace,a=>$"種族:{a.raceId}のモンスター" },
        { AbilityEffectConditionType.Any,a=>"いずれかのモンスター" },
    };
    public static Dictionary<AbilityEffectType, Func<MstAbilityRecord,string>> _aet = new Dictionary<AbilityEffectType, Func<MstAbilityRecord,string>>()
    {
        { AbilityEffectType.None,a=>"無効" },
        { AbilityEffectType.AtUp,a=>$"{ATT(a)}、{AETT(a)}にAtk + {a.param1}" },
        { AbilityEffectType.Summon,a=>$"{ATT(a)}、{a.param1}を{a.param2}体召喚" },
        { AbilityEffectType.Shield,a=>$"{ATT(a)}、{AETT(a)}に次のダメージを無効するシールドをつける" },
        { AbilityEffectType.Blocker,a=>$"{ATT(a)}から、敵の攻撃が{AETT(a)}に集中するようになる" },
        { AbilityEffectType.Revive,a=>$"{ATT(a)}、{AETT(a)}が一度だけ復活するようになる" },
        { AbilityEffectType.Stun,a=>$"{ATT(a)}、{AETT(a)}をスタンさせる" },
        { AbilityEffectType.ExtAttack,a=>$"{ATT(a)}、{AETT(a)}に{a.param1}ダメージを与える" },
        { AbilityEffectType.AddAbility,a=>$"{ATT(a)}、{AETT(a)}に能力{a.param1}を与える" },
        { AbilityEffectType.SummonRandom,a=>$"{ATT(a)}、モンスターをランダムに{a.param1}体召喚" },
        { AbilityEffectType.HpUp,a=>$"{ATT(a)}、{AETT(a)}にHp + {a.param1}" },
        { AbilityEffectType.AtHpUp,a=>$"{ATT(a)}、{AETT(a)}のHp + {a.param1},At + {a.param2}" },
        { AbilityEffectType.Union,a=>$"{ATT(a)}、{AETT(a)}が存在すれば、そのモンスターに自分のHp、Atk、能力を上乗せし、自分は消滅する" },
    };
    public static Dictionary<AbilityTimingType, Func<MstAbilityRecord,string>> _att = new Dictionary<AbilityTimingType, Func<MstAbilityRecord,string>>()
    {
        { AbilityTimingType.None,a=>"無効" },
        { AbilityTimingType.Summon,a=>$"{AECT(a)}を召喚した時" },
        { AbilityTimingType.BattleStart,a=>"バトル開始した時" },
        { AbilityTimingType.Attack,a=>$"{AECT(a)}が攻撃したあと" },
        { AbilityTimingType.Defence,a=>$"{AECT(a)}が攻撃されたあと" },
        //{ AbilityTimingType.ConfirmAttacker,a=>$"{AECT(a)}が攻撃する直前" },
        { AbilityTimingType.ConfirmTargetAttack,a=>$"{AECT(a)}が攻撃する直前" },
        { AbilityTimingType.ConfirmTargetDefence,a=>$"{AECT(a)}が攻撃される直前" },
        { AbilityTimingType.Dead,a=>$"{AECT(a)}が倒された時" },
        { AbilityTimingType.TurnEnd,a=>$"ターン終了時" },
        { AbilityTimingType.TurnStart,a=>"ターン開始時" },
    };
    private void Load()
    {
        FieldInfo[] fields = typeof(MstAbilityRecord).GetFields();

        Attribute[] attributes = Attribute.GetCustomAttributes(typeof(MstAbilityRecord), typeof(MasterPath));
        if (attributes == null || attributes.Length == 0)
        {
            throw new InvalidOperationException("The provided object is not serializable");

        }
        var path = "Assets/Resources/" + (attributes[0] as MasterPath).Path;
        var d = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
        Debug.Log(path);
        var json = (IList)MiniJSON.Json.Deserialize(d.text);
        var dicList = new List<IDictionary>();
        foreach (var o in json)
        {
            var dic = (IDictionary)o;
            var ret = new Dictionary<string, object>();
            dicList.Add(dic);

        }
        Data.Clear();
        foreach (var fieldInfo in fields)
        {
            if (!Data.ContainsKey(fieldInfo.Name))
            {
                Data.Add(fieldInfo.Name, new List<string>());
            }

        }
        foreach (var dictionary in dicList.OrderBy(_ =>
        {
            foreach (DictionaryEntry elem in _)
            {
                if (elem.Key.ToString() == "id")
                {
                    try
                    {
                        return int.Parse(elem.Value.ToString());
                    }
                    catch
                    {
                        return int.MaxValue;
                    }
                }
            }

            return int.MaxValue;
        }))
        {
            foreach (DictionaryEntry o1 in dictionary)
            {

                if (Data.ContainsKey(o1.Key.ToString()))
                {
                    Data[o1.Key.ToString()].Add(o1.Value.ToString());
                }
            }
        }
        var datalength = 0;
        foreach (var keyValuePair in Data)
        {
            datalength = Math.Max(keyValuePair.Value.Count, datalength);
        }

        //_max = datalength;
        foreach (var keyValuePair in Data)
        {
            var num = datalength - keyValuePair.Value.Count;
            for (int i = 0; i < num; i++)
            {
                keyValuePair.Value.Add("0");
            }
        }

        ConvertView();
    }

    void UpdateDisp()
    {
        _disps = _disps.OrderBy(_ => _.Record.id).ToList();

    }
    private void Save()
    {
        if (_disps.Count == 0)
        {
            Debug.Log("SAVE FAILED : NO DATA");
        }
        ConvertData();
        Debug.Log("Save");
        var max = 0;
        foreach (var l in Data)
        {
            max = Math.Max(l.Value.Count, max);
        }
        var list = new List<Dictionary<string, string>>();
            
        for (int i = 0;i < max; i++)
        {
            var dic = new Dictionary<string, string>();
            foreach (var d in Data)
            {
                dic.Add(d.Key, d.Value[i]);

            }
            list.Add(dic);
        }
        var t = typeof(MstAbilityRecord);

        Attribute[] attributes = Attribute.GetCustomAttributes(t, typeof(MasterPath));
        if (attributes == null || attributes.Length == 0)
        {
            throw new InvalidOperationException("The provided object is not serializable");

        }

        var path = "Assets/Resources/" + (attributes[0] as MasterPath).Path;
        var json = MiniJSON.Json.Serialize(list);
        Debug.Log("Write Data : " + path);
        File.WriteAllText(path,json);
        AssetDatabase.Refresh();
    }
    private void ConvertView()
    {
        var max = 0;
        foreach (var l in Data)
        {
            max = Math.Max(l.Value.Count, max);
        }
        
        _disps = new List<AbilityDisp>();
        _disps.Clear();
       
        for (int i = 0; i < max; i++)
        {
            var d = new AbilityDisp();
            d.Record=new MstAbilityRecord();
            foreach (var keyValuePair in Data)
            {
                
                switch (keyValuePair.Key)
                {
                    case "id":
                        d.Record.id = int.Parse(keyValuePair.Value[i]);
                        break;
                    case "name":
                        d.Record.name = keyValuePair.Value[i].ToString();
                        break;
                    case "timingType":
                        d.Record.timingType = (AbilityTimingType) int.Parse(keyValuePair.Value[i]);
                        break;
                    case "conditionType":
                        d.Record.conditionType = (AbilityEffectConditionType) int.Parse(keyValuePair.Value[i]);
                        break;
                    case "targetType":
                        d.Record.targetType = (AbilityEffectTargetType) int.Parse(keyValuePair.Value[i]);
                        break;
                    case "effectType":
                        d.Record.effectType = (AbilityEffectType) int.Parse(keyValuePair.Value[i]);
                        break;
                    case "param1":
                        d.Record.param1 = int.Parse(keyValuePair.Value[i]);
                        break;
                    case "description":
                        d.Record.description = keyValuePair.Value[i];
                        break;
                    default:
                        break;
                }
            }


            _disps.Add(d);
        }
    }

    private void ConvertData()
    {
        Data.Clear();   
        foreach (var abilityDisp in _disps.OrderBy(_=>_.Record.id))
        {
            var fields = abilityDisp.Record.GetType().GetFields();
            var index = -1;
            if (Data.ContainsKey("id"))
            {
                //重複チェック
                for (var i = 0; i < Data["id"].Count; i++)
                {
                    if (int.Parse(Data["id"][i]) == abilityDisp.Record.id)
                    {
                        index = i;

                    }
                }
                
            }
            foreach (var fieldInfo in fields)
            {
                if (!Data.ContainsKey(fieldInfo.Name))
                {
                    Data.Add(fieldInfo.Name,new List<string>());
                }

                var val = fieldInfo.GetValue(abilityDisp.Record);
                string aval = "";
                switch (fieldInfo.FieldType.ToString())
                {
                    case "AbilityTimingType":
                    case "AbilityEffectConditionType":
                    case "AbilityEffectTargetType":
                    case "AbilityEffectType":
                        aval = ((int) val).ToString();
                        break;
                    default:
                        aval = val.ToString();
                        break;
                }
                if (index == -1)
                {
                    Debug.Log(fieldInfo.FieldType.BaseType);
                    Data[fieldInfo.Name].Add(aval);
                }
                else
                {
                    if (Data.ContainsKey(fieldInfo.Name))
                    {
                        Data[fieldInfo.Name][index] = aval;
                    }
                }
               
            }
        }
        
    }

    private void Add()
    {
        var id = 0;
        if (_disps.Count > 0)
        {
            id = _disps[_disps.Count - 1].Record.id + 1;
        }

        _disps.Add(new AbilityDisp()
        {
            Record = new MstAbilityRecord()
            {
                id = id,
                description = "",
                name = "",
            }
        });
    }

    private Vector2 scr;
    private void OnGUI()
    {
        
        GUILayout.BeginVertical();
        if (GUILayout.Button("LOAD"))
        {
            Load();
        }
        if (GUILayout.Button("SAVE"))
        {
            Save();
        }

        if (GUILayout.Button("Update"))
        {
            UpdateDisp();
        }

        scr = GUILayout.BeginScrollView(scr);
        foreach (var abilityDisp in _disps)
        {
            abilityDisp.View();
        }
        GUILayout.EndScrollView();
        if (GUILayout.Button("+"))
        {
            UpdateDisp();
            Add();
        }
        GUILayout.EndVertical();
    }
}
