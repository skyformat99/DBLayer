using System;

namespace DBLayer.Persistence
{
    public interface IGenerator
    {
        /// <summary>
        /// 生成唯一标识
        /// </summary>
        /// <returns></returns>
        object Generate();
    }

    /// <summary>
    /// 自定义 ID 生成器
    /// ID生成规则：ID 长达64 bits
    /// 
    /// |42 bits:Timestamp(毫秒)|3 bits:区域(机房)|10 bits:机器编号|10 bits:序列号|
    /// </summary>
    public class UUIDGenerator: IGenerator
    {
        
        //区域标识位数
        private readonly static int regionIdBits = 3;
        //机器标识位数
        private readonly static int workerIdBits = 10;
        //序列号标识位数
        private readonly static int sequenceBits = 10;
        
        //区域标志ID最大值
        private readonly static int maxRegionId = -1 ^ (-1 << regionIdBits);
        // 机器ID最大值
        private readonly static int maxWorkerId = -1 ^ (-1 << workerIdBits);
        // 序列号ID最大值
        private readonly static int sequenceMask = -1 ^ (-1 << sequenceBits);

        // 机器ID偏左移10位
        private readonly static int workerIdShift = sequenceBits;
        // 业务ID偏左移20位
        private readonly static int regionIdShift = sequenceBits + workerIdBits;
        // 时间毫秒左移23位
        private readonly static int timestampLeftShift = sequenceBits + workerIdBits + regionIdBits;


        private static long lastTimestamp = -1L;

        private int sequence = 0;
        private readonly int workerId;
        private readonly int regionId;
        //基准时间
        private readonly long twepoch;

        public UUIDGenerator(int workerId)
        {
            // 如果超出范围就抛出异常
            if (workerId > maxWorkerId || workerId < 0)
            {
                throw new ArgumentException("worker Id can't be greater than %d or less than 0");
            }
            this.workerId = workerId;
            this.regionId = 0;
            this.twepoch = 1288834974657L;//Thu, 04 Nov 2010 01:42:54 GMT
        }
        public UUIDGenerator(int workerId, int regionId)
        {

            // 如果超出范围就抛出异常
            if (workerId > maxWorkerId || workerId < 0)
            {
                throw new ArgumentException("worker Id can't be greater than %d or less than 0");
            }
            if (regionId > maxRegionId || regionId < 0)
            {
                throw new ArgumentException("datacenter Id can't be greater than %d or less than 0");
            }

            this.workerId = workerId;
            this.regionId = regionId;
            this.twepoch = 1288834974657L;//Thu, 04 Nov 2010 01:42:54 GMT
        }
        public UUIDGenerator(int workerId, int regionId, long twepoch)
        {

            // 如果超出范围就抛出异常
            if (workerId > maxWorkerId || workerId < 0)
            {
                throw new ArgumentException("worker Id can't be greater than %d or less than 0");
            }
            if (regionId > maxRegionId || regionId < 0)
            {
                throw new ArgumentException("datacenter Id can't be greater than %d or less than 0");
            }

            this.workerId = workerId;
            this.regionId = regionId;
            this.twepoch = twepoch;
        }
        public object Generate()
        {
            return this.nextId(false, 0);
        }
        /// <summary>
        /// 实际产生代码的
        /// </summary>
        /// <param name="isPadding"></param>
        /// <param name="busId"></param>
        /// <returns></returns>
        private long nextId(bool isPadding, long busId)
        {

            long timestamp = timeGen();
            long paddingnum = regionId;

            if (isPadding)
            {
                paddingnum = busId;
            }

            if (timestamp < lastTimestamp)
            {
                try
                {
                    throw new Exception("Clock moved backwards.  Refusing to generate id for " + (lastTimestamp - timestamp) + " milliseconds");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }

            //如果上次生成时间和当前时间相同,在同一毫秒内
            if (lastTimestamp == timestamp)
            {
                //sequence自增，因为sequence只有10bit，所以和sequenceMask相与一下，去掉高位
                sequence = (sequence + 1) & sequenceMask;
                //判断是否溢出,也就是每毫秒内超过1024，当为1024时，与sequenceMask相与，sequence就等于0
                if (sequence == 0)
                {
                    //自旋等待到下一毫秒
                    timestamp = tailNextMillis(lastTimestamp);
                }
            }
            else
            {
                // 如果和上次生成时间不同,重置sequence，就是下一毫秒开始，sequence计数重新从0开始累加,
                // 为了保证尾数随机性更大一些,最后一位设置一个随机数

                sequence = new Random().Next(10);
            }

            lastTimestamp = timestamp;
            #pragma warning disable CS0675
            return ((timestamp - twepoch) << timestampLeftShift) | (paddingnum << regionIdShift) | (workerId << workerIdShift) | sequence;
            
        }

        /// <summary>
        ///  防止产生的时间比之前的时间还要小（由于NTP回拨等问题）,保持增量的趋势.
        /// </summary>
        /// <param name="lastTimestamp"></param>
        /// <returns></returns>
        private long tailNextMillis(long lastTimestamp)
        {
            long timestamp = this.timeGen();
            while (timestamp <= lastTimestamp)
            {
                timestamp = this.timeGen();
            }
            return timestamp;
        }

        /// <summary>
        /// 获取当前的时间戳
        /// </summary>
        /// <returns></returns>
        protected long timeGen()
        {
            var currentTimeMillis = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
            return currentTimeMillis;
        }

    }
    public class DUIDGenerator : IGenerator
    {
        public object Generate()
        {
            var buffer = Guid.NewGuid().ToByteArray();
            var longGuid = BitConverter.ToInt64(buffer, 0);

            var value = Math.Abs(longGuid).ToString();

            var buf = new byte[value.Length];
            var p = 0;
            for (var i = 0; i < value.Length; )
            {
                var ph = Convert.ToByte(value[i]);

                var fix = 1;
                if ((i + 1) < value.Length)
                {
                    var pl = Convert.ToByte(value[i + 1]);
                    buf[p] = (byte)((ph << 4) + pl);
                    fix = 2;
                }
                else
                {
                    buf[p] = (ph);
                }

                if ((i + 3) < value.Length)
                {
                    if (Convert.ToInt16(value.Substring(i, 3)) < 256)
                    {
                        buf[p] = Convert.ToByte(value.Substring(i, 3));
                        fix = 3;
                    }
                }
                p++;
                i = i + fix;
            }
            var buf2 = new byte[p];
            for (var i = 0; i < p; i++)
            {
                buf2[i] = buf[i];
            }
            string guid2Int = BitConverter.ToInt32(buf2, 0).ToString().Replace("-", "").Replace("+", "");
            guid2Int = guid2Int.Length >= 9 ? guid2Int.Substring(0, 9) : guid2Int.PadLeft(9, '0');
            return Convert.ToInt64(DateTime.Now.ToString("yyMMddHHmm") + guid2Int);
        }
    }
    public class GUIDGenerator : IGenerator
    {
        public object Generate()
        {
            var uid = Guid.NewGuid().ToByteArray();
            var binDate = BitConverter.GetBytes(DateTime.UtcNow.Ticks);

            var secuentialGuid = new byte[uid.Length];

            secuentialGuid[0] = uid[0];
            secuentialGuid[1] = uid[1];
            secuentialGuid[2] = uid[2];
            secuentialGuid[3] = uid[3];
            secuentialGuid[4] = uid[4];
            secuentialGuid[5] = uid[5];
            secuentialGuid[6] = uid[6];
            // set the first part of the 8th byte to '1100' so     
            // later we'll be able to validate it was generated by us   

            secuentialGuid[7] = (byte)(0xc0 | (0xf & uid[7]));

            // the last 8 bytes are sequential,    
            // it minimizes index fragmentation   
            // to a degree as long as there are not a large    
            // number of Secuential-Guids generated per millisecond  

            secuentialGuid[9] = binDate[0];
            secuentialGuid[8] = binDate[1];
            secuentialGuid[15] = binDate[2];
            secuentialGuid[14] = binDate[3];
            secuentialGuid[13] = binDate[4];
            secuentialGuid[12] = binDate[5];
            secuentialGuid[11] = binDate[6];
            secuentialGuid[10] = binDate[7];

            return new Guid(secuentialGuid);
        }
    }

}
