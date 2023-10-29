namespace AutomatedCar.SystemComponents.Engine
{
    /*!!! Merge conflict BEGIN --> these are defined in other branches, MUST remove these and using implemented classes !!!*/
    public interface IGearBox
    {
       int CalculateGearSpeed(int revolution, int enginespeed);
    }

    public interface IThrottle
    {
        int GetThrottle();
    }

    /*!!! Merge conflict END !!!*/

    /// <summary>
    /// The engine class.
    /// </summary>
    public class Engine : SystemComponent, IEngine
    {
        private int revolution;
        private IGearBox gearbox;
        private IThrottle throttle;
        public int Revolution
        {
            get { return this.revolution; }
            set { this.revolution = value; }
        }

        public Engine(IGearBox gearbox, IThrottle throttle, VirtualFunctionBus virtualFunctionBus)
            : base(virtualFunctionBus)
        {
            this.gearbox = gearbox;
            this.revolution = 1000;
            this.throttle = throttle;
        }

        public int CalculateRPM()
        {
            return throttle.GetThrottle() * 70;
        }

        public override void Process()
        {
            Revolution = gearbox.CalculateGearSpeed(Revolution, CalculateRPM());
        }
    }
}
