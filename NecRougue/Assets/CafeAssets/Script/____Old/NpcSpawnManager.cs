

//
// public class NpcSpawnManager : INpcSpawnManager
// {
//
//     private List<INpcSpawnReceiver> _receivers;
//
//     public NpcSpawnManager(
//         [InjectOptional]
//         List<INpcSpawnReceiver> receivers
//     )
//     {
//
//         _receivers = receivers;
//     }
//
//     public void Add(INpcSpawnReceiver tickable)
//     {
//         _receivers.Add(tickable);
//     }
//
//     public void RemoveNull()
//     {
//         _receivers.RemoveAll(_ => _ == null);
//     }
//
//     public void OnSpawn(NpcModel model)
//     {
//         foreach (var receiver in _receivers)
//         {
//             receiver?.OnSpawnNpc(model);
//         }
//         
//     }
//
//     public void Dispose()
//     {
//         _receivers.Clear();
//     }
// }