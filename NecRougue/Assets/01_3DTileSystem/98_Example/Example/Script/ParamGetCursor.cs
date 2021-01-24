using System;
using System.Collections;
using System.Collections.Generic;
using TileParamSystem.Example.Script.PraramProvider;
using UnityEngine;

public class ParamGetCursor : MonoBehaviour
{

    public class ParamSet
    {
        public string Name;
        public string Param;
        public override string ToString()
        {
            return $"{Name} : {Param}";
        }
    }

    public List<ParamSet> Params { get; } = new List<ParamSet>();
    private bool _updateParams = false;
    public void SetPosition(Vector3 pos)
    {
        gameObject.transform.position = pos;
    }

    private void FixedUpdate()
    {
        if (_updateParams == false)
        {
            Params.Clear();
        }
        _updateParams = false;
    }

    private void OnTriggerStay(Collider c)
    {
        if (_updateParams == false)
        {
            Params.Clear();
        }
        _updateParams = true;
        var p = c.GetComponent<ParamExample>();
        if (p == null)
        {
            return;
        }
        if (p.CanGetParam(gameObject.transform.position))
        {
            Params.Add(new ParamSet()
            {
                Name = "ExampleParam",
                Param = p.Param.ToString()
            });
        }
    }
}
