public interface IWaypointFan
{
    public delegate void StartedDelegate(IWaypointFan fan);
    public static StartedDelegate OnStarted;

    public delegate void StoppedDelegate(IWaypointFan fan);
    public static StoppedDelegate OnStopped;
}