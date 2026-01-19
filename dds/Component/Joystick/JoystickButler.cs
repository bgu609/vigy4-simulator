using Microsoft.Win32.SafeHandles;
using Rti.Dds.Subscription;

namespace Pa5455CmsDds.Component.Joystick
{
    public class JoystickButler
    {
        public delegate void IdentityDelegator(E_EQUIP_ID mfcId, E_SW_ID swId);
        public delegate void BufferDelegator(byte[] buffer, int readLength);

        public event IdentityDelegator? notifyJoysticOwner;
        public event IdentityDelegator? notifyDroppedJoysticOwner;
        public event BufferDelegator? onReceiveJoystickData;


        private DdsParticipant joystickDdsParticipant;
        private Thread? joystickDdsServiceThread;
        private Thread? joystickStreamThread;


        public JoystickIdentity userIdentity { get; private set; } = new JoystickIdentity();

        public E_EQUIP_ID? joystickOwnerMfcId { get; private set; } = null;
        public E_SW_ID? joystickOwnerSwId { get; private set; } = null;

        private ushort hidVendorId;
        private ushort hidProductId;
        private Guid hidGuid;
        private IntPtr hidHandler;


        private ST_MSG_HEADER joystickOwnershipStatusHeader = new ST_MSG_HEADER() {
            eRedundancy = E_REDUNDANCY.E_REDUNDANCY_NONE,
        };
        private ST_MSG_HEADER joystickOwnershipCommandHeader = new ST_MSG_HEADER() {
            eRedundancy = E_REDUNDANCY.E_REDUNDANCY_NONE,
        };
        private ST_MSG_HEADER joystickOwnershipDropCommandHeader = new ST_MSG_HEADER() {
            eRedundancy = E_REDUNDANCY.E_REDUNDANCY_NONE,
        };


        private Timer joystickOwnershipWatchdog;
        private Timer joystickClaimWatchdog;

        public int joystickDdsServiceCoolTime { get; private set; } = 250;
        public int joystickClaimWaitingTime { get; private set; } = 1000;
        public int joystickAchieveLifeTime { get; private set; } = 2000;


        public bool onJoystickDdsService { get; private set; } = false;



        public JoystickButler(ushort vendorId = 0xFDFF, ushort productId = 0x5710, int ddsDomain = 30)
        {
            this.joystickOwnershipWatchdog = new Timer(state => {
                lock (this.userIdentity)
                {
                    if ((this.joystickOwnerMfcId != null) && (this.joystickOwnerSwId != null))
                    {
                        this.notifyDroppedJoysticOwner?.Invoke(this.joystickOwnerMfcId.Value, this.joystickOwnerSwId.Value);

                        this.joystickOwnerMfcId = null;
                        this.joystickOwnerSwId = null;
                    }
                }
            });
            this.joystickClaimWatchdog = new Timer(state => achieveJoystickOwnership());

            this.joystickDdsParticipant = new DdsParticipant(ddsDomain);


            this.hidVendorId = vendorId;
            this.hidProductId = productId;

            try
            {
                WindowsApi.HidD_GetHidGuid(out this.hidGuid);

                foreach (WindowsApi.DeviceInterface deviceInterface in WindowsApi.getEnumerateHidInterfaces(this.hidGuid))
                {
                    WindowsApi.DeviceAttibute deviceAttribute = WindowsApi.getDeviceAttribute(deviceInterface.path);

                    if ((deviceAttribute.vid == this.hidVendorId) && (deviceAttribute.pid == this.hidProductId))
                    {
                        this.hidHandler = WindowsApi.tryOpen(deviceInterface.path);
                        break;
                    }
                }

                if (this.hidHandler > IntPtr.Zero)
                {
                    this.joystickStreamThread = new Thread(() => {
                        using (FileStream joystickStream = new FileStream(new SafeFileHandle(this.hidHandler, ownsHandle: true), FileAccess.ReadWrite))
                        {
                            byte[] joystickDataBuffer = new byte[5];

                            while (true)
                            {
                                lock (this.userIdentity)
                                {
                                    if (this.userIdentity.joystickOwnership.progress == SUCCESSION_PROCESS.ACHIEVE)
                                    {
                                        int read = joystickStream.Read(joystickDataBuffer, 0, joystickDataBuffer.Length);

                                        if (read > 0)
                                        {
                                            this.onReceiveJoystickData?.Invoke(joystickDataBuffer, read);
                                        }
                                    }
                                }
                            }
                        }
                    }) { IsBackground = true };
                    this.joystickStreamThread.Start();
                }
            }
            catch (Exception e)
            {
                Exception debig = e;
            }
        }



        public void startDdsService()
        {
            this.joystickDdsParticipant.publishTopic<JOYSTICK_OWNERSHIP_STATUS>();
            this.joystickDdsParticipant.publishTopic<JOYSTICK_OWNERSHIP_COMMAND>();
            this.joystickDdsParticipant.publishTopic<JOYSTICK_OWNERSHIP_DROP_COMMAND>();

            this.joystickDdsParticipant.subscribeTopic<JOYSTICK_OWNERSHIP_STATUS>(readJoystickOwnershipStatus);
            this.joystickDdsParticipant.subscribeTopic<JOYSTICK_OWNERSHIP_COMMAND>(readJoystickOwnershipCommand);
            this.joystickDdsParticipant.subscribeTopic<JOYSTICK_OWNERSHIP_DROP_COMMAND>(readJoystickOwnershipDropCommand);

            if (this.joystickDdsServiceThread == null)
            {
                this.onJoystickDdsService = true;

                this.joystickDdsServiceThread = new Thread(() => {
                    while (this.onJoystickDdsService)
                    {
                        Thread.Sleep(this.joystickDdsServiceCoolTime);

                        lock (this.userIdentity)
                        {
                            switch (this.userIdentity.joystickOwnership.progress)
                            {
                                case SUCCESSION_PROCESS.CLAIM: { writeJoystickOwnershipCommand(); } break;
                                case SUCCESSION_PROCESS.ACHIEVE: { writeJoystickOwnershipStatus(); } break;
                            }
                        }
                    }
                }) { IsBackground = true };
                this.joystickDdsServiceThread.Start();
            }
        }

        public void stopDdsService()
        {
            this.onJoystickDdsService = false;

            dropJoystickOwnership();
            removeUserIdentity();

            this.joystickDdsParticipant.clearAll();

            this.joystickDdsServiceThread?.Join();
            this.joystickDdsServiceThread = null;
        }

        public void setUserIdentity(E_EQUIP_ID mfcId, E_DEV_ID deviceId, E_SW_ID swId)
        {
            lock (this.userIdentity)
            {
                this.userIdentity.mfcId = mfcId;
                this.userIdentity.deviceId = deviceId;
                this.userIdentity.swId = swId;
            }
        }

        public void removeUserIdentity()
        {
            lock (this.userIdentity)
            {
                this.userIdentity.mfcId = null;
                this.userIdentity.deviceId = null;
                this.userIdentity.swId = null;
            }
        }

        public bool allocJoystick()
        {
            if (this.userIdentity.isIdentified())
            {
                claimJoystickOwnership();

                return true;
            }
            else
            {
                return false;
            }
        }

        public void deallocJoystick()
        {
            dropJoystickOwnership();
        }


        private void claimJoystickOwnership()
        {
            lock (this.userIdentity)
            {
                if (this.userIdentity.isIdentified() && (this.userIdentity.joystickOwnership.progress == SUCCESSION_PROCESS.NONE))
                {
                    this.userIdentity.joystickOwnership.claim(DateTime.Now);

                    this.joystickClaimWatchdog.Change(this.joystickClaimWaitingTime, Timeout.Infinite);
                }
            }
        }
        
        private void achieveJoystickOwnership()
        {
            lock (this.userIdentity)
            {
                if (this.userIdentity.isIdentified() && (this.userIdentity.joystickOwnership.progress == SUCCESSION_PROCESS.CLAIM))
                {
                    this.userIdentity.joystickOwnership.achieve(DateTime.Now);
                }
            }
        }

        private void dropJoystickOwnership()
        {
            lock (this.userIdentity)
            {
                if (this.userIdentity.joystickOwnership.progress != SUCCESSION_PROCESS.NONE)
                {
                    writeJoystickOwnershipDropCommand();
                }

                this.userIdentity.joystickOwnership.drop();
            }
        }


        #region [[ DDS ]]
        private void writeJoystickOwnershipStatus()
        {
            lock (this.userIdentity)
            {
                if (this.userIdentity.isIdentified())
                {
                    this.joystickOwnershipStatusHeader.eSendEquipID = this.userIdentity.mfcId!.Value;
                    this.joystickOwnershipStatusHeader.eSendDevID = this.userIdentity.deviceId!.Value;
                    this.joystickOwnershipStatusHeader.eSendSW = this.userIdentity.swId!.Value;

                    DDS_UTIL.inputTopicHeaderTime(this.joystickOwnershipStatusHeader, DateTime.Now);

                    this.joystickDdsParticipant.writeTopic(new JOYSTICK_OWNERSHIP_STATUS() {
                        stMsgHeader = this.joystickOwnershipStatusHeader,
                        eOwnerMfc = this.joystickOwnershipStatusHeader.eSendEquipID,
                        eOwnerSW = this.joystickOwnershipStatusHeader.eSendSW,
                    });
                }
            }
        }

        private void writeJoystickOwnershipCommand()
        {
            lock (this.userIdentity)
            {
                if (this.userIdentity.isIdentified())
                {
                    this.joystickOwnershipCommandHeader.eSendEquipID = this.userIdentity.mfcId!.Value;
                    this.joystickOwnershipCommandHeader.eSendDevID = this.userIdentity.deviceId!.Value;
                    this.joystickOwnershipCommandHeader.eSendSW = this.userIdentity.swId!.Value;

                    DDS_UTIL.inputTopicHeaderTime(this.joystickOwnershipCommandHeader, DateTime.Now);

                    this.joystickDdsParticipant.writeTopic(new JOYSTICK_OWNERSHIP_COMMAND() {
                        stMsgHeader = this.joystickOwnershipCommandHeader,
                        eHeirMfc = this.joystickOwnershipCommandHeader.eSendEquipID,
                        eHeirSW = this.joystickOwnershipCommandHeader.eSendSW,
                    });
                }
            }
        }

        private void writeJoystickOwnershipDropCommand()
        {
            lock (this.userIdentity)
            {
                if (this.userIdentity.isIdentified())
                {
                    this.joystickOwnershipDropCommandHeader.eSendEquipID = this.userIdentity.mfcId!.Value;
                    this.joystickOwnershipDropCommandHeader.eSendDevID = this.userIdentity.deviceId!.Value;
                    this.joystickOwnershipDropCommandHeader.eSendSW = this.userIdentity.swId!.Value;

                    DDS_UTIL.inputTopicHeaderTime(this.joystickOwnershipDropCommandHeader, DateTime.Now);

                    this.joystickDdsParticipant.writeTopic(new JOYSTICK_OWNERSHIP_DROP_COMMAND() {
                        stMsgHeader = this.joystickOwnershipDropCommandHeader,
                        eDropMfc = this.joystickOwnershipDropCommandHeader.eSendEquipID,
                        eDropSW = this.joystickOwnershipDropCommandHeader.eSendSW,
                    });
                }
            }
        }


        private void readJoystickOwnershipStatus(LoanedSample<JOYSTICK_OWNERSHIP_STATUS> topicPayload)
        {
            if (topicPayload.Info.ValidData)
            {
                this.joystickOwnershipWatchdog.Change(Timeout.Infinite, Timeout.Infinite);

                JOYSTICK_OWNERSHIP_STATUS topic = topicPayload.Data;

                lock (this.userIdentity)
                {
                    this.joystickClaimWatchdog.Change(Timeout.Infinite, Timeout.Infinite);

                    if (this.userIdentity.isIdentified())
                    {
                        if (this.userIdentity.joystickOwnership.progress == SUCCESSION_PROCESS.CLAIM)
                        {
                            switch (topic)
                            {
                                case var _ when ((topic.eOwnerMfc == this.userIdentity.mfcId) && (topic.eOwnerSW != this.userIdentity.swId)):
                                case var _ when ((topic.eOwnerMfc != this.userIdentity.mfcId) && (topic.eOwnerSW == this.userIdentity.swId)):
                                    {
                                        this.joystickClaimWatchdog.Change(this.joystickClaimWaitingTime, Timeout.Infinite);
                                    }
                                    break;
                            }
                        }

                        if (topic.eOwnerSW == this.userIdentity.swId)
                        {
                            this.joystickOwnerMfcId = topic.eOwnerMfc;
                            this.joystickOwnerSwId = topic.eOwnerSW;

                            this.notifyJoysticOwner?.Invoke(topic.eOwnerMfc, topic.eOwnerSW);
                        }
                    }
                }

                this.joystickOwnershipWatchdog.Change(this.joystickAchieveLifeTime, Timeout.Infinite);
            }
        }

        private void readJoystickOwnershipCommand(LoanedSample<JOYSTICK_OWNERSHIP_COMMAND> topicPayload)
        {
            if (topicPayload.Info.ValidData)
            {
                JOYSTICK_OWNERSHIP_COMMAND topic = topicPayload.Data;

                lock (this.userIdentity)
                {
                    if (this.userIdentity.isIdentified())
                    {
                        if (this.userIdentity.joystickOwnership.progress == SUCCESSION_PROCESS.CLAIM)
                        {
                            switch (topic)
                            {
                                case var _ when ((topic.eHeirMfc == this.userIdentity.mfcId) && (topic.eHeirSW != this.userIdentity.swId)):
                                case var _ when ((topic.eHeirMfc != this.userIdentity.mfcId) && (topic.eHeirSW == this.userIdentity.swId)):
                                    {
                                        DateTime competitorClaimTime = DDS_UTIL.extractDateTime(topic.stClaimTime);

                                        if (competitorClaimTime >= this.userIdentity.joystickOwnership.claimTime)
                                        {
                                            dropJoystickOwnership();
                                        }
                                    }
                                    break;
                            }
                        }
                        else
                        {
                            dropJoystickOwnership();
                        }
                    }
                }
            }
        }

        private void readJoystickOwnershipDropCommand(LoanedSample<JOYSTICK_OWNERSHIP_DROP_COMMAND> topicPayload)
        {
            if (topicPayload.Info.ValidData)
            {
                JOYSTICK_OWNERSHIP_DROP_COMMAND topic = topicPayload.Data;

                lock (this.userIdentity)
                {
                    if (this.userIdentity.isIdentified())
                    {
                        if (topic.eDropSW == this.userIdentity.swId)
                        {
                            this.notifyDroppedJoysticOwner?.Invoke(topic.eDropMfc, topic.eDropSW);

                            this.joystickOwnerMfcId = null;
                            this.joystickOwnerSwId = null;
                        }
                    }
                }
            }
        }
        #endregion
    }
}