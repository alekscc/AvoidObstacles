using NeatImpl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;
using Zenject;


[RequireComponent(typeof(VehicleAgent))]
public class Player : MonoBehaviour, IPlayer{


    public class Factory : PlaceholderFactory<Player> { }

    public enum Types {
        AI,
        Human
    };

    [SerializeField] private Types type = Types.AI;

    private int _currentCheckpoint = 0;
    private int _lapNumber = 0;
    private int _timeRecordInSeconds = 0;


    private Timer _timer;
    private UIPlayerPanel _panel;
    private VehicleAgent _vehicleAgent;

    [Inject]
    private IGameController _gameController;




    private void Awake() {
        _timer = new Timer(() => Time.time);

        _vehicleAgent = GetComponent<VehicleAgent>();
    }


    public void SetCheckpoint(int checkpointNumber) {
        _currentCheckpoint = checkpointNumber;
    }

    public int GetCheckpoint() {
        return _currentCheckpoint;
    }

    public bool IsHuman() {
        return type.Equals(Types.Human) ? true : false;
    }

    void OnCollisionEnter(Collision collision) {
        if (!_vehicleAgent.IsVehicleDisabled()) {
            _gameController.Signal_Crash(this); 
        }
    }

    IEnumerator UpdatePanelRoutine() {

        while (_panel!=null) {

                
            _panel.UpdateOutcome(_currentCheckpoint.ToString(),
                               _lapNumber.ToString(),
                               _timer.To_hhmmss());

            yield return new WaitForSeconds(.5f);
        }
    }

    public void Reset(Vector3 position, Quaternion rotation,Types type,UIPlayerPanel panel,Genome aiModel = null) {

        this.type = type;

        _vehicleAgent.ResetAll();

        _vehicleAgent.Setup(type, aiModel);

        transform.position = position;
        transform.rotation = rotation;
        _panel = panel;
        

        StartCoroutine(UpdatePanelRoutine());

        _timer.Start();

    }
    public void SelfDestroy() {

        StopAllCoroutines();

        Destroy(gameObject);
    }

    public void AddLap() {


        _currentCheckpoint = 0;
        ++_lapNumber;

        _gameController.Signal_LapAchieved(this);

    }

    public int GetLapNumber() {
        return _lapNumber;
    }

    public void Stop() {

        Debug.Log($"STOP ishuman:{IsHuman()}");
        _timer.Stop();
        _vehicleAgent.DisableVehicle();
    }

}
