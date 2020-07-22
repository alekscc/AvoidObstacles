using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class UIPlayerPanel : MonoBehaviour
{
    enum PlayerNumber {
        Player1 = 1,
        Player2 = 2
    }

    [SerializeField] private PlayerNumber player;
    [SerializeField] private Text textCheckpointsNumber;
    [SerializeField] private Text textLapNumber;
    [SerializeField] private Text textTotalTime;

    private int _maxLaps = 0,
                _maxCheckpoints = 0;



    //[SerializeField] private Text textLapRecordTime;

    public void Setup(int maxLaps,int maxCheckpoints) {

        _maxLaps = maxLaps;
        _maxCheckpoints = maxCheckpoints;
    }


    public void UpdateOutcome(string strCheckpoint, string strLap, string strTime/*,string strLapTimeRecord*/) {

        textCheckpointsNumber.text = strCheckpoint + " / " + _maxLaps;
        textLapNumber.text = strLap + " / " + _maxCheckpoints;
        textTotalTime.text = strTime;
        //textLapRecordTime.text = strLapTimeRecord;

    }


}
