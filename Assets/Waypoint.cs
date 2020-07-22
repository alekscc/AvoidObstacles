using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    [SerializeField] private Vector3[] waypoints;

    [Header("Debug")]
    [SerializeField] private bool isDebug = false;
    [SerializeField] private Color color = Color.green;
    [SerializeField] private float radius = 2f;

    private System.Random _rand = new System.Random();

    public void CollectWaypointsFromChilds() {

        waypoints = GetWaypoints(transform.GetComponentsInChildren<Transform>()).ToArray();

    }

    private IEnumerable<Vector3> GetWaypoints(Transform[] waypoints) {

        for(int i = 1; i < waypoints.Length; i++) {

            waypoints[i].name = "waypoint" + i;

            yield return waypoints[i].position;

        }

    }
    public IEnumerable<Vector3> GetRandomPosition() {

        for(int i = 1; i < waypoints.Length; i++) {

            yield return Vector3.Lerp(waypoints[i - 1], waypoints[i], (float)_rand.NextDouble());

        }

    }
    private void OnDrawGizmos() {

        if (waypoints.Length <= 0)
            return;

        Gizmos.color = color;

        Gizmos.DrawSphere(waypoints[0], radius);

        for (int i = 1; i < waypoints.Length; i++) {

            Gizmos.DrawSphere(waypoints[i], radius);
            Gizmos.DrawLine(waypoints[i - 1], waypoints[i]);
        }

    }

}
