namespace Conreign.LoadTest.Supervisor.Utility
{
    public class ProgressStreamEventArgs
    {
        public ProgressStreamEventArgs(int writtenBytes, int readBytes)
        {
            WrittenBytes = writtenBytes;
            ReadBytes = readBytes;
        }

        public int WrittenBytes { get; }
        public int ReadBytes { get; }
    }
}