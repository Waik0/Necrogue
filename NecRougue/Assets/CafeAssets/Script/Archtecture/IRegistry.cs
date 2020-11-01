using System.Collections.Generic;

public interface IRegistry<T>
{
    IEnumerable<T> Entity { get; }
    void Add(T element);
    void Remove(T element);
}