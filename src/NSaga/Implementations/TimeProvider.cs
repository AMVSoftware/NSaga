using System;

namespace NSaga.Implementations
{
    /// <summary>
    /// Ambient context for providing current DateTime. 
    /// </summary>
    public abstract class TimeProvider
    {
        private static TimeProvider current;

        public abstract DateTime UtcNow { get; }
        public abstract DateTime Today { get; }

        static TimeProvider()
        {
            current = new DefaultTimeProvider();
        }

        public static TimeProvider Current
        {
            get
            {
                return current;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }
                current = value;
            }
        }
        public static void ResetToDefault()
        {
            current = new DefaultTimeProvider();
        }
    }



    public class DefaultTimeProvider : TimeProvider
    {
        public override DateTime UtcNow => DateTime.UtcNow;

        public override DateTime Today => DateTime.Today;
    }



    public class StubTimeProvider : TimeProvider
    {
        private DateTime currentTime;


        public StubTimeProvider(DateTime currentTime)
        {
            this.currentTime = currentTime;
        }

        public override DateTime UtcNow => currentTime;

        public override DateTime Today => currentTime.Date;
    }
}
