using UnityEngine;
using Zenject;

public class DefaultInstaller : MonoInstaller
{
    [SerializeField] private Player prefab;
    [SerializeField] private ObstaclesSelector obstacleSelector;


    public override void InstallBindings()
    {
        var gameCtrl = GetComponent<GameController>();

        Container.Bind<IGameController>().To<GameController>().FromInstance(gameCtrl);

        Container.BindFactory<Player, Player.Factory>().FromComponentInNewPrefab(prefab);

        Container.Bind<IObstacleSelector>().To<ObstaclesSelector>().FromInstance(obstacleSelector);

    }
}