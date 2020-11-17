using System.Collections.Generic;
/// <summary>
/// エンティティを管理
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IRegistry<T>
{
    IEnumerable<T> Entity { get; }
    void Add(T element);
    void Remove(T element);
}