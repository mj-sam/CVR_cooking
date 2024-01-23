using UnityEngine;

namespace Manus.Interaction
{
	/// <summary>
	/// This class makes an object grabbable in the most basic of ways.
	/// It changes its position and rotation according to the hands grabbing it.
	/// </summary>
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Rigidbody))]
	[AddComponentMenu("Manus/Interaction/Grabbable Object")]
	public class GrabbableObject : MonoBehaviour, IGrabbable
	{
		private bool keepUpright = false;
		private GrabbedItem grabbedItem;

		public void Start(){
			grabbedItem = this.gameObject.GetComponent<GrabbedItem>();
		}

		/// <summary>
		/// Called when this starts getting grabbed.
		/// </summary>
		/// <param name="p_Object">Contains information about the grab</param>
		public void OnGrabbedStart(GrabbedObject p_Object)
		{
			this.gameObject.GetComponent<GrabbedItem>().DisableAllColliders();
			GameObject.Find("TaskProgression").GetComponent<TaskProgression>().SetHapticVibration(0.4f,0.4f);
			GameObject.Find("TaskProgression").GetComponent<TaskProgression>().timerRunning = true;
			// knead dough instead of picking up the bowl
			if(this.gameObject.GetComponent<Bowl>() != null && this.gameObject.GetComponent<Bowl>().bowlType == Bowl.BowlType.DoughBowl)
			{
				this.gameObject.GetComponent<Bowl>().KneadDough();
				return;
			}
			if(this.gameObject.GetComponent<Topping>() != null)
			{
				this.gameObject.GetComponent<Topping>().PickUp();
			}
			if(this.gameObject.GetComponent<SliceToppings>() != null)
			{
				this.gameObject.GetComponent<SliceToppings>().PickUp();
			}
			if(this.gameObject.GetComponent<NetworkTomatoDrawer>() != null)
			{
				this.gameObject.GetComponent<NetworkTomatoDrawer>().PickUp();
			}
			if(this.gameObject.GetComponent<ParticlesObject>() != null)
			{
				this.gameObject.GetComponent<ParticlesObject>().PickUp();
			}
			if(this.gameObject.GetComponent<CheeseGrating>() != null && this.gameObject.name == "CheeseGrater")
			{
				this.gameObject.GetComponent<CheeseGrating>().GraterGrabbed();
			}
			if(this.gameObject.name == "Cheese")
			{
				GameObject.FindObjectOfType<CheeseGrating>().CheeseGrabbed();
			}
			if(this.gameObject.CompareTag("DoughBall"))
			{
				this.gameObject.GetComponent<DoughIndicator>().PickUpBall();
			}
			if(this.gameObject.GetComponent<NetworkTomatoDrawer>() != null ||this.gameObject.GetComponent<SliceToppings>() != null ||this.gameObject.GetComponent<PizzaShovel>() != null)
			{
				keepUpright = true;
			}
		}

	

		/// <summary>
		/// Called when this stops being grabbed.
		/// </summary>
		/// <param name="p_Object">Contains information about the grab</param>
		public void OnGrabbedEnd(GrabbedObject p_Object)
		{
			this.gameObject.GetComponent<GrabbedItem>().Release();
			this.gameObject.GetComponent<GrabbedItem>().EnableAllColliders();
			if(this.gameObject.GetComponent<Topping>() != null)
			{
				this.gameObject.GetComponent<Topping>().LetGo();
			}
			if(this.gameObject.GetComponent<NetworkTomatoDrawer>() != null)
			{
				this.gameObject.GetComponent<NetworkTomatoDrawer>().LetGo();
			}
			if(this.gameObject.GetComponent<ParticlesObject>() != null)
			{
				this.gameObject.GetComponent<ParticlesObject>().LetGo();
			}
			if(this.gameObject.GetComponent<SliceToppings>() != null)
			{
				this.gameObject.GetComponent<SliceToppings>().LetGo();
			}
			if(this.gameObject.GetComponent<CheeseGrating>() != null && this.gameObject.name == "CheeseGrater")
			{
				this.gameObject.GetComponent<CheeseGrating>().GraterReleased();
			}
			if(this.gameObject.GetComponent<CheeseGrating>() != null && this.gameObject.name == "Cheese")
			{
				this.gameObject.GetComponent<CheeseGrating>().CheeseReleased();
			}
			if(this.gameObject.CompareTag("DoughBall"))
			{
				this.gameObject.GetComponent<DoughIndicator>().LetGoBall();
			}
			if(this.gameObject.GetComponent<NetworkTomatoDrawer>() != null ||this.gameObject.GetComponent<SliceToppings>() != null ||this.gameObject.GetComponent<PizzaShovel>() != null)
			{
				keepUpright = false;
			}
			
		}


		/// <summary>
		/// Called when a new grabber starts grabbing this.
		/// </summary>
		/// <param name="p_Object">Contains information about the grab</param>
		/// <param name="p_Info">Contains information about the added grabber</param>
		public void OnAddedInteractingInfo(GrabbedObject p_Object, GrabbedObject.Info p_Info)
		{
		}

		/// <summary>
		/// Called when a grabber stops grabbing this.
		/// </summary>
		/// <param name="p_Object">Contains information about the grab</param>
		/// <param name="p_Info">Contains information about the removed grabber</param>
		public void OnRemovedInteractingInfo(GrabbedObject p_Object, GrabbedObject.Info p_Info)
		{
		}

		/// <summary>
		/// Called every FixedUpdate when this is grabbed.
		/// This is where the position and rotation of the object is determined according to the hands.
		/// </summary>
		/// <param name="p_Object">Contains information about the grab</param>
		public void OnGrabbedFixedUpdate(GrabbedObject p_Object)
		{
			// Written by Fangio to implement the kneading of the dough in the bowl of water and flour
			// bowl should not be picked up
			if(this.gameObject.GetComponent<Bowl>() != null && this.gameObject.GetComponent<Bowl>().bowlType == Bowl.BowlType.DoughBowl)
			{
				return;
			}

			Vector3 t_Pos = Vector3.zero;

			Quaternion t_Rot = transform.rotation;
			if (p_Object.hands.Count == 1)
			{
				GrabbedObject.Info t_Info = p_Object.hands[0];
				t_Pos = t_Info.interacter.hand.transform.TransformPoint(t_Info.handToObject);
				t_Rot = t_Info.interacter.hand.transform.rotation * t_Info.handToObjectRotation;
			}
			else
			{
				t_Rot = Quaternion.identity;
				Quaternion t_RRot;
				Quaternion t_MRot;
				GrabbedObject.Info t_Info = p_Object.hands[p_Object.hands.Count - 1];

				Quaternion t_PRRot = t_Info.interacter.hand.transform.rotation * t_Info.handToObjectRotation;
				Vector3 t_PCAPt = t_Info.nearestColliderPoint;
				Vector3 t_PIPos = t_Info.interacter.transform.position;
				for (int i = 0; i < p_Object.hands.Count; i++)
				{
					t_Info = p_Object.hands[i];
					t_RRot = t_Info.interacter.hand.transform.rotation * t_Info.handToObjectRotation;

					Vector3 t_CAPt = t_Info.nearestColliderPoint;
					Vector3 t_IPos = t_Info.interacter.transform.position;

					Vector3 t_Before = t_CAPt - t_PCAPt;
					Vector3 t_After = transform.InverseTransformPoint(t_IPos) - transform.InverseTransformPoint(t_PIPos);
					t_MRot = transform.rotation * Quaternion.FromToRotation(t_Before, t_After);

					//Determine the blend through Rotation comparison, distance to original grab point and hand direction
					float t_Dist = 1.0f - Mathf.Clamp01(Vector3.SqrMagnitude(transform.TransformPoint(t_CAPt) - t_IPos) / 0.01f); //10cm
					float t_Blend = Quaternion.Dot(t_RRot, t_PRRot);
					t_Blend = Mathf.Clamp01((t_Blend * 6.0f) - 5.0f);
					t_Blend *= t_Dist;
					t_Blend *= Mathf.Clamp01(Vector3.Dot(t_Info.interacter.transform.forward, transform.TransformDirection(t_Info.objectInteractorForward)));

					t_PCAPt = t_CAPt;
					t_PIPos = t_IPos;
					t_PRRot = t_RRot;
					t_Rot = Quaternion.Lerp(t_Rot, Quaternion.Lerp(t_MRot, t_RRot, t_Blend), 1.0f / (i + 1));
				}

				t_Rot = Quaternion.Lerp(transform.rotation, t_Rot, 0.4f);

				/*
				Quaternion t_PRRot = t_Info.interacter.hand.transform.rotation * t_Info.handToObjectRotation;
				Vector3 t_PCAPt = transform.TransformPoint(t_Info.nearestColliderPoint);
				Vector3 t_PIPos = t_Info.interacter.transform.position;
				for (int i = 0; i < _Object.hands.Count; i++)
				{
					t_Info = _Object.hands[i];
					t_RRot = t_Info.interacter.hand.transform.rotation * t_Info.handToObjectRotation;

					Vector3 t_CAPt = transform.TransformPoint(t_Info.nearestColliderPoint);
					Vector3 t_IPos = t_Info.interacter.transform.position;

					Vector3 t_Before = t_CAPt - t_PCAPt;
					Vector3 t_After = t_IPos - t_PIPos;
					t_MRot = transform.rotation * Quaternion.FromToRotation(transform.InverseTransformDirection(t_Before), transform.InverseTransformDirection(t_After));

					//Determine the blend through Rotation comparison, distance to original grab point and hand direction
					float t_Dist = 1.0f - Mathf.Clamp01(Vector3.SqrMagnitude(t_CAPt - t_IPos) / 0.01f); //10cm
					float t_Blend = Quaternion.Dot(t_RRot, t_PRRot);
					t_Blend = Mathf.Clamp01((t_Blend * 6.0f) - 5.0f);
					t_Blend *= t_Dist;
					float t_FFwd = Vector3.Dot(t_Info.interacter.transform.forward, transform.TransformDirection(t_Info.objectInteractorForward));
					t_Blend *= Mathf.Clamp01(t_FFwd * t_FFwd);

					t_PCAPt = t_CAPt;
					t_PIPos = t_IPos;
					t_PRRot = t_RRot;
					t_Rot = Quaternion.Lerp(t_Rot, Quaternion.Lerp(t_MRot, t_RRot, t_Blend), 1.0f / (i + 1));
				}

				//t_Rot = Quaternion.Lerp(transform.rotation, t_Rot, 0.4f);
				 */

				for (int i = 0; i < p_Object.hands.Count; i++)
				{
					t_Info = p_Object.hands[i];
					Vector3 t_NP = transform.TransformPoint(t_Info.nearestColliderPoint);
					Vector3 t_P = t_Info.interacter.transform.position - t_NP;
					t_Pos += t_P;
					Debug.DrawLine(transform.position, t_NP);
				}
				t_Pos /= p_Object.hands.Count;
				t_Pos = transform.position + t_Pos;
			}

			if (keepUpright)
			{
				// t_Rot =  Quaternion.Euler(0,t_Rot.eulerAngles.y,0);
			}

			if (p_Object.rigidBody)
			{
				p_Object.rigidBody.velocity = Vector3.zero;
				p_Object.rigidBody.angularVelocity = Vector3.zero;
				p_Object.rigidBody.MovePosition(t_Pos);
				p_Object.rigidBody.MoveRotation(t_Rot);
			}
			else
			{
				transform.position = t_Pos;
				transform.rotation = t_Rot;
			}
		}
	}
}
