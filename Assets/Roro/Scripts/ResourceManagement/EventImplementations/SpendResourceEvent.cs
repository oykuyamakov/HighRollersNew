// using Events;
// using UnityEngine;
//
// namespace Based.ResourceManagement.EventImplementations
// {
//     public class SpendResourceEvent : Event<SpendResourceEvent>
//     {
//         public ResourceType ResourceType;
//         public int Amount;
//         public Vector3 InitPos = Vector3.zero;
//         
//         public static SpendResourceEvent Get(ResourceType type, int amount)
//         {
//             var evt = GetPooledInternal();
//             evt.Amount = amount;
//             evt.ResourceType = type;
//             return evt;
//         }
//     }
// }
