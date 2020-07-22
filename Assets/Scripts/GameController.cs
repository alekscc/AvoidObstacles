using NeatImpl;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using Zenject;

public class GameController : MonoBehaviour, IGameController{



    [Header("Player 1")]
    [SerializeField] private Player.Types player1Type;
    [SerializeField] private UIPlayerPanel player1Panel;

    [Header("Player 2")]
    [SerializeField] private Player.Types player2Type;
    [SerializeField] private UIPlayerPanel player2Panel;

    [Header("Track")]
    [SerializeField] private int numberOfObstacles = 5;

    [Header("Other")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private KeyCode resetKey = KeyCode.Space;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private int numberOfLaps = 3;
    [SerializeField] private string echo;
    [SerializeField] private string aiModelPathname;

    private Genome _aiModel;
    private Checkpoint[] _checkpoints;
    private int _playersLeft = 0;
    private List<IPlayer> _listPlayers = new List<IPlayer>();

    [Inject]
    private Player.Factory _playerFactory;

    [Inject]
    private IObstacleSelector _obstacleSelector;

   void Awake() {

        _checkpoints = FindObjectsOfType<Checkpoint>();
        _aiModel = LoadAIModel(aiModelPathname);





        ResetGame();
    }

    private Genome LoadAIModel(string pathname) {

        var modelData = Utils.DataOps.XmlLoad<GenomeData>("trainedmodels\\" + pathname);
        Assert.IsNotNull(modelData, "Couldnt load AI model");

        return new Genome(modelData);

    }
    void Update() {

        if (Input.GetKeyUp(resetKey))
            ResetGame();

    }
    private void ResetGame() {

        foreach (var item in new UIPlayerPanel[] { player1Panel, player2Panel })
            item.Setup(_checkpoints.Length, numberOfLaps);

        foreach(var p in _listPlayers) {
            p.SelfDestroy();
        }

        Spawn2Players();

        _obstacleSelector.SelectRandom(numberOfObstacles);
    }
    public void Spawn2Players() {

        _listPlayers = new List<IPlayer>();

        _listPlayers.Add(SpawnPlayer(spawnPoint.position, spawnPoint.rotation, player1Type, player1Panel));
        _listPlayers.Add(SpawnPlayer(spawnPoint.position, spawnPoint.rotation, player2Type, player2Panel));

        _playersLeft = 2;

    }
    public void Signal_Crash(IPlayer player) {

        Player.Types type = Player.Types.AI;

        if (player.IsHuman()) {

            foreach (var item in _checkpoints)
                item.ResetCheck();

            type = Player.Types.Human;
        }
        else {
            Debug.Log("Not human crashed");
        }

        player.Stop();

        //caller.SelfDestroy();


        if(--_playersLeft == 0) {

            ResetGame();
        }


        //SpawnPlayer(spawnPoint.position, spawnPoint.rotation,type);

    }
    private void SummaryGame() {

    }
    private IPlayer SpawnPlayer(Vector3 position,Quaternion rotation,Player.Types type,UIPlayerPanel panel) {
        var player = _playerFactory.Create() as IPlayer;
        player.Reset(position, rotation,type,panel,_aiModel);


        return player;
    }
    public void Signal_LapAchieved(IPlayer player) {
        if(player.GetLapNumber() >= numberOfLaps) {
            player.Stop();
        }

        foreach (var item in _checkpoints) {
            item.ResetCheck();
        }
    }
    public int GetNumberOfLaps() {
        return numberOfLaps;
    }
    public int GetNumberOfCheckpoints() {
        return _checkpoints.Count(x=> !x.IsMeta());
    }

}
