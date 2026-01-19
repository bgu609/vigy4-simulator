namespace Pa5455CmsDds.Component.Joystick
{
    public class JoystickIdentity
    {
        internal JoystickOwnership joystickOwnership { get; private set; } = new JoystickOwnership();

        internal E_EQUIP_ID? mfcId;
        internal E_DEV_ID? deviceId;
        internal E_SW_ID? swId;



        internal JoystickIdentity(E_EQUIP_ID? mfcId = null, E_DEV_ID? deviceId = null, E_SW_ID? swId = null)
        {
            this.mfcId = mfcId;
            this.deviceId = deviceId;
            this.swId = swId;
        }



        public bool isIdentified()
        {
            return ((this.mfcId != null) && (this.deviceId != null) && (this.swId != null));
        }
    }
}