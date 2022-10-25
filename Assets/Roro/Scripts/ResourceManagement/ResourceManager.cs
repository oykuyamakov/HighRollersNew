// using Based.ResourceManagement.EventImplementations;
// using Based.Utility;
// using Events;
// using UnityCommon.Variables;
// using UnityEngine;
//
// namespace Based.ResourceManagement
// {
//     public class ResourceManager : MonoBehaviour
//     {
//         [SerializeField]
//         private IntVariable m_GoldVar;
//
//         [SerializeField]
//         private Transform m_GoldBar;
//         
//         private void Awake()
//         {
//             GEM.AddListener<SpendResourceEvent>(OnSpendResourceEvent);
//             GEM.AddListener<EarnResourceEvent>(OnEarnResourceEvent);
//         }
//
//         private void OnDestroy()
//         {
//             GEM.RemoveListener<SpendResourceEvent>(OnSpendResourceEvent);
//             GEM.RemoveListener<EarnResourceEvent>(OnEarnResourceEvent);
//         }
//
//         private void OnSpendResourceEvent(SpendResourceEvent evt)
//         {
//             if (evt.ResourceType == ResourceType.Gold)
//             {
//                 if (evt.Amount > m_GoldVar.Value)
//                 {
//                     evt.result = EventResult.Negative;
//                     return;
//                 }
//
//                 m_GoldVar.Value -= evt.Amount;
//                 evt.result = EventResult.Positive;
//             }
//
//             if (evt.InitPos != Vector3.zero)
//             {
//                 //var initPos = m_Camera.WorldToScreenPoint(evt.InitPos);
//                 ItemAnimator.MoveFromToBar(evt.ResourceType, evt.InitPos, m_GoldBar.position, evt.Amount);
//             }
//             
//             Variable.SavePlayerPrefs();
//         }
//
//         private void OnEarnResourceEvent(EarnResourceEvent evt)
//         {
//             if (evt.Type == ResourceType.Gold)
//             {
//                 m_GoldVar.Value += evt.Amount;
//                 evt.result = EventResult.Positive;
//             }
//
//             if (evt.InitPos != Vector3.zero)
//             {
//                 //var initPos = m_Camera.WorldToScreenPoint(evt.InitPos);
//                 ItemAnimator.MoveFromToBar(evt.Type, evt.InitPos, m_GoldBar.position);
//             }
//         }
//     }
// }