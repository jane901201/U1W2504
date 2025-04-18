namespace TimerFrame
{
    public interface ITimerTask
    {
        string Name { get; }
        void Execute();
    }
}