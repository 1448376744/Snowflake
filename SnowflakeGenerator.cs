namespace ConsoleApp1
{
    /// <summary>
    /// 雪花生成器
    /// </summary>
    public static class SnowflakeGenerator
    {
        private static int _counter = 0;
        private static long _timestamp = 0;
        private static object _locker = new object();

        private static long GetTimestamp(int year)
        {
            var timestamp = (long)(DateTime.Now - new DateTime(year, 1, 1)).TotalMilliseconds;
            if (timestamp >= 2199023255552L)
            {
                throw new InvalidOperationException("Timestamp overflow");
            }
            return timestamp;
        }

        private static long GetSequence(long timestamp)
        {
            if (_timestamp == timestamp)
            {
                _counter++;
            }
            else
            {
                _counter = 0;
            }
            _timestamp = timestamp;
            if (_counter >= 4096)
            {
                throw new InvalidOperationException("Sequence overflow");
            }
            return _counter;
        }
        public static long Generate(long mac, int year = 1970)
        {
            if (mac >= 1024)
            {
                throw new InvalidOperationException("Mac overflow");
            }
            lock (_locker)
            {
                var timestamp = GetTimestamp(year);
                var sequence = GetSequence(timestamp);
                var p1 = timestamp << 22;
                var p2 = mac << 12;
                var p3 = sequence;
                return p1 | p2 | p3;
            }
        }
    }

    /// <summary>
    /// 雪花
    /// </summary>
    public class Snowflake
    {

    }
}
