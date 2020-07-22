using NeatImpl;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Rigidbody))]
public class VehicleAgent : MonoBehaviour
{

    [Serializable]
    public class RaycastWheel
    {
        [HideInInspector]
        public float targetPosition;
        public float restLength;
        public float spring;
        public float damper;
        public float radius;

        [HideInInspector]
        public Quaternion localRotation;

        [HideInInspector]
        public float yRad = 0.0f;

        [HideInInspector]
        public bool isGrounded = false;

        [HideInInspector]
        public RaycastHit hit;

        [HideInInspector]
        public float visualRotationRad = 0.0f;

        private float prevLength = 0.0f;
        private float springLength = 0.0f;
        private float springForce = 0.0f;
        private float damperForce = 0.0f;
        private float springVelocity = 0.0f;
        
        private float prevCompression = 0.0f;
        private Vector3 suspensionForce = Vector3.zero;

        public float Compression { get; set; }

        public float MaxDistance
        {
            get
            {
                return restLength + radius;
            }
        }
        public float Distance
        {
            get
            {
                return springLength;
            }
        }

   

        public Quaternion LocalWheelRotation
        {
            get
            {
                return localRotation;
            }
            set
            {
                localRotation = value;
            }
        }

        public void ResetCompression(float dt) {
            prevCompression = Compression;
            Compression = Mathf.Clamp01(Compression - dt * 1.0f);
        }

        public Vector3 CalculateSuspensionForce(Vector3 down,float distance,Vector3 normal,float fixedDeltaTime)
        {
            prevLength = springLength;
            springLength = distance - radius;


            float suspForce = 0f;
            Compression = 1.0f - Mathf.Clamp01(springLength / restLength);

            springForce = Compression * -spring;
            suspForce += springForce;

            springVelocity = (Compression - prevCompression) / fixedDeltaTime;
            prevCompression = Compression;

            damperForce = -springVelocity * damper;
            suspForce += damperForce;

            suspForce *= Vector3.Dot(normal, -down);

            return down * suspForce;

        }

        public void Reset() {
             
        yRad = 0.0f;

        isGrounded = false;

        hit = new RaycastHit();

        visualRotationRad = 0.0f;

        prevLength = 0.0f;
        springLength = 0.0f;
        springForce = 0.0f;
        damperForce = 0.0f;
        springVelocity = 0.0f;
        prevCompression = 0.0f;
        suspensionForce = Vector3.zero;
    }



    }

    [Serializable]
    public class Axle
    {
        [Header("General")]
        public Vector3 offset;
        public float width;
        public float visualScale;
        public float antiRollForce = 100000.0f;




        [HideInInspector]
        public float wheelWidth = 0.085f;

        [Header("Debug")]
        [Tooltip("Color or debugging axle.")]
        public Color color;

        public RaycastWheel leftWheel;
        public RaycastWheel rightWheel;

        public GameObject leftVisualWheel;
        public GameObject rightVisualWheel;



        [HideInInspector]
        public GameObject[] visualWheels { private set; get; }

        

        public Vector3 Get_LeftPoint()
        {
            return new Vector3(offset.x - width * .5f, offset.y, offset.z);
        }
        public Vector3 Get_RightPoint()
        {
            return new Vector3(offset.x + width * .5f, offset.y, offset.z);
        }

    }

    [Header("General")]
    public AnimationCurve downForceCurve;
    public AnimationCurve accelerationCurve = AnimationCurve.EaseInOut(0.1f,50f,1f,200f);
    public Vector3 centerOfMass;
    public MeshRenderer body;
    [Tooltip("Max speed (Km/h)")]
    public float capSpeed = 200f;

    [Header("Steering")]
    public float maxSteeringAngle = 35f;
    public AnimationCurve steeringSpeedCurve;

    [Header("Axles")]
    public Axle frontAxle;
    public Axle rearAxle;

    [Header("Debug")]
    public bool isDebugMode = true;

    private Axle[] axles;
    private int wheelsCount;

    private Rigidbody rb;

    private float steeringInput;
    private float accelerationInput;
    private float brakeInput;





    Ray ray = new Ray();
    //RaycastHit[] wheelRayHits = new RaycastHit[16];

    #region AGENT
    private Vector3 prevPosition;
    private bool isVehicleDisabled = false;
    private RaySensor3D sensor;
    private float _currentSpeedKmH = 0.0f;

    ANN ann;
    

    #endregion

    private delegate void InputsHandler();


    InputsHandler inputsHandler;

    public void Setup(Player.Types type, Genome model = null) {

        if (type.Equals(Player.Types.AI)) {

            Assert.IsNotNull(model, "AI model cannot be null for this type of vehicle control.");

            ann = new ANN(model);
            inputsHandler = GetInputsFromANN;


            sensor = GetComponent<RaySensor3D>();
            sensor.AutoSetup(model.InputNodeNumber, 60f);

            sensor.SetDrawLines(true);

        }
        else {
            inputsHandler = GetInputsFromKB;
        }


    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        rb.centerOfMass = centerOfMass;

        axles = new Axle[2]{frontAxle,rearAxle};

        sensor = GetComponent<RaySensor3D>();

        //foreach (Axle a in axles)
        //    a.Setup();


        wheelsCount = axles.Length * 2;
    }

    //private void Start() {

    //    var player = GetComponent<Player>() as IPlayer;

    //    if (player.IsHuman()) {
    //        inputsHandler = GetInputsFromKB;
    //    }
    //    else {
    //        inputsHandler = GetInputsFromANN;
    //    }

    //}

    public void ResetAll() {

        steeringInput = 0f;
        accelerationInput = 0f;

        foreach(Axle a in axles) {

            foreach (var rw in new RaycastWheel[] { a.leftWheel,a.rightWheel}) {
                rw.Reset();

            }
        }

    }

    void FixedUpdate()
    {
        if (isVehicleDisabled)
            return;

        inputsHandler();

        CalculateCurrentSpeed();

        CalculateSteering();
        CalculateAxleForces();

    }

    void GetInputsFromANN() {
        float [] outputs = ann.FeedForward(sensor.GetNormalizedData());

        steeringInput = outputs[0];
        accelerationInput = outputs[1];

    }
    void GetInputsFromKB() {
        steeringInput = Input.GetAxis("Horizontal");
        accelerationInput = Input.GetAxis("Vertical");
    }

    // Update is called once per frame
    void Update()
    {
        if (isVehicleDisabled)
            return;

        ApplyVisuals();
    }
    public float GetCurrentSpeed() {
        return _currentSpeedKmH;
    }

    private void CalculateSteering()
    {


        float steeringSpeeed = steeringSpeedCurve.Evaluate(_currentSpeedKmH);


        float angleDeg = steeringSpeeed * steeringInput;

    

        float angleRad = Mathf.Min(Mathf.Abs(angleDeg), maxSteeringAngle) * Mathf.Sign(angleDeg) * Mathf.Deg2Rad;

        float axleSeprationLength = Vector3.Distance(frontAxle.offset, rearAxle.offset);

        float wheelSeparationLength = Vector3.Distance(frontAxle.Get_LeftPoint(), frontAxle.Get_RightPoint());



        float radius = axleSeprationLength * (1/Mathf.Tan(angleRad));

        frontAxle.leftWheel.yRad = Mathf.Atan(axleSeprationLength / (radius + wheelSeparationLength * .5f ));
        frontAxle.rightWheel.yRad = Mathf.Atan(axleSeprationLength / (radius - wheelSeparationLength * .5f ));



    }

    private float CalculateSpeed_MS() {

        Vector3 velocity = rb.velocity;

        Vector3 wsForward = rb.transform.rotation * Vector3.forward;

        float dot = Vector3.Dot(velocity, wsForward);
        Vector3 projVelo = dot * wsForward;
        float speed = velocity.magnitude * Mathf.Sign(dot);

        return speed;
    }
    private void CalculateCurrentSpeed() {
        _currentSpeedKmH = CalculateSpeed_KmH();
    }
    private float CalculateSpeed_KmH()
    {
        return CalculateSpeed_MS() * 3.6f;
    }
    private void AddDownForce(Vector3 wsDown)
    {

        float evaluateDownForce = downForceCurve.Evaluate(_currentSpeedKmH);

        Vector3 downForce = wsDown * rb.mass * evaluateDownForce;

        rb.AddForce(downForce);

        //Debug.Log($"DownForce:{d}");
    }
    private void ComputeWheelForce(Axle a,RaycastWheel w,Vector3 wsPos,Vector3 wsDown,float fixedDeltaTime) {

        w.LocalWheelRotation = Quaternion.Euler(new Vector3(0f, w.yRad * Mathf.Rad2Deg, 0f));
        Quaternion wsWheelRot = transform.rotation * w.LocalWheelRotation;

        Debug.DrawLine(wsPos, wsWheelRot * Vector3.right * 10f * Mathf.Sign(steeringInput) + wsPos, Color.yellow);

        //Debug.Log($"wr:{w.yRad}");

        Vector3 wsAxleLeft = wsWheelRot * Vector3.left;


        ray.direction = wsDown;

        ray.origin = wsPos + wsAxleLeft * a.wheelWidth;
        RaycastHit hit1 = new RaycastHit();
        bool b1 = Physics.Raycast(ray, out hit1, w.MaxDistance);


        ray.origin = wsPos - wsAxleLeft * a.wheelWidth;
        RaycastHit hit2 = new RaycastHit();
        bool b2 = Physics.Raycast(ray, out hit2, w.MaxDistance);


        bool b0 = Physics.Raycast(ray,out w.hit, w.MaxDistance);


        if (!b0 || !b1 || !b2) {
            w.ResetCompression(fixedDeltaTime);

            return;
        }

        w.isGrounded = true;

        Vector3 suspensionForce = w.CalculateSuspensionForce(wsDown, w.hit.distance, w.hit.normal, fixedDeltaTime);
        
        rb.AddForceAtPosition(suspensionForce, w.hit.point);

        Debug.DrawRay(wsPos, w.hit.distance * wsDown, Color.magenta);

        Vector3 contact_up = w.hit.normal;
        Vector3 contact_right = (hit1.point - hit2.point).normalized;
        Vector3 contact_forward = Vector3.Cross(contact_up, contact_right);

        Vector3 wheelVelocity = rb.GetPointVelocity(w.hit.point);

        Vector3 rvel = Vector3.Dot(wheelVelocity, contact_right) * contact_right;
        Vector3 fvel = Vector3.Dot(wheelVelocity, contact_forward) * contact_forward;
        Vector3 slideVelocity = (rvel + fvel) * 0.5f;


        Vector3 slidingForce = (slideVelocity * rb.mass / fixedDeltaTime) / wheelsCount;

        Vector3 frictionForce = -slidingForce;

        Vector3 longitudinalForce = Vector3.Dot(frictionForce, contact_forward) * contact_forward;

        frictionForce -= longitudinalForce;



        rb.AddForceAtPosition(frictionForce, w.hit.point);



        float currentSpeed = _currentSpeedKmH;
        bool brake = false;

        if (currentSpeed < 0f && accelerationInput < 0f)
            brake = true;


        float forceMagnitude = Mathf.Sign(accelerationInput) * accelerationCurve.Evaluate(Mathf.Abs(accelerationInput)); //CalcAccelForceMagnitude();

        if (!brake && currentSpeed < capSpeed && Mathf.Abs(forceMagnitude) > 0.01f) {
            
            Vector3 forcePoint = w.hit.point;
            Vector3 engineForce = contact_forward * forceMagnitude / wheelsCount / fixedDeltaTime;

            rb.AddForceAtPosition(engineForce, forcePoint);

            if (isDebugMode) {
                Debug.DrawRay(forcePoint, engineForce, Color.green);
            }
        }


    }
    private void CalculateAxleForces()
    {
        Vector3 wsDown = transform.TransformDirection(Vector3.down);
        wsDown.Normalize();

        float fixedDeltaTime = Time.fixedDeltaTime;

        foreach(Axle a in axles)
        {

            ComputeWheelForce(a, a.leftWheel,transform.TransformPoint(a.Get_LeftPoint()), wsDown, fixedDeltaTime);
            ComputeWheelForce(a, a.rightWheel,transform.TransformPoint(a.Get_RightPoint()), wsDown, fixedDeltaTime);

            float travelL = 1f - Mathf.Clamp01(a.leftWheel.Compression);
            float travelR = 1f - Mathf.Clamp01(a.rightWheel.Compression);

            float antiRollForce = (travelL - travelR) * a.antiRollForce;

            if (a.leftWheel.isGrounded) {
                Vector3 v = wsDown * antiRollForce;
                rb.AddForceAtPosition(v, a.leftWheel.hit.point);
    

                Debug.DrawRay(a.leftWheel.hit.point, v.normalized, Color.blue);
            }

            if (a.rightWheel.isGrounded) {
                Vector3 v = wsDown * -antiRollForce;
                rb.AddForceAtPosition(v, a.rightWheel.hit.point);

                Debug.DrawRay(a.rightWheel.hit.point, v.normalized, Color.blue);

   

            }


        }


        AddDownForce(wsDown);
    }
    private void ApplyVisuals()
    {
        foreach (Axle a in axles)
        {

            a.leftVisualWheel.transform.position = transform.TransformPoint(a.Get_LeftPoint()) - new Vector3(0f,a.leftWheel.Distance,0f);
  
            a.rightVisualWheel.transform.position = transform.TransformPoint(a.Get_RightPoint()) - new Vector3(0f,a.rightWheel.Distance,0f); 

            Quaternion localRot = Quaternion.Euler(new Vector3(a.leftWheel.visualRotationRad*Mathf.Rad2Deg,0f,0f) + a.leftWheel.LocalWheelRotation.eulerAngles);
            a.leftVisualWheel.transform.localRotation = localRot;
            localRot = Quaternion.Euler(new Vector3(a.rightWheel.visualRotationRad * Mathf.Rad2Deg, 0f, 0f) + a.rightWheel.LocalWheelRotation.eulerAngles);
            a.rightVisualWheel.transform.localRotation = localRot;


            a.leftVisualWheel.transform.localScale = a.rightVisualWheel.transform.localScale = new Vector3(a.visualScale, a.visualScale, a.visualScale);

            //RPM

            if (a.leftWheel.isGrounded)
            {
                CalculateWheelRotationFromSpeed(a.leftWheel, transform.TransformPoint(a.Get_LeftPoint()));

            }
            if (a.rightWheel.isGrounded)
            {
                CalculateWheelRotationFromSpeed(a.rightWheel, transform.TransformPoint(a.Get_RightPoint()));

            }



        }
    }
    private void CalculateWheelRotationFromSpeed(RaycastWheel w, Vector3 wsPos)
    {

        Quaternion wsWheelRot = transform.rotation * Quaternion.Euler(0f,w.yRad * Mathf.Rad2Deg,0f);
        Vector3 wsWheelForward = wsWheelRot * Vector3.forward;
        Vector3 velocityQueryPos = w.isGrounded ? w.hit.point : wsPos;
        Vector3 pointVelocity = rb.GetPointVelocity(velocityQueryPos);

        float longitudinalSpeed = Vector3.Dot( pointVelocity, wsWheelForward);

        float wheelLengthInMeters = 2 * Mathf.PI * w.radius;

        // revolutions per second
        float rps = longitudinalSpeed / wheelLengthInMeters;

        float deltaRot = Mathf.PI * 2f * rps * Time.deltaTime;


        w.visualRotationRad += deltaRot;

    }
    public void DisableVehicle() {
        rb.velocity = Vector3.zero;
        isVehicleDisabled = true;
    }
    public bool IsVehicleDisabled() {
        return isVehicleDisabled;
    }
   
}
