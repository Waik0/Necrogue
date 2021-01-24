using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TileParamSystem.Source.Core.Script
{
    public interface ITileParamCollection<out T>
    {
        T GetParam();
    }
    //todo ここから下いらないかも
    public interface ITileParamRepository
    {
        
        //IEnumerable<ITileParamCollection> Entity { get; }
        void Reset();
        void Tick();
        string Log();
    }
    public interface ITileParamRegistry
    {
        IEnumerable<ITileParamRepository> Entity { get; }
        T Find<T>() where T :  class, ITileParamRepository;
        void Reset();
        void Tick();
    }
    public class TileParamRegistry : ITileParamRegistry
    {
        readonly List<ITileParamRepository> _entities = new List<ITileParamRepository>();

        public IEnumerable<ITileParamRepository> Entity => _entities;

        public TileParamRegistry(
            List<ITileParamRepository> collections)
        {
            _entities = collections;
        }
        public T Find<T>()  where T : class, ITileParamRepository
        {
            return (T)_entities.FirstOrDefault(ins => ins is T c);
        }
        public void Tick()
        {
            foreach (var paramCollection in _entities)
            {
                paramCollection.Tick();
            }
        }

        public void Reset()
        {
            foreach (var paramCollection in _entities)
            {
                paramCollection.Reset();
            }
        }
    }
}