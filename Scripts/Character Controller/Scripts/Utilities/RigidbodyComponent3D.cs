using UnityEngine;

namespace ShadowFort.Utilities
{
    /// <summary>
    /// An implementation of RigidbodyComponent for 3D rigidbodies.
    /// </summary>
    public sealed class RigidbodyComponent3D : RigidbodyComponent
    {
        private Rigidbody rb = null;

        protected override bool IsUsingContinuousCollisionDetection => rb.collisionDetectionMode > 0;

        public override HitInfo Sweep(Vector3 position, Vector3 direction, float distance)
        {
            var p = Position;
            Position = position;
            rb.SweepTest(direction, out RaycastHit raycastHit, distance);
            Position = p;
            return new HitInfo(ref raycastHit, direction);
        }

        protected override void Awake()
        {
            base.Awake();
            rb = gameObject.GetOrAddComponent<Rigidbody>();
            rb.hideFlags = HideFlags.NotEditable;

            previousContinuousCollisionDetection = IsUsingContinuousCollisionDetection;
        }


        public override bool Is2D => false;

        public override float Mass
        {
            get => rb.mass;
            set => rb.mass = value;
        }

        public override float LinearDrag
        {
#if UNITY_6000_0_OR_NEWER
            get => rb.linearDamping;
            set => rb.linearDamping = value;
#else
            get => rigidbody.drag;
            set => rigidbody.drag = value;
#endif
        }

        public override float AngularDrag
        {
#if UNITY_6000_0_OR_NEWER
            get => rb.angularDamping;
            set => rb.angularDamping = value;
#else
            get => rigidbody.angularDrag;
            set => rigidbody.angularDrag = value;            
#endif
        }


        public override bool IsKinematic
        {
            get => rb.isKinematic;
            set
            {
                if (value == IsKinematic)
                    return;

                // Since CCD can't be true for kinematic bodies, the body type must change to dynamic before setting CCD
                if (value)
                {
                    ContinuousCollisionDetection = false;
                    rb.isKinematic = true;
                }
                else
                {
                    rb.isKinematic = false;
                    ContinuousCollisionDetection = previousContinuousCollisionDetection;
                }

                InvokeOnBodyTypeChangeEvent();
            }
        }

        public override bool UseGravity
        {
            get => rb.useGravity;
            set => rb.useGravity = value;
        }

        public override bool UseInterpolation
        {
            get => rb.interpolation == RigidbodyInterpolation.Interpolate;
            set => rb.interpolation = value ? RigidbodyInterpolation.Interpolate : RigidbodyInterpolation.None;
        }

        public override bool ContinuousCollisionDetection
        {
            get => rb.collisionDetectionMode == CollisionDetectionMode.Continuous;
            set => rb.collisionDetectionMode = value ? CollisionDetectionMode.Continuous : CollisionDetectionMode.Discrete;
        }

        public override RigidbodyConstraints Constraints
        {
            get => rb.constraints;
            set => rb.constraints = value;
        }

        public override Vector3 Position
        {
            get => rb.position;
            set => rb.position = value;
        }

        public override Quaternion Rotation
        {
            get => rb.rotation;
            set => rb.rotation = value;
        }

        public override Vector3 Velocity
        {
#if UNITY_6000_0_OR_NEWER
            get => rb.linearVelocity;
            set => rb.linearVelocity = value;
#else
            get => rigidbody.velocity;
            set => rigidbody.velocity = value;            
#endif
        }

        public override Vector3 AngularVelocity
        {
            get => rb.angularVelocity;
            set => rb.angularVelocity = value;
        }

        public override void Interpolate(Vector3 position) => rb.MovePosition(position);
        public override void Interpolate(Quaternion rotation) => rb.MoveRotation(rotation);
        public override Vector3 GetPointVelocity(Vector3 point) => rb.GetPointVelocity(point);
        public override void AddForceToRigidbody(Vector3 force, ForceMode forceMode = ForceMode.Force) => rb.AddForce(force, forceMode);
        public override void AddExplosionForceToRigidbody(float explosionForce, Vector3 explosionPosition, float explosionRadius, float upwardsModifier = 0) => rb.AddExplosionForce(explosionForce, explosionPosition, explosionRadius, upwardsModifier);
    }

}