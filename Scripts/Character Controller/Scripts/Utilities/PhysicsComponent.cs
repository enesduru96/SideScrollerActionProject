﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShadowFort.Utilities
{

    /// <summary>
    /// This component offers a set of useful functionalities for 2D and 3D physics.
    /// </summary>
    public abstract class PhysicsComponent : MonoBehaviour
    {
        public List<HitInfo> HitsBuffer { get; protected set; } = new List<HitInfo>(50);

        /// <summary>
        /// Gets a list with all the current contacts.
        /// </summary>
        public List<Contact> Contacts { get; protected set; } = new List<Contact>(50);

        /// <summary>
        /// Gets a list with all the current triggers.
        /// </summary>
        public List<Trigger> Triggers { get; protected set; } = new List<Trigger>(50);

        protected abstract LayerMask GetCollisionLayerMask();

        /// <summary>
        /// Ignores the collision between this object and some other collider.
        /// </summary>
        public abstract void IgnoreCollision(in HitInfo hitInfo, bool ignore);

        /// <summary>
        /// Ignores the collision between this object and some other collider.
        /// </summary>
        public abstract void IgnoreCollision(Transform otherTransform, bool ignore);

        /// <summary>
        /// Ignores the collision between this object and a layer.
        /// </summary>
        public abstract void IgnoreLayerCollision(int targetLayer, bool ignore);

        /// <summary>
        /// Ignores the collision between this object and a layer mask.
        /// </summary>
        public abstract void IgnoreLayerMaskCollision(LayerMask layerMask, bool ignore);
        protected abstract int FilterOverlaps(int overlaps, LayerMask ignoredLayerMask, HitFilterDelegate hitFilter);

        public void ClearContacts() => Contacts.Clear();

        protected abstract void GetClosestHit(out HitInfo hitInfo, int hits, Vector3 castDisplacement, in HitInfoFilter filter, bool allowOverlaps, HitFilterDelegate hitFilter);
        protected abstract List<HitInfo> GetAllHits(int hits, Vector3 castDisplacement, in HitInfoFilter filter, bool allowOverlaps, HitFilterDelegate hitFilter);

        protected abstract int InternalRaycast(Vector3 origin, Vector3 castDisplacement, int layerMask, bool ignoreTriggers, bool allowOverlaps);
        protected abstract int InternalSphereCast(Vector3 center, float radius, Vector3 castDisplacement, int layerMask, bool ignoreTriggers, bool allowOverlaps);
        protected abstract int InternalCapsuleCast(Vector3 bottom, Vector3 top, float radius, Vector3 castDisplacement, int layerMask, bool ignoreTriggers, bool allowOverlaps);
        protected abstract int InternalBoxCast(Vector3 center, Vector3 size, Vector3 castDisplacement, Quaternion orientation, int layerMask, bool ignoreTriggers, bool allowOverlaps);
        protected abstract int InternalOverlapSphere(Vector3 center, float radius, int layerMask, bool ignoreTriggers);
        protected abstract int InternalOverlapCapsule(Vector3 bottom, Vector3 top, float radius, int layerMask, bool ignoreTriggers);
        protected abstract int InternalOverlapBox(Vector3 center, Vector3 size, Quaternion orientation, int layerMask, bool ignoreTriggers);

        /// <summary>
        /// Returns true if collisions between this collider and the target GameObject is valid at the physics simulation level.
        /// </summary>
        public abstract bool CheckCollisionsWith(GameObject gameObject);

        RigidbodyComponent rigidbodyComponent = null;
        Coroutine postSimulationCoroutine = null;

        #region Physics queries

        /// <summary>
        /// Performs a Raycast and gets the closest valid hit.
        /// </summary>
        public int Raycast(out HitInfo hitInfo, Vector3 origin, Vector3 castDisplacement, in HitInfoFilter filter, bool allowOverlaps = false, HitFilterDelegate hitFilter = null)
        {
            var hits = InternalRaycast(
                origin,
                castDisplacement,
                filter.collisionLayerMask,
                filter.ignoreTriggers,
                allowOverlaps
            );

            if (hits != 0)
                GetClosestHit(out hitInfo, hits, castDisplacement, in filter, allowOverlaps, hitFilter);
            else
                hitInfo = new HitInfo();

            return hits;
        }

        /// <summary>
        /// Performs a Raycast and gets all the valid hits.
        /// </summary>
        public List<HitInfo> Raycast(Vector3 origin, Vector3 castDisplacement, in HitInfoFilter filter, bool allowOverlaps = false, HitFilterDelegate hitFilter = null)
        {
            var hits = InternalRaycast(
                origin,
                castDisplacement,
                filter.collisionLayerMask,
                filter.ignoreTriggers,
                allowOverlaps
            );

            if (hits != 0)
                return GetAllHits(hits, castDisplacement, in filter, allowOverlaps, hitFilter);
            else
                return null;
        }

        /// <summary>
        /// Performs a SphereCast and gets the closest valid hit.
        /// </summary>
        public int SphereCast(out HitInfo hitInfo, Vector3 center, float radius, Vector3 castDisplacement, in HitInfoFilter filter, bool allowOverlaps = false, HitFilterDelegate hitFilter = null)
        {
            var hits = InternalSphereCast(
                center,
                radius,
                castDisplacement,
                filter.collisionLayerMask,
                filter.ignoreTriggers,
                allowOverlaps
            );

            if (hits != 0)
                GetClosestHit(out hitInfo, hits, castDisplacement, in filter, allowOverlaps, hitFilter);
            else
                hitInfo = new HitInfo();

            return hits;
        }

        /// <summary>
        /// Performs a SphereCast and gets all the valid hits.
        /// </summary>
        public List<HitInfo> SphereCast(Vector3 center, float radius, Vector3 castDisplacement, in HitInfoFilter filter, bool allowOverlaps = false, HitFilterDelegate hitFilter = null)
        {
            var hits = InternalSphereCast(
                center,
                radius,
                castDisplacement,
                filter.collisionLayerMask,
                filter.ignoreTriggers,
                allowOverlaps
            );

            if (hits != 0)
                return GetAllHits(hits, castDisplacement, in filter, allowOverlaps, hitFilter);
            else
                return null;
        }

        /// <summary>
        /// Performs a CapsuleCast and gets the closest valid hit.
        /// </summary>
        public int CapsuleCast(out HitInfo hitInfo, Vector3 bottom, Vector3 top, float radius, Vector3 castDisplacement, in HitInfoFilter filter, bool allowOverlaps = false, HitFilterDelegate hitFilter = null)
        {
            var hits = InternalCapsuleCast(
                bottom,
                top,
                radius,
                castDisplacement,
                filter.collisionLayerMask,
                filter.ignoreTriggers,
                allowOverlaps
            );

            if (hits != 0)
                GetClosestHit(out hitInfo, hits, castDisplacement, in filter, allowOverlaps, hitFilter);
            else
                hitInfo = new HitInfo();

            return hits;
        }

        /// <summary>
        /// Performs a CapsuleCast and gets all the valid hits.
        /// </summary>
        public List<HitInfo> CapsuleCast(Vector3 bottom, Vector3 top, float radius, Vector3 castDisplacement, in HitInfoFilter filter, bool allowOverlaps = false, HitFilterDelegate hitFilter = null)
        {
            var hits = InternalCapsuleCast(
                bottom,
                top,
                radius,
                castDisplacement,
                filter.collisionLayerMask,
                filter.ignoreTriggers,
                allowOverlaps
            );

            if (hits != 0)
                return GetAllHits(hits, castDisplacement, in filter, allowOverlaps, hitFilter);
            else
                return null;
        }

        /// <summary>
        /// Performs a BoxCast and gets the closest valid hit.
        /// </summary>
        public int BoxCast(out HitInfo hitInfo, Vector3 center, Vector3 size, Vector3 castDisplacement, Quaternion orientation, in HitInfoFilter filter, bool allowOverlaps = false, HitFilterDelegate hitFilter = null)
        {
            var hits = InternalBoxCast(
                center,
                size,
                castDisplacement,
                orientation,
                filter.collisionLayerMask,
                filter.ignoreTriggers,
                allowOverlaps
            );

            if (hits != 0)
                GetClosestHit(out hitInfo, hits, castDisplacement, in filter, allowOverlaps, hitFilter);
            else
                hitInfo = new HitInfo();

            return hits;
        }

        /// <summary>
        /// Performs a BoxCast and gets all the valid hits.
        /// </summary>
        public List<HitInfo> BoxCast(Vector3 center, Vector3 size, Vector3 castDisplacement, Quaternion orientation, in HitInfoFilter filter, bool allowOverlaps = false, HitFilterDelegate hitFilter = null)
        {
            var hits = InternalBoxCast(
                center,
                size,
                castDisplacement,
                orientation,
                filter.collisionLayerMask,
                filter.ignoreTriggers,
                allowOverlaps
            );

            if (hits != 0)
                return GetAllHits(hits, castDisplacement, in filter, allowOverlaps, hitFilter);
            else
                return null;
        }

        #endregion

        #region Overlaps

        /// <summary>
        /// Performs an OverlapSphere and returns true if any of the results is valid.
        /// </summary>
        public bool OverlapSphere(Vector3 center, float radius, in HitInfoFilter filter, HitFilterDelegate hitFilter = null)
        {
            var overlaps = InternalOverlapSphere(
                center,
                radius,
                filter.collisionLayerMask,
                filter.ignoreTriggers
            );

            var filteredOverlaps = FilterOverlaps(overlaps, filter.ignorePhysicsLayerMask, hitFilter);
            return filteredOverlaps != 0;
        }

        /// <summary>
        /// Performs an OverlapCapsule and returns true if any of the results is valid.
        /// </summary>
        public bool OverlapCapsule(Vector3 bottom, Vector3 top, float radius, in HitInfoFilter filter, HitFilterDelegate hitFilter = null)
        {
            var overlaps = InternalOverlapCapsule(
                bottom,
                top,
                radius,
                filter.collisionLayerMask,
                filter.ignoreTriggers
            );

            var filteredOverlaps = FilterOverlaps(overlaps, filter.ignorePhysicsLayerMask, hitFilter);
            return filteredOverlaps != 0;
        }

        /// <summary>
        /// Performs an OverlapBox and returns true if any of the results is valid.
        /// </summary>
        public bool OverlapBox(Vector3 center, Vector3 size, Quaternion orientation, in HitInfoFilter filter, HitFilterDelegate hitFilter = null)
        {
            var overlaps = InternalOverlapBox(
                center,
                size,
                orientation,
                filter.collisionLayerMask,
                filter.ignoreTriggers
            );

            var filteredOverlaps = FilterOverlaps(overlaps, filter.ignorePhysicsLayerMask, hitFilter);
            return filteredOverlaps != 0;
        }

        #endregion

        /// <summary>
        /// Returns a layer mask with all the valid collisions associated with the object, based on the collision matrix (physics settings).
        /// </summary>
        public LayerMask CollisionLayerMask { get; protected set; } = 0;

        protected virtual void Awake()
        {
            this.hideFlags = HideFlags.None;

            CollisionLayerMask = GetCollisionLayerMask();
        }

        protected virtual void Start()
        {
            rigidbodyComponent = GetComponent<RigidbodyComponent>();

        }

        public static PhysicsComponent CreateInstance(GameObject gameObject)
        {
            Rigidbody rigidbody3D = gameObject.GetComponent<Rigidbody>();


            if (rigidbody3D != null)
                return gameObject.GetOrAddComponent<PhysicsComponent3D>();

            return null;
        }

        protected bool ignoreCollisionMessages = false;



        void OnEnable()
        {
            rigidbodyComponent = GetComponent<RigidbodyComponent>();

            if (rigidbodyComponent != null)
                rigidbodyComponent.OnBodyTypeChange += OnBodyTypeChange;

            postSimulationCoroutine ??= StartCoroutine(PostSimulationUpdate());
        }

        void OnDisable()
        {
            if (rigidbodyComponent != null)
                rigidbodyComponent.OnBodyTypeChange -= OnBodyTypeChange;

            if (postSimulationCoroutine != null)
            {
                StopCoroutine(PostSimulationUpdate());
                postSimulationCoroutine = null;
            }
        }

        void OnBodyTypeChange() => ignoreCollisionMessages = true;

        void FixedUpdate()
        {
            // Update the collision layer mask (collision matrix) of this object.
            // CollisionLayerMask = GetCollisionLayerMask();
            // --> Performance cost! This has been replaced by an internal mask that's modified every time IgnoreCollision is called. <--

            // If there are null triggers then delete them from the list
            for (int i = Triggers.Count - 1; i >= 0; i--)
            {
                if (Triggers[i].gameObject == null)
                    Triggers.RemoveAt(i);
            }
        }

        IEnumerator PostSimulationUpdate()
        {
            YieldInstruction waitForFixedUpdate = new WaitForFixedUpdate();
            while (true)
            {
                yield return waitForFixedUpdate;

                ignoreCollisionMessages = false;

            }
        }

        protected bool wasKinematic = false;
    }

}

