using NeatImpl;
using UnityEngine;



public interface IPlayer {

    void SetCheckpoint(int checkpointNumber);
    int GetCheckpoint();
    void AddLap();
    int GetLapNumber();
    bool IsHuman();
    void Reset(Vector3 position,Quaternion rotation,Player.Types type,UIPlayerPanel panel,Genome aiModel);
    void Stop();
    void SelfDestroy();

}