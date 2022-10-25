using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
#if UNITY_EDITOR

using UnityCommon.Editor.Utility;
#endif

using UnityEngine;
#if UNITY_EDITOR
#endif

namespace Based.Utility
{
 //     public class GeneralSettings<T> : ScriptableObject where T : class
 //     {
 //         private static GeneralSettings<T> _settings;
 //
 //         private static T settings
 //         {
 //             get
 //             {
 //                 if (!_settings)
 //                 {
 //                     _settings = Resources.Load<GeneralSettings<T>>($"Settings/");
 //
 //                     if (!_settings)
 //                     {
 // #if UNITY_EDITOR
 //                         Debug.Log("Creating General Settings");
 //                         _settings = CreateInstance<GeneralSettings<T>>();
 //                         var path = "Assets/Resources/Settings/Setting12.asset";
 //                         AssetDatabaseHelpers.CreateAssetMkdir(_settings, path);
 // #else
 // 				//		throw new Exception("Global settings could not be loaded");
 // #endif
 //                     }
 //                 }
 //
 //                 return _settings as T;
 //             }
 //         }
 //
 //         public static T Get()
 //         {
 //             return settings;
 //         }
 //     }

}