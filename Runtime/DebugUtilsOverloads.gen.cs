//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using UnityEngine;

namespace Vertx.Debugging
{
	public static partial class DebugUtils
	{
		public static void DrawSphereCast(Ray ray, float radius, float distance, int iterationCount = 10)
			=> DrawSphereCast(ray, radius, distance, StartColor, EndColor, iterationCount);

		public static void DrawSphereCast(Vector3 origin, float radius, Vector3 direction, float distance, int iterationCount = 10)
			=> DrawSphereCast(origin, radius, direction, distance, StartColor, EndColor, iterationCount);

		public static void DrawBoxCast(Vector3 center, Vector3 halfExtents, Vector3 direction, Quaternion orientation, float distance, int iterationCount = 1)
			=> DrawBoxCast(center, halfExtents, direction, orientation, distance, StartColor, EndColor, iterationCount);

		public static void DrawCapsuleCast(Vector3 point1, Vector3 point2, float radius, Vector3 direction, float distance, int iterationCount = 10)
			=> DrawCapsuleCast(point1, point2, radius, direction, distance, StartColor, EndColor, iterationCount);

		public static void DrawRaycastHits(RaycastHit[] hits, int maxCount = -1, float rayLength = 1, float duration = 0)
			=> DrawRaycastHits(hits, HitColor, maxCount, rayLength, duration);

		public static void DrawSphereCastHits(RaycastHit[] hits, Ray ray, float radius, int maxCount = -1)
			=> DrawSphereCastHits(hits, ray, radius, HitColor, maxCount);

		public static void DrawSphereCastHits(RaycastHit[] hits, Vector3 origin, float radius, Vector3 direction, int maxCount = -1)
			=> DrawSphereCastHits(hits, origin, radius, direction, HitColor, maxCount);

		public static void DrawBoxCastHits(RaycastHit[] hits, Vector3 origin, Vector3 halfExtents, Vector3 direction, Quaternion orientation, int maxCount = -1)
			=> DrawBoxCastHits(hits, origin, halfExtents, direction, orientation, HitColor, maxCount);

		public static void DrawCapsuleCastHits(RaycastHit[] hits, Vector3 point1, Vector3 point2, float radius, Vector3 direction, int maxCount = -1)
			=> DrawCapsuleCastHits(hits, HitColor, point1, point2, radius, direction, maxCount);

		public static void DrawRaycast(Ray ray, RaycastHit[] hits, float distance, int maxCount = -1, float hitRayLength = 1, float duration = 0)
			=> DrawRaycast(ray, hits, distance, StartColor, HitColor, maxCount, hitRayLength, duration);

		public static void DrawSphereCast(Vector3 origin, float radius, Vector3 direction, RaycastHit[] hits, float distance, int count)
			=> DrawSphereCast(origin, radius, direction, hits, distance, count, StartColor, EndColor, HitColor);

		public static void DrawBoxCast(Vector3 center, Vector3 halfExtents, Vector3 direction, RaycastHit[] hits, Quaternion orientation, float distance, int count)
			=> DrawBoxCast(center, halfExtents, direction, hits, orientation, distance, count, StartColor, EndColor, HitColor);

		public static void DrawCapsuleCast(Vector3 point1, Vector3 point2, float radius, Vector3 direction, RaycastHit[] hits, float distance, int count)
			=> DrawCapsuleCast(point1, point2, radius, direction, hits, distance, count, StartColor, EndColor, HitColor);

		public static void DrawCircleCast2D(Vector2 origin, float radius, Vector2 direction, float distance)
			=> DrawCircleCast2D(origin, radius, direction, distance, StartColor, EndColor);

		public static void DrawBoxCast2D(Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance)
			=> DrawBoxCast2D(origin, size, angle, direction, distance, StartColor, EndColor);

		public static void DrawCapsuleCast2D(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, float distance)
			=> DrawCapsuleCast2D(origin, size, capsuleDirection, angle, direction, distance, StartColor, EndColor);

		public static void DrawRaycast2DHits(RaycastHit2D[] hits, int maxCount = -1, float rayLength = 1, float duration = 0)
			=> DrawRaycast2DHits(hits, HitColor, maxCount, rayLength, duration);

		public static void DrawBoxCast2DHits(RaycastHit2D[] hits, Vector2 origin, Vector2 size, float angle, Vector2 direction, int maxCount = -1)
			=> DrawBoxCast2DHits(hits, origin, size, angle, direction, HitColor, maxCount);

		public static void DrawCircleCast2DHits(RaycastHit2D[] hits, Vector2 origin, float radius, Vector2 direction, int maxCount = -1)
			=> DrawCircleCast2DHits(hits, origin, radius, direction, HitColor, maxCount);

		public static void DrawCapsuleCast2DHits(RaycastHit2D[] hits, Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, int maxCount = -1)
			=> DrawCapsuleCast2DHits(hits, origin, size, capsuleDirection, angle, direction, HitColor, maxCount);

		public static void DrawRaycast2D(Vector2 origin, Vector2 direction, RaycastHit2D[] hits, float distance, int maxCount = -1, float hitRayLength = 1, float duration = 0)
			=> DrawRaycast2D(origin, direction, hits, distance, StartColor, HitColor, maxCount, hitRayLength, duration);

		public static void DrawCircleCast2D(Vector2 origin, float radius, Vector2 direction, RaycastHit2D[] hits, float distance, int count)
			=> DrawCircleCast2D(origin, radius, direction, hits, distance, count, StartColor, EndColor, HitColor);

		public static void DrawBoxCast2D(Vector2 origin, Vector2 size, float angle, Vector2 direction, RaycastHit2D[] hits, float distance, int count)
			=> DrawBoxCast2D(origin, size, angle, direction, hits, distance, count, StartColor, EndColor, HitColor);

		public static void DrawCapsuleCast2D(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, RaycastHit2D[] hits, float distance, int count)
			=> DrawCapsuleCast2D(origin, size, capsuleDirection, angle, direction, hits, distance, count, StartColor, EndColor, HitColor);


	}
}