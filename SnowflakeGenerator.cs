namespace Twitter
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
                do
                {
                    var timestamp = GetTimestamp(year);
                    if (timestamp >= 2199023255552L)
                    {
                        throw new InvalidOperationException("Timestamp overflow");
                    }
                    if (_timestamp > timestamp)
                    {
                        throw new InvalidOperationException("Abnormal system clock");
                    }
                    var sequence = GetSequence(timestamp);
                    if (sequence >= 4096)
                    {
                        Thread.Sleep(1);
                        continue;
                    }
                    var p1 = timestamp << 22;
                    var p2 = mac << 12;
                    var p3 = sequence;
                    return p1 | p2 | p3;
                } while (true);
            }
        }
    }
}
