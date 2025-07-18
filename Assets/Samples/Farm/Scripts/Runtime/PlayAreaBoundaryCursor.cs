namespace VRTK.Examples
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.XR;

    public class PlayAreaBoundaryCursor : MonoBehaviour
    {
        public float activationDistance = 0.5f;
        public LineRenderer boundaryLineRenderer;

        private List<Vector3> boundaryPoints = new List<Vector3>();
        private bool boundaryVisible = false;

        void Update()
        {
            if (InputDevices.GetDeviceAtXRNode(XRNode.Head).TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 headPosition))
            {
                float closestDistance = float.MaxValue;
                if (boundaryPoints.Count > 0)
                {
                    for (int i = 0; i < boundaryPoints.Count; i++)
                    {
                        Vector3 p1 = boundaryPoints[i];
                        Vector3 p2 = boundaryPoints[(i + 1) % boundaryPoints.Count];
                        float distance = DistanceToLineSegment(headPosition, p1, p2);
                        if (distance < closestDistance)
                        {
                            closestDistance = distance;
                        }
                    }

                    if (closestDistance < activationDistance)
                    {
                        if (!boundaryVisible)
                        {
                            UpdateBoundary();
                            boundaryLineRenderer.enabled = true;
                            boundaryVisible = true;
                        }
                    }
                    else
                    {
                        if (boundaryVisible)
                        {
                            boundaryLineRenderer.enabled = false;
                            boundaryVisible = false;
                        }
                    }
                }
                else
                {
                    UpdateBoundary();
                }
            }
        }

        void UpdateBoundary()
        {
            boundaryPoints.Clear();
            if (InputDevices.GetDeviceAtXRNode(XRNode.TrackingReference).TryGetBoundaryPoints(boundaryPoints))
            {
                boundaryLineRenderer.positionCount = boundaryPoints.Count;
                boundaryLineRenderer.SetPositions(boundaryPoints.ToArray());
            }
        }

        float DistanceToLineSegment(Vector3 point, Vector3 p1, Vector3 p2)
        {
            float l2 = (p1 - p2).sqrMagnitude;
            if (l2 == 0.0f)
                return Vector3.Distance(point, p1);
            float t = Mathf.Max(0, Mathf.Min(1, Vector3.Dot(point - p1, p2 - p1) / l2));
            Vector3 projection = p1 + t * (p2 - p1);
            return Vector3.Distance(point, projection);
        }
    }
}
