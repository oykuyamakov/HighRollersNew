// using Events;
// using UnityEngine;
//
// namespace Based.ResourceManagement.EventImplementations
// {
//     public class EarnResourceEvent : Event<EarnResourceEvent>
//     {
//         public ResourceType Type;
//         public int Amount;
//         public Vector3 InitPos = Vector3.zero;
//         
//         public static EarnResourceEvent Get(ResourceType type, int amount)
//         {
//             var evt = GetPooledInternal();
//             evt.Amount = amount;
//             evt.Type = type;
//             return evt;
//         }
//     }
// }
