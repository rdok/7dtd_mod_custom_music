namespace CustomMusic.Harmony
{
    public class Logger : ILogger
    {
        public void Info(string message)
        {
#if DEBUG
            Log.Out("[CustomMusic]: " + message);
#endif
        }

        public void Warn(string message)
        {
#if DEBUG
            Log.Warning("[CustomMusic]: " + message);
#endif
        }

        public void Error(string message)
        {
#if DEBUG
            Log.Error("[CustomMusic]: " + message);
#endif
        }
    }

    public interface ILogger
    {
        void Info(string message);
        void Warn(string message);
        void Error(string message);
    }
}