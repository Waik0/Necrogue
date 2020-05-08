using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//depends on master data system
//----------------------------------------------------------------------------------------------------------------------
//Entity
//----------------------------------------------------------------------------------------------------------------------

public interface IConvertFromMasterRecord<T> where T : IMasterRecord
{
    void Convert(T record);
}

public interface IConvertFromMasterRecords<T> where T : IMasterRecord
{
    void Convert(T[] records);
}
public interface IConvertFromEntity<T> where T : IEntity
{
    void Convert(T entity);
}

public interface IGenerateFromEntity<T1, T2> 
    where T1 : IGenerateFromEntity<T1, T2>
    where T2 : IEntity
{
    T1 Generate(T2 entity);
}
public interface IGenerateFromMasterRecord<T1, T2> 
    where T1 : IGenerateFromMasterRecord<T1, T2>
    where T2 : IMasterRecord
{
    T1 Generate(T2 record);
}
public interface IGenerateFromMasterRecords<T1, T2> 
    where T1 : IGenerateFromMasterRecords<T1, T2>
    where T2 : IMasterRecord
{
    T1 Generate(T2[] records);
}
public interface IEntity
{
    
}

public interface IDeepCopyable
{
    
}

//----------------------------------------------------------------------------------------------------------------------
//UseCase
//----------------------------------------------------------------------------------------------------------------------
public interface IEntityUseCase<T> where T : IEntity
{
    T Data { get; }
    void ResetData();
}