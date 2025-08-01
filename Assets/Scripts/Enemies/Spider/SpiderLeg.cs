using System.Collections.Generic;
using MEC;
using RootMotion.FinalIK;
using UnityEngine;
using UnityEngine.Serialization;

public class SpiderLeg : MonoBehaviour
{
    public Spider mechSpider; // Reference to the target
		public SpiderLeg unSync; // One of the other legs that we dont want to be completely in sync with, that is stepping at the same time
		public Vector3 offset; // Offset from the default position
		public float minDelay = 0.2f, maxOffset = 1.0f, stepSpeed = 5.0f, footHeight = 0.15f, velocityPrediction = 0.2f, raycastFocus = 0.1f; // Parameters for stepping
		public AnimationCurve yOffset;
        public Transform foot;
        public Vector3 footUpAxis;
        public float footRotationSpeed = 10f;
        
        [FormerlySerializedAs("sand")] public ParticleSystem stepParticle; // FX for sand

		private IK ik;
		private float stepProgress = 1f, lastStepTime;
		private Vector3 defaultPosition;
		private RaycastHit hit = new RaycastHit();
        private Quaternion lastFootLocalRotation;
        private Vector3 smoothHitNormal = Vector3.up;
        private Vector3 lastStepPosition;

        // Is the leg stepping?
        public bool isStepping {
			get {
				return stepProgress < 1f;
			}
		}

		// Gets and sets the IK position for this leg
		public Vector3 position {
			get {
				return ik.GetIKSolver().GetIKPosition();
			}
			set {
				ik.GetIKSolver().SetIKPosition(value);
			}
		}
		
        void Awake()
        {
            // Find the ik component
            ik = GetComponent<IK>();

            if (foot != null)
            {
                if (footUpAxis == Vector3.zero) footUpAxis = Quaternion.Inverse(foot.rotation) * Vector3.up;
                lastFootLocalRotation = foot.localRotation;
                ik.GetIKSolver().OnPostUpdate += AfterIK;
            }
        }

        private void AfterIK()
        {
            if (foot == null) return;
            foot.localRotation = lastFootLocalRotation;

            smoothHitNormal = Vector3.Slerp(smoothHitNormal, hit.normal, Time.deltaTime * footRotationSpeed);
            Quaternion f = Quaternion.FromToRotation(foot.rotation * footUpAxis, smoothHitNormal);
            foot.rotation = f * foot.rotation;
        }

        void Start() {
			// Workaround for Unity Win Store/Phone serialization bug
			stepProgress = 1f;
			
			hit = new RaycastHit();
			
			var points = ik.GetIKSolver().GetPoints();
			position = points[points.Length - 1].transform.position;
            lastStepPosition = position;

			hit.point = position;

			// Store the default rest position of the leg
			defaultPosition = mechSpider.transform.InverseTransformPoint(position + offset * mechSpider.scale);

            Timing.RunCoroutine(Step(position, position).CancelWith(gameObject));
        }

		// Find the relaxed grounded positon of the leg relative to the body in world space.
		private Vector3 GetStepTarget(out bool stepFound, float focus, float distance) {
			stepFound = false;

			// place hit.point to the default position relative to the body
			Vector3 stepTarget = mechSpider.transform.TransformPoint(defaultPosition);
            //stepTarget += (hit.point - position) * velocityPrediction;
            stepTarget += mechSpider.velocity * velocityPrediction;

			Vector3 up = mechSpider.transform.up;

			// Focus the ray directions towards the spider body
			Vector3 toBody = mechSpider.body.position - position;
			Vector3 axis = Vector3.Cross(up, toBody);
			up = Quaternion.AngleAxis(focus, axis) * up;

			// Raycast to ground the relaxed position
			if (Physics.Raycast(stepTarget + up * mechSpider.raycastHeight * mechSpider.scale, -up, out hit, mechSpider.raycastHeight * mechSpider.scale + distance, mechSpider.raycastLayers, QueryTriggerInteraction.Ignore)) stepFound = true;

            //return hit.point + mechSpider.transform.up * footHeight * mechSpider.scale;
            return hit.point + hit.normal * footHeight * mechSpider.scale;
        }

        private void UpdatePosition(float distance)
        {
            Vector3 up = mechSpider.transform.up;
            
            if (Physics.Raycast(lastStepPosition + up * mechSpider.raycastHeight * mechSpider.scale, -up, out hit, mechSpider.raycastHeight * mechSpider.scale + distance, mechSpider.raycastLayers, QueryTriggerInteraction.Ignore))
            {
                position = hit.point + hit.normal * footHeight * mechSpider.scale;
            }
        }

        void Update () {
            UpdatePosition(mechSpider.raycastDistance * mechSpider.scale);

            // if already stepping, do nothing
            if (isStepping) return;

			// Minimum delay before stepping again
			if (Time.time < lastStepTime + minDelay) return;

			// If the unSync leg is stepping, do nothing
			if (unSync != null) {
				if (unSync.isStepping) return;
			}

			// Find the ideal relaxed position for the leg relative to the body
			bool stepFound = false;
			Vector3 idealPosition = GetStepTarget(out stepFound, raycastFocus, mechSpider.raycastDistance * mechSpider.scale);
			if (!stepFound) idealPosition = GetStepTarget(out stepFound, -raycastFocus, mechSpider.raycastDistance * 3f * mechSpider.scale); // Try again with inverted focus
			if (!stepFound) return;

			// If distance to that ideal position is less than the threshold, do nothing
			if (Vector3.Distance(position, idealPosition) < maxOffset * mechSpider.scale * UnityEngine.Random.Range(0.9f, 1.2f)) return;

			// Need to step closer to the ideal position
			StopAllCoroutines();
			Timing.RunCoroutine(Step(position, idealPosition));
		}

		// Stepping co-routine
		private IEnumerator<float> Step(Vector3 stepStartPosition, Vector3 targetPosition) {
			stepProgress = 0f;

			// Moving the IK position
			while (stepProgress < 1) {
				stepProgress += Time.deltaTime * stepSpeed;
				
				position = Vector3.Lerp(stepStartPosition, targetPosition, stepProgress);
				position += mechSpider.transform.up * yOffset.Evaluate(stepProgress) * mechSpider.scale;
                lastStepPosition = position;

				yield return Timing.WaitForOneFrame;
			}

			position = targetPosition;
            lastStepPosition = position;

			// Emit sand
			if (stepParticle != null) {
				stepParticle.transform.position = position - mechSpider.transform.up * footHeight * mechSpider.scale;
				stepParticle.Emit(20);
			}
			
			lastStepTime = Time.time;
		}
	}
