using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;



public class RaySensor3D : MonoBehaviour
{

    [Header("3D ray configuration")]
    [Tooltip("Angles of rays")]
    [SerializeField] private float[] angles;

    //[Tooltip("Automatic fill array with angles")]
    //[SerializeField] private bool isAuto;
    [Tooltip("Use to define field of view")]
    [SerializeField] private float minAngle, maxAngle;
    [Tooltip("Max distance of casting rays")]
    [SerializeField] private float maxDistance;
    [Tooltip("Offset of rays")]
    [SerializeField] private Vector3 offset;
    [Tooltip("Axel to rotate around")]
    [SerializeField] private Vector3 axle = Vector3.forward;

    [Header("Debug")]
    [SerializeField] private bool isDebugMode = false;

    public RaycastHit[] RayHits { get; private set; }

    private LineRenderer lineRenderer;

    public void SetDrawLines(bool isDrawLines) {
        lineRenderer.enabled = isDrawLines;
    }

    private float[] NormalizedData {
        get {

            return RayHits.Select(x => x.distance / maxDistance).ToArray();

        }
    }
    public float[] GetNormalizedData() {
        CheckRayContacts();
        return NormalizedData;
    }
    //public void Start()
    //{
    //    AutoSetup(9,60f);
    //}
    //private void LateUpdate() {
    //    CheckRayContacts();
    //}
    public void AutoSetup(int nRays,float fov)
    {
        angles = new float[nRays];

        RayHits = new RaycastHit[angles.Length];

        minAngle = -fov;
        maxAngle = fov;


        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = nRays * 2;
        lineRenderer.startColor = Color.white;
        lineRenderer.endColor = Color.white;



        float totalAngle = maxAngle - minAngle;
        float step = totalAngle / (angles.Length - 1);
        float cur = minAngle;
        int i = 0;
        while (cur <= maxAngle)
        {
            angles[i++] = cur;
            cur += step;
            //lineRenderer.SetPosition(0,)
        }

        lineRenderer.enabled = false;
    }
    //void FixedUpdate()
    //{
    //    CheckRayContacts();  
    //}
    void CheckRayContacts()
    {
        Vector3 pos = transform.position + offset;

        int j=0;
        for(int i = 0; i < angles.Length; i++)
        {
            //Debug.Log($"angle[{i}]={angles[i]}");
            Vector3 _direction = transform.TransformDirection(Quaternion.AngleAxis(angles[i], axle) * Vector3.forward);

            Physics.Raycast(pos, _direction, out RayHits[i], maxDistance, 1);


            Vector3 point = RayHits[i].point;
            if (point == Vector3.zero) {
                RayHits[i].distance = maxDistance;
                point = _direction.normalized * maxDistance + pos;
            }


            lineRenderer.SetPosition(j++, transform.position);
            lineRenderer.SetPosition(j++, point);


            //Debug.Log($"ray[{i}]={RayHits[i].distance}");

            //if (RayHits[i].distance >= maxDistance)
            //{

            //    Debug.Log($"distance max {RayHits[i].distance}"); 
            //}
        }
    }
    
    void OnDrawGizmos()
    {
        if (isDebugMode)
        {
            foreach (var item in RayHits.Zip(NormalizedData, (hit, normalized)=> new { hit,normalized}))
            {

                RaycastHit hit = item.hit;

                Vector3 _pos = transform.position + offset;
                Gizmos.DrawLine(hit.point, _pos);
                //Handles.Label(Vector3.Lerp(hit.point,_pos,.5f), item.normalized.ToString());
            }


        }
    }


}
