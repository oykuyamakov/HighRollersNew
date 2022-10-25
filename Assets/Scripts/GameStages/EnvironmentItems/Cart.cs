using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using DG.Tweening.Plugins.Core.PathCore;
using UnityEngine;

public class Cart : MonoBehaviour
{
   [SerializeField] private List<Transform> m_Waypoints;

   private void Awake()
   {
      FollowPath();
   }

   private void FollowPath()
   {
      var points = m_Waypoints.Select(i => i.position).ToArray();
      Path path = new Path(PathType.Linear, points, 2);

      transform.DOPath(path, 15f, PathMode.Full3D).SetLoops(-1,LoopType.Restart);
   }
}
