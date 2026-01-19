namespace Pa5455CmsDds.Component.Joystick
{
    public enum SUCCESSION_PROCESS
    {
        NONE = 0,
        CLAIM = 1,
        ACHIEVE = 2,
    }



    public class JoystickOwnership
    {
        public SUCCESSION_PROCESS progress = SUCCESSION_PROCESS.NONE;

        public DateTime? claimTime;
        public DateTime? achieveTime;



        public void claim(DateTime claimTime)
        {
            this.claimTime = claimTime;
            this.progress = SUCCESSION_PROCESS.CLAIM;
        }

        public void achieve(DateTime achieveTime)
        {
            this.achieveTime = achieveTime;
            this.progress = SUCCESSION_PROCESS.ACHIEVE;
        }

        public void drop()
        {
            this.progress = SUCCESSION_PROCESS.NONE;
            this.claimTime = null;
            this.achieveTime = null;
        }
    }
}