using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;


public class RagdollController : MonoBehaviour
{

	[SerializeField] private PhysicMaterial physicMaterial;
	
	[SerializeField] private Animator animator;
	[SerializeField] private Rigidbody[] rigidbodies;
	[SerializeField] private Collider[] colliders;


#if UNITY_EDITOR

	private void OnValidate()
	{
		animator = this.GetComponent<Animator>();
		List<Rigidbody> rbs = new List<Rigidbody>();
		List<Collider> cls = new List<Collider>();

		// rigidbodies = this.GetComponentsInChildren<Rigidbody>();
		foreach (var rb in this.GetComponentsInChildren<Rigidbody>())
		{
			if (rb.gameObject != gameObject)
			{
				// rb.maxDepenetrationVelocity
				rbs.Add(rb);
			}
		}
		
		foreach (var collider in this.GetComponentsInChildren<Collider>())
		{
			if (collider.gameObject != gameObject)
			{
				if (collider.gameObject.layer != gameObject.layer)
				{
					continue;
				}
				
				cls.Add(collider);
				collider.material = physicMaterial;
			}
		}

		rigidbodies = rbs.ToArray();
		colliders = cls.ToArray();
	}

#endif

	private void Awake()
	{
		DisableRagdoll();

		foreach (var rb in rigidbodies)
		{
			rb.interpolation = RigidbodyInterpolation.Interpolate;
		}

	}

	public void StopForces()
	{
		foreach (var rb in rigidbodies)
		{
			rb.velocity = Vector3.zero;
			rb.detectCollisions = false;
		}

		// foreach (var coll in colliders)
		// {
		// 	coll.isTrigger = true;
		// }
	}

	public void TurnOffBounce()
	{
		physicMaterial.bounciness = 0;
	}
	
	public void SetConstraints()
	{
		// foreach (var rb in rigidbodies)
		// {
			ChangeGravity(false);
			rigidbodies[0].constraints = RigidbodyConstraints.FreezeRotation;
			//rigidbodies[11].constraints = RigidbodyConstraints.FreezeRotation;
			//rigidbodies[7].constraints = RigidbodyConstraints.FreezeRotation;
			//}
	}

	public void FreezeZPosition()
	{
		rigidbodies.ForEach(rb => rb.constraints = RigidbodyConstraints.FreezePositionZ);
	}
	
	/*
	public void TakeHit(BodyPart part, Vector3 force)
	{
		var hitBox = GetComponentsInChildren<HitBox>().FirstOrDefault(h => h.bodyPart == part);

		if (hitBox)
		{
			hitBox.GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);
		}

	}
	*/

	public void SetRagdoll(bool enabled)
	{
		if (enabled)
		{
			EnableRagdoll();
		}
		else
		{
			DisableRagdoll();
		}
	}

	[Button]
	public void EnableRagdoll()
	{
		animator.enabled = false;
		for (int i = 0; i < rigidbodies.Length; i++)
		{
			rigidbodies[i].isKinematic = false;
			rigidbodies[i].useGravity = true;
			rigidbodies[i].collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
			// rigidbodies[i].maxDepenetrationVelocity
		}
		
		foreach (var collider in colliders)
		{
			collider.isTrigger = false;
		}
	}

	[Button]
	public void DisableRagdoll()
	{
		for (int i = 0; i < rigidbodies.Length; i++)
		{
			rigidbodies[i].isKinematic = true;
			rigidbodies[i].useGravity = false;
		}
		animator.enabled = true;

		foreach (var collider in colliders)
		{
			collider.isTrigger = true;
		}
	}

	public void ChangeGravity(bool b)
	{
		for (int i = 0; i < rigidbodies.Length; i++)
		{
			rigidbodies[i].useGravity = b;
		}
	}

	public void ChangeLayers(int layer)
	{
		colliders.ForEach(coll =>
		{
			coll.gameObject.layer = layer;
		});
	}
	
}

