using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CafeAssets.Script.Interface.Layer_01.Manager;
using CafeAssets.Script.Interface.Layer_02.UseCase;
using CafeAssets.Script.Model;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapParamUseCase : ITilemapParamUseCase,IPlaceTileReceiver
{
    private ITilemapParameterRepository _tilemapPropsRepository;
    private ITilemapUseCase _tilemapUseCase;
    private ICreateParamData[] _createParamDatas;
    public TilemapParamUseCase(
        ITilemapParameterRepository tilemapPropsRepository,
        ITilemapUseCase tilemapUseCase
    )
    {
        _tilemapPropsRepository = tilemapPropsRepository;
        _tilemapUseCase = tilemapUseCase;
        _createParamDatas = new ICreateParamData[]
        {
            new CreateParamDataSingle(tilemapUseCase),
            new CreateParamDataDraw(tilemapUseCase), 
            new CreateParamDataRect(tilemapUseCase)
        };
    }

    public Vector2Int[] FindCanSitDownPlace()
    {
        return (from tilemapPropsModel in _tilemapPropsRepository.Entity where tilemapPropsModel.Value.CanSitDown && !tilemapPropsModel.Value.IsSitDown select tilemapPropsModel.Key).ToArray();
    }

    public Vector2Int? RegisterAndGetSitDownPlace()
    {
        var list = FindCanSitDownPlace();
        if (list.Length == 0)
        {
            return null;
        }
        var model = _tilemapPropsRepository.Get(list[0]);
        model.IsSitDown = true;
        return list[0];
    }

    public void OnPlaceTile(TilePlaceModel model)
    {
        Debug.Log("AddTileParam");
        foreach (var createParamData in _createParamDatas)
        {
            createParamData.Add(model,_tilemapPropsRepository);
        }
    }

    public void OnRemoveTile(TilePlaceModel model)
    {
        throw new System.NotImplementedException();
    }
}

interface ICreateParamData
{
    //passableに追加する
    void Add(TilePlaceModel model,ITilemapParameterRepository repository);
    void Remove(TilePlaceModel model, ITilemapParameterRepository repository);
}

class CreateParamDataBase 
{
    protected void AddToRepository(ITilemapParameterRepository repository,Vector2Int pos, EffectiveTileModel effective)
    {
        repository.Add(pos,new TilemapPropsModel()
        {
            CanSitDown = effective != null && effective.EffectType == TileEffectType.Sit,
            IsSitDown = false,
        });
    }
}
class CreateParamDataSingle : CreateParamDataBase,ICreateParamData
{
    private ITilemapUseCase _tilemapUseCase;
    public CreateParamDataSingle(
        ITilemapUseCase tilemapUseCase
    )
    {
        _tilemapUseCase = tilemapUseCase;
    }
    public void Add(TilePlaceModel model, ITilemapParameterRepository repository)
    {
        if (model.PlaceMode != PlaceTileMode.PlaceTileSingle)
            return;
        var effective = model.Model as EffectiveTileModel;
        if (model.Model.Brush.sqrMagnitude > 1)
        {
            Debug.Log(model.Model);
            var origin = _tilemapUseCase.WorldToCell(model.StartWorldPos);
            int xhalf = (model.Model.Brush.x + 1 - (model.Model.Brush.x % 2)) / 2;
            int yhalf = (model.Model.Brush.y + 1 - (model.Model.Brush.y % 2)) / 2;
          
            for (var i = 0; i < model.Model.Brush.x; i++)
            {
                for (var j = 0; j < model.Model.Brush.y; j++)
                {
                    var pos = new Vector2Int(origin.x - xhalf + i,origin.y - yhalf + j);
                    AddToRepository(repository,pos,effective);
                }
            }
               
        }
        else
        {
            var pos = (Vector2Int)_tilemapUseCase.WorldToCell(model.StartWorldPos);
            AddToRepository(repository,pos,effective);
        }
        //repository.Add();
        
    }

    public void Remove(TilePlaceModel model, ITilemapParameterRepository repository)
    {
        throw new System.NotImplementedException();
    }
}
class CreateParamDataDraw : CreateParamDataBase,ICreateParamData
{
    private ITilemapUseCase _tilemapUseCase;
    public CreateParamDataDraw(
        ITilemapUseCase tilemapUseCase
    )
    {
        _tilemapUseCase = tilemapUseCase;
    }
    public void Add(TilePlaceModel model, ITilemapParameterRepository repository)
    {
        if (model.PlaceMode != PlaceTileMode.PlaceTileDraw)
            return;
        var effective = model.Model as EffectiveTileModel;
        if (model.Model.Brush.sqrMagnitude > 1)
        {
            Debug.Log(model.Model);
            var positions = new Vector3Int[model.Model.Brush.x * model.Model.Brush.y];
            var origin = _tilemapUseCase.WorldToCell(model.StartWorldPos);
            int xhalf = (model.Model.Brush.x + 1 - (model.Model.Brush.x % 2)) / 2;
            int yhalf = (model.Model.Brush.y + 1 - (model.Model.Brush.y % 2)) / 2;
          
            for (var i = 0; i < model.Model.Brush.x; i++)
            {
                for (var j = 0; j < model.Model.Brush.y; j++)
                {
                    var pos = new Vector2Int(origin.x - xhalf + i,origin.y - yhalf + j);
                    AddToRepository(repository,pos,effective);
                }
            }
               
        }
        else
        {
            var pos = (Vector2Int)_tilemapUseCase.WorldToCell(model.StartWorldPos);
            AddToRepository(repository,pos,effective);
        }
        //repository.Add();
        
    }

    public void Remove(TilePlaceModel model, ITilemapParameterRepository repository)
    {
        throw new System.NotImplementedException();
    }
} 
class CreateParamDataRect : CreateParamDataBase,ICreateParamData
{
    private ITilemapUseCase _tilemapUseCase;
    public CreateParamDataRect(
        ITilemapUseCase tilemapUseCase
    )
    {
        _tilemapUseCase = tilemapUseCase;
    }
    public void Add(TilePlaceModel model, ITilemapParameterRepository repository)
    {
        if (model.PlaceMode != PlaceTileMode.PlaceTileRect)
            return;
        var start = _tilemapUseCase.WorldToCell(model.StartWorldPos);
        var end = _tilemapUseCase.WorldToCell(model.EndWorldPos);
        var sx = Mathf.Min(start.x, end.x);
        var ex = Mathf.Max(start.x, end.x);
        var sy = Mathf.Min(start.y, end.y);
        var ey = Mathf.Max(start.y, end.y);
        var effective = model.Model as EffectiveTileModel;
        for (var i = sx; i < ex; i++)
        {
            for (var j = sy; j < ey; j++)
            {
                var pos = new Vector2Int(i,j);
                AddToRepository(repository,pos,effective);
            }
        }
        
    }

    public void Remove(TilePlaceModel model, ITilemapParameterRepository repository)
    {
        throw new System.NotImplementedException();
    }
} 