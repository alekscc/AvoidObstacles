using NeatImpl;

internal interface IGameController {

    void Signal_Crash(IPlayer player);
    void Signal_LapAchieved(IPlayer player);
    int GetNumberOfLaps();
    int GetNumberOfCheckpoints();


}