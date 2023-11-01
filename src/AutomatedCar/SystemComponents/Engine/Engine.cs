namespace AutomatedCar.SystemComponents.Engine
{
    using AutomatedCar.SystemComponents.Gearbox;
    using AutomatedCar.SystemComponents.InputHandling;

    /// <summary>
    /// The engine class.
    /// </summary>
    public class Engine : IEngine
    {
        private int revolution;
        private IGearBox gearbox;
        private IThrottle throttle;

        public int Revolution
        {
            get { return this.revolution; }
            set { this.revolution = value; }
        }

        public Engine(IGearBox gearbox, IThrottle throttle)
        {
            this.gearbox = gearbox;
            this.revolution = 1000;
            this.throttle = throttle;
        }

        public int CalculateRPM()
        {
            return throttle.GetThrottle() * 70;
        }

        public void Process()
        {
            Revolution = gearbox.CalculateGearSpeed(Revolution, CalculateRPM());
        }
    }
}
