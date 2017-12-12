using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;
using System.Collections;
using NorthStateFramework;
using NSFTime = System.Int64;
using UInt8 = System.Byte;
using Int8 = System.SByte;
using System.Globalization;

namespace TraceFileReader.TraceTypes
{
    class CanMessageTrace : Trace
    {
        #region Members

        #region String Constants
        private const String PortNameString = "PortName";
        private const String BaudRateString = "BaudRate";
        private const String DataBitsString = "DataBits";
        private const String ParityString = "Parity";
        private const String StopBitsString = "StopBits";
        private const String ResetBoardTimeString = "ResetBoardTime";
        private const String QueryingQueuesString = "QueryingQueues";
        private const String CheckStatusEventString = "CheckStatusEvent";

        private const String BaudRate2400String = "U6";
        private const String BaudRate9600String = "U5";
        private const String BaudRate19200String = "U4";
        private const String BaudRate38400String = "U3";
        private const String BaudRate57600String = "U2";
        private const String BaudRate115200String = "U1";

        private const String CANRate10String = "S0";
        private const String CANRate20String = "S1";
        private const String CANRate50String = "S2";
        private const String CANRate100String = "S3";
        private const String CANRate125String = "S4";
        private const String CANRate250String = "S5";
        private const String CANRate500String = "S6";
        private const String CANRate800String = "S7";
        private const String CANRate1000String = "S8";

        private const String ResetCommand = "R";
        private const String VersionCommand = "V";
        private const String TimestampOffCommand = "Z0";
        private const String AutoPollOnCommand = "X1";
        private const String ReadStatusCommand = "F";
        private const String OpenCANNetworkCommand = "O";
        private const String CloseCANNetworkCommand = "C";
        private const String ReadCANTxBufferEntriesCommand = "G";
        private const String ReadCANRxBufferEntriesCommand = "H";
        private const String ReadSerialTxBufferEntriesCommand = "I";
        private const String ReadSerialRxBufferEntriesCommand = "J";

        private const Char CANMessage = 't';
        private const Char ExtendedCANMessage = 'T';
        private const Char ReadStatusResponse = 'F';
        private const Char ReadCANTxBufferEntriesResponse = 'G';
        private const Char ReadCANRxBufferEntriesResponse = 'H';
        private const Char ReadSerialTxBufferEntriesResponse = 'I';
        private const Char ReadSerialRxBufferEntriesResponse = 'J';

        private const String serialReadLineErrorCountString = "SerialReadLineErrorCount";
        private const String serialWriteLineErrorCountString = "SerialWriteLineErrorCount";
        private const String serialErrorReceivedCountString = "SerialErrorReceivedCount";
        private const String invalidCommandCountString = "InvalidCommandCount";
        private const String invalidResponseCountString = "InvalidResponseCount";
        private const String unexpectedResponseCountString = "UnexpectedResponseCount";
        private const String firmwareErrorCountString = "FirmwareErrorCount";
        private const String canTxBufferMaxUsedEntriesString = "CANTxBufferMaxUsedEntries";
        private const String canRxBufferMaxUsedEntriesString = "CANRxBufferMaxUsedEntries";
        private const String serialTxBufferMaxUsedEntriesString = "SerialTxBufferMaxUsedEntries";
        private const String serialRxBufferMaxUsedEntriesString = "SerialRxBufferMaxUsedEntries";
        private const String canTxBufferUsedEntriesString = "CANTxBufferUsedEntries";
        private const String canRxBufferUsedEntriesString = "CANRxBufferUsedEntries";
        private const String serialTxBufferUsedEntriesString = "SerialTxBufferUsedEntries";
        private const String serialRxBufferUsedEntriesString = "SerialRxBufferUsedEntries";

        #endregion String Constants

        public enum NodeId
        {
            RobotZNode = 10,                    //0x0A
            RobotXNode = 11,                    //0x0B
            RobotThetaNode = 12,                //0x0C
            RobotGripperNode = 13,              //0x0D
            LeftVialFeederNode = 20,            //0x14
            RightVialFeederNode = 21,           //0x15
            LeftCapFeederNode = 30,             //0x1E
            RightCapFeederNode = 31,            //0x1F
            CapperElevatorNode = 32,            //0x20
            CapperSpinnerNode = 33,             //0x21
            LabelerIndexWheelNode = 40,         //0x28
            LabelerDriveRollerNode = 41,        //0x29
            CellEPRNode = 50,                   //0x32
            CellDRMNode = 51,                   //0x33
            DropOffLeftBinArrayNode = 60,       //0x3C
            DropOffRightBinArrayNode = 61,      //0x3D
            ExceptionCarouselNode = 62,         //0x3E
            DropOffOverflowBinNode = 63         //0x3F
        }

        // The term "Command Specifier" is from the CANOpen spec and refers to the first byte of an SDO,
        // which describes the nature of the SDO
        public enum CommandSpecifier
        {
            // Client
            ExpeditedDownloadRequest = 0x22,
            NormalDownloadRequest = 0x20,
            DownloadEvenSegmentRequest = 0x00,
            DownloadOddSegmentRequest = 0x10,
            DownloadEvenLastSegmentRequest = 0x01,
            DownloadOddLastSegmentRequest = 0x11,
            UploadRequest = 0x40,
            UploadEvenSegmentRequest = 0x60,
            UploadOddSegmentRequest = 0x61,
            InitiateBlockDownloadRequest = 0xC4,
            EndBlockDownloadRequest = 0xC1,
            // Server
            ExpeditedDownloadResponse = 0x60,
            NormalDownloadResponse = 0x60,
            DownloadEvenSegmentResponse = 0x20,
            DownloadOddSegmentResponse = 0x30,
            DownloadEvenLastSegmentResponse = 0x20,
            DownloadOddLastSegmentResponse = 0x30,
            ExpeditedUploadResponse = 0x42,
            NormalUploadResponse = 0x40,
            UploadEvenSegmentResponse = 0x00,
            UploadOddSegmentResponse = 0x10,
            UploadEvenLastSegmentResponse = 0x01,
            UploadOddLastSegmentResponse = 0x11,
            InitiateBlockDownloadResponse = 0xA4,
            BlockSegmentResponse = 0xA2,
            EndBlockDownloadResponse = 0xA1,
            // Bi-Directional
            BlockDownloadAbort = 0x80
        };

        protected const UInt32 DigitalInputPDONumber = 1;
        protected const UInt32 AnalogInputPDONumber = 2;
        protected const UInt32 StatusPDONumber = 3;

        protected enum AnalogOutputNumbers { SensorLEDOutputNumber = 1 }

        protected enum AnalogInputNumbers { ActualPositionInput = 17, ActualVelocityInput = 18, ActualCurrentInput = 19, ContinuousCurrentInput = 20 }

        #region Single SubIndex List
        //PWMCyclesBeforeSensorSampleSubindex = 0,
        //SensorSamplesPerRoundRobinSubindex = 0,
        //PWMFrequencySubindex = 0,
        //SampleWeightSubindex = 0,
        //DigitalInputsSubIndex = 0x01,    
        //DownloadProgramDataSubindex = 0x00,        
        //FirmwareStatusSubindex = 0x00,        
        //ResetBoardSubindex = 0x00,        
        //ControlWordSubindex = 0,
        //StatusWordSubindex = 0,        
        //ControlModeSubindex = 0,        
        //ErrorCodeSubindex = 0,
        //StatusWordAndErrorCodeSubindex = 0,
        //TargetPWMSubindex = 0,
        //ActualPWMSubindex = 0,
        //TargetCurrentSubindex = 0,
        //ActualCurrentSubindex = 0,
        //TargetPositionSubindex = 0,
        //CommandedPositionSubindex = 0,        
        //ActualPositionSubindex = 0,
        //TargetVelocitySubindex = 0,
        //ProfileTimeSubindex = 0,
        //ActualVelocitySubindex = 0,
        //AverageVelocityTimeSubindex = 0,
        //AverageVelocityTimeSubindex = 0,
        //MaxVelocitySubindex = 0,
        //MaxAccelerationSubindex = 0,
        //AverageVelocitySubindex = 0,
        //MaxDecelerationSubindex = 0,
        //MaxJerkSubindex = 0,
        //PositionErrorWindowSubindex = 0,
        //InPositionWindowSubindex = 0,
        //InPositionTimeSubindex = 0,        
        //ContinuousCurrentSubindex = 0,
        //ContinuousCurrentTimeSubindex = 0,        
        //ContinuousCurrentLimitSubindex = 0,        
        //MinInstantCurrentLimitSubindex = 0,        
        //MaxInstantCurrentLimitSubindex = 0,
        //ProfileScaledCountsPerCountSubindex = 0,        
        //ProfileUpdateFrequencySubindex = 0,
        //ActualPositionOffsetSubindex = 0,
        //ActualPositionErrorSubindex = 0,
        //ActualAbsolutePositionErrorSubindex = 0,
        //MaxActualAbsolutePositionErrorSubindex = 0,
        //CommutationOffsetSubindex = 0,
        //SteppingPWMSubindex = 0,
        //LockingSettlingPWMSubindex = 0,
        //LockingRampIncrementPWMSubindex = 0,
        //LockingRampIncrementTimeSubindex = 0,
        //LockingSettlingTimeSubindex = 0,
        //SteppingTimeSubindex = 0,
        //MinimumFreedomSubindex = 0,
        //MotorTemperatureSensorInstalledSubindex = 0,
        //CommutationHysteresisSubindex = 0,
        //CommutationTypeSubindex = 0,        
        //ProtocolversionSubIndex = 0x00,
        //DownloadProgramDataSubindex = 0x00,
        //FirmwareStatusSubindex = 0x00,
        //ResetBoardSubindex = 0x00,
        //HeartbeatTimeSubindex = 0x00,
        //KeepAliveTimeSubIndex = 0x00,
        #endregion Single SubIndex List

        public enum CurrentControlParametersSubIndex
        {
            CurrentProportionalGainSubindex = 1,
            CurrentDerivativeGainSubindex = 2,
            CurrentDerivativeSampleCountSubindex = 3,
            CurrentIntegralGainSubindex = 4,
            CurrentIntegrationSummationLimitSubindex = 5,
            CurrentIntegrationContributionLimitSubindex = 6
        }

        public enum PositionControlParametersSubIndex
        {
            PositionProportionalGainSubindex = 1,
            PositionDerivativeGainSubindex = 2,
            PositionDerivativeSampleCountSubindex = 3,
            PositionIntegralGainSubindex = 4,
            PositionIntegrationSummationLimitSubindex = 5,
            PositionIntegrationContributionLimitSubindex = 6,
            PositionVelocityFeedForwardSubindex = 7,
            PositionAccelerationFeedForwardSubindex = 8,
            PositionGravityFeedForwardSubindex = 9
        }

        public enum HistoryParametersSubIndex
        {
            HistorySampleTimeSubindex = 0,
            HistoryLengthSubindex = 1,
            HistoryModeSubindex = 2,
            HistoryOnSubindex = 3
        }

        public enum IdentitySubIndex
        {
            VendorIdSubindex = 0x01,
            ProductCodeSubindex = 0x02,
            HardwareRevisionNumberSubindex = 0x03,
            BootCodePartNumberSubIndex = 0x05,
            BootCodeRevisionSubIndex = 0x06,
            FirmwarePartNumberSubIndex = 0x07,
            FirmwareRevisionSubIndex = 0x08
        }

        #region Servo Types

        protected enum ControlMode : sbyte
        {
            PWM = -1,
            Position = 1,
            Velocity = 3,
            Current = 4,
        }

        [Flags]
        protected enum ControlWordFlags : ushort
        {
            None = 0x0000,
            SwitchOn = 0x0001,
            EnableVoltage = 0x0002,
            QuickStopOff = 0x0004,
            EnableOperation = 0x0008,
            NewSetPoint = 0x0010,
            ChangeImmediately = 0x0020,
            RelativeMove = 0x0040,
            ResetFault = 0x0080,
            Halt = 0x0100,
            All = 0xFFFF
        }

        [Flags]
        protected enum StatusWordFlags : ushort
        {
            ReadyToSwitchOn = 0x0001,
            SwitchedOn = 0x0002,
            OperationEnabled = 0x0004,
            Fault = 0x0008,
            VoltageEnabled = 0x0010,
            QuickStopOff = 0x0020,
            SwitchOnDisabled = 0x0040,
            Warning = 0x0080,
            TrajectoryAborted = 0x0100,
            Remote = 0x0200,
            TargetReached = 0x0400,
            SetPointAck = 0x1000,
            TrajectoryRunning = 0x4000,
        }

        [Flags]
        protected enum ErrorCodeFlags : ushort
        {
            NoError = 0x0000,
            FirmwareError = 0x0001,
            FlashWriteError = 0x0002,
            InvalidHallState = 0x0004,          // valid only in hall commutation context
            AlignmentError = 0x0004,            // valid only in encoder commutation context
            InvalidHallTransition = 0x0008,     // valid only in hall commutation context
            CommutationError = 0x0008,          // valid only in encoder commutation context
            BoardTempError = 0x0010,
            MotorTempError = 0x0020,
            HBridgeFault = 0x0040,
            EStop = 0x0080,
            InputFault = 0x0100,
            Current_Offset_Error = 0x0200,
            CAN_Error = 0x400,
            PositionLimit = 0x0800,
            KeepAliveTimeout = 0x1000,
            FollowingError = 0x2000,
            CurrentLimit = 0x8000
        }

        public enum Commutation : byte
        {
            Undefined = 0,
            Mechanical = 1,
            EncoderDetent = 2,
            Hall = 3,
            EncoderIndex = 4
        }

        #endregion Servo Types

        public enum SDOIndex
        {
            // CANOpenAnalogInputs indices

            AnalogInputIndex = 0x6402,

            RisingEdgeNotificationThresholdIndex = 0x6424,
            FallingEdgeNotificationThresholdIndex = 0x6425,
            EdgeNotificationDeadbandIndex = 0x6426,

            AnalogStopOnRisingEdgeIndex = 0x2045,
            AnalogStopOnFallingEdgeIndex = 0x2047,
            AnalogStopOnHighIndex = 0x2021,
            AnalogStopOnLowIndex = 0x2023,

            AnalogFaultOnRisingEdgeIndex = 0x202B,
            AnalogFaultOnFallingEdgeIndex = 0x202D,

            AnalogLatchPositionOnRisingEdgeIndex = 0x2035,
            AnalogLatchPositionOnFallingEdgeIndex = 0x2037,

            // CANOpenAnalogOutputs indices

            AnalogOutputIndex = 0x6411,

            // CANOpenDigitalInputs indices

            DigitalInputsIndex = 0x6120,
            ChangeOfStateInterruptIndex = 0x2025,
            DigitalStopOnRisingEdgeIndex = 0x2041,
            DigitalStopOnFallingEdgeIndex = 0x2043,
            DigitalStopOnHighIndex = 0x2017,
            DigitalStopOnLowIndex = 0x2019,
            DigitalFaultOnRisingEdgeIndex = 0x203B,
            DigitalFaultOnFallingEdgeIndex = 0x203D,
            DigitalLatchPositionOnRisingEdgeIndex = 0x2031,
            DigitalLatchPositionOnFallingEdgeIndex = 0x2033,

            // CANOpenDigitalOutputs Indices

            DigitalOutputIndex = 0x6220,

            // CANOpenFirmwareUpdate Indices

            DownloadProgramDataIndex = 0x1F50,
            FirmwareStatusIndex = 0x200C,
            ResetBoardIndex = 0x200D,

            // CANOpenServoMotor Indices

            ControlWordIndex = 0x6040,
            StatusWordIndex = 0x6041,
            ControlModeIndex = 0x6060,
            ErrorCodeIndex = 0x603F,
            StatusWordAndErrorCodeIndex = 0x2050,

            // Target and actual PWM are the same thing
            TargetPWMIndex = 0x2039,
            ActualPWMIndex = 0x2039,
            TargetCurrentIndex = 0x6071,
            ActualCurrentIndex = 0x6078,
            TargetPositionIndex = 0x607A,
            CommandedPositionIndex = 0x60FC,
            ActualPositionIndex = 0x6064,
            TargetVelocityIndex = 0x60ff,
            ProfileTimeIndex = 0x2009,
            ActualVelocityIndex = 0x6069,
            AverageVelocityIndex = 0x200E,
            AverageVelocityTimeIndex = 0x200F,
            MoveAverageCurrentIndex = 0x2049,

            // Trajectory control parameters
            MaxVelocityIndex = 0x6081,
            MaxAccelerationIndex = 0x6083,
            MaxDecelerationIndex = 0x6084,
            MaxJerkIndex = 0x2007,
            PositionErrorWindowIndex = 0x6065,
            InPositionWindowIndex = 0x6067,
            InPositionTimeIndex = 0x6068,

            // Current control Parameters
            CurrentControlParametersIndex = 0x60F6,
            ContinuousCurrentIndex = 0x2004,
            ContinuousCurrentTimeIndex = 0x2005,
            ContinuousCurrentLimitIndex = 0x2006,
            MinInstantCurrentLimitIndex = 0x200B,
            MaxInstantCurrentLimitIndex = 0x200A,

            // Position control parameters
            PositionControlParametersIndex = 0x60FB,
            CommandedPositionHistoryIndex = 0x2010,
            ActualPositionHistoryIndex = 0x2011,
            CommandedCurrentHistoryIndex = 0x2012,
            ActualCurrentHistoryIndex = 0x2013,
            PWMHistoryIndex = 0x2028,
            HistoryParametersIndex = 0x2014,
            ProfileScaledCountsPerCountIndex = 0x2026,
            ProfileUpdateFrequencyIndex = 0x2027,
            ActualPositionOffsetIndex = 0x2038,
            ActualPositionErrorIndex = 0x201A,
            ActualAbsolutePositionErrorIndex = 0x201B,
            MaxActualAbsolutePositionErrorIndex = 0x2015,

            // Alignment control parameters
            CommutationOffsetIndex = 0x2070,
            SteppingPWMIndex = 0x2071,
            LockingSettlingPWMIndex = 0x2072,
            LockingRampIncrementPWMIndex = 0x2073,
            LockingRampIncrementTimeIndex = 0x2074,
            LockingSettlingTimeIndex = 0x2075,
            SteppingTimeIndex = 0x2076,
            MinimumFreedomIndex = 0x2077,

            // Commutation control parameters
            MotorTemperatureSensorInstalledIndex = 0x2080,
            CommutationHysteresisIndex = 0x2081,
            MotorSectorSizeTableIndex = 0x2082,
            CommutationTypeIndex = 0x2083,

            // CANOpenBinIO Indices

            PWMCyclesBeforeSensorSampleIndex = 0x2051,
            SensorSamplesPerRoundRobinIndex = 0x2056,
            PWMFrequencyIndex = 0x2052,
            SampleWeightIndex = 0x2055,

            // CANOpenBinAnalogInputs indices

            // CANOpenBinAnalogOutputs indices

            // CANOpenBinDigitalOut Indices

            // CANOpenBinDigitalInputs indices

            //CANOpen Constants
            HeartbeatTimeIndex = 0x1017,
            KeepAliveTimeIndex = 0x204D,
            IdentityIndex = 0x1018,
            ProtocolVersionIndex = 0x204E,

        }

        protected const UInt32 EmergencyBaseId = 0x80;
        protected const UInt32 SyncId = 0x80;
        protected const UInt32 HeartbeatBaseId = 0x700;
        protected const UInt32 NMTCommandId = 0x0;

        protected const UInt32 NodeIdAcceptanceMask = ~((UInt32)0x3F);
        protected const UInt32 NoProtocol = UInt32.MaxValue;
        protected const UInt32 BootProtocol = 0;

        protected enum HeartbeatStatus { Bootup = 0, Stopped = 4, Operational = 5, PreOperational = 127 };

        protected enum NMTCommandSpecifier { Start = 1, Stop = 2, PreOperational = 128, ResetNode = 129, ResetCommunications = 130, SetNodeID = 224 };

        // These terms are a bit confusing, but widely used in the CANOpen spec
        // Transmit and Receive are with respect to the node, not the master
        protected const UInt32 TransmitSDOBaseId = 0x580;
        protected const UInt32 ReceiveSDOBaseId = 0x600;

        // Hashtable with commands as keys into arrays of possible responses
        protected static readonly Hashtable commandResponseTable = new Hashtable();

        private String readData = String.Empty;
        [Flags]
        private enum CANStatus : ushort
        {
            NoError = 0x00,
            CANTxQueueFull = 0x01,
            SerialTxQueueFull = 0x02,
            SerialRxQueueFull = 0x04,
            InvalidCommand = 0x08,
            CANTxFailure = 0x10,
            CANRxFailure = 0x20,
            SerialTxFailure = 0x40,
            SerialRxFailure = 0x80,
        }

        #region Constants

        private const char STX = (char)0x02;
        private const string messageTypeString = "MessageType";
        private const string nodeIdString = "NodeID";
        private const string commandTypeString = "CommmandType";
        private const string indexString = "Index";
        private const string subIndexString = "SubIndex";
        private const string valueString = "Value";

        #endregion
        private byte[] data;
        private string[] message;
        private UInt32 msgId;
        private static StatusWordFlags statusWord;
        private static ErrorCodeFlags errorCode;

        Dictionary<string, string> messageDetails = new Dictionary<string, string>();

        #endregion Members
        
        public CanMessageTrace()
            : base()
        {

        }

        public CanMessageTrace(XPathNavigator trace)
            : base(trace)
        {
            base.DerivedType = this.GetType();
            TagAndData.Add("Direction", this.TraceType);
            TraceType = "CanMessaging";
            data = new byte[8];
            setMessage(TagAndData[NSFTraceTags.messageTag]);

            foreach (KeyValuePair<string, string> kvp in parseMessage())
                TagAndData.Add(kvp.Key, kvp.Value);
        }

        private void setMessage(string msg)
        {
            int index = 0;
            message = msg.Split(' ');

            foreach (string s in message)
            {
                if (index > 0)
                {
                    data[index - 1] = UInt8.Parse(s, NumberStyles.HexNumber);
                    index++;
                }
                else
                {
                    index++;
                }
            }
            msgId = UInt32.Parse(message[0], NumberStyles.HexNumber);
        }

        public Dictionary<string, string> parseMessage()
        {

            messageDetails.Clear();
            if (isHeartbeatMessage())
                return messageDetails;
            else if (isSDOResponse())
                return messageDetails;
            else if (isPDOMessage())
                return messageDetails;
            else
                return messageDetails;
        }

        private bool isHeartbeatMessage()
        {
            if ((HeartbeatBaseId & msgId) == HeartbeatBaseId)
            {
                handleHeartBeatMessage();
                return true;
            }
            return false;
        }

        private string handleHeartBeatMessage()
        {
            Double txErrors = Convert.ToDouble(getDataUInt8(6));
            Double rxErrors = Convert.ToDouble(getDataUInt8(7));
            string canMessageType = string.Empty;
            NodeId node = (NodeId)(msgId - HeartbeatBaseId);

            messageDetails.Add(messageTypeString, "HeartBeatStatus");
            messageDetails.Add(nodeIdString, node.ToString());

            switch ((HeartbeatStatus)getDataUInt8(0))
            {
                case HeartbeatStatus.Bootup:
                    canMessageType = "HeartBeat Status " + "BootUp";
                    messageDetails.Add(valueString, "BootUp");
                    //if (resetState.isActive())
                    //{
                    //    queueEvent(bootupEvent, this);
                    //}
                    //else
                    //{
                    //    // If a bootup message occurs when not in reset or error state,
                    //    // then node has reset on its own!  Force through coordinated initialization!
                    //    queueEvent(nodeResetEvent, this);
                    //}
                    break;
                case HeartbeatStatus.PreOperational:
                    canMessageType = "HeartBeat Status " + "PreOperational" + node.ToString();

                    messageDetails.Add(valueString, "PreOperational");
                    // Queue event for either reset or bootup state, in case pre-op message arrives just after bootup message
                    //if (bootupState.isActive() || resetState.isActive())
                    //{
                    //    queueEvent(preOperationalEvent, this);
                    //}
                    //else if (!(preOperationalState.isActive() || verifyFirmwareState.isActive()))
                    //{
                    //    // This is an invalid transition to pre-operational, reset node!
                    //    queueEvent(invalidTransitionEvent, this);
                    //}
                    break;
                case HeartbeatStatus.Operational:
                    canMessageType = "HeartBeat Status " + "Operational " + node.ToString();

                    messageDetails.Add(valueString, "Operational");
                    // If the node is in preOperational the heartbeat message will be consumed on the transition from preOperational to operational

                    //if (!(resetState.isActive() || operationalState.isActive()))
                    //{
                    //    // This is an invalid transition to operational, reset node!
                    //    queueEvent(invalidTransitionEvent, this);
                    //}
                    break;
                case HeartbeatStatus.Stopped:
                    canMessageType = "HeartBeat Status " + "Stopped";

                    messageDetails.Add(valueString, "Stopped");
                    // Handle stopped as an error, reset node!
                    //queueEvent(nodeStoppedEvent, this);
                    break;
                default:
                    canMessageType = "HeartBeat Status " + "Unexpected";

                    messageDetails.Add(valueString, "UnExpected");
                    // Unexpected state, reset node!
                    //queueEvent(invalidTransitionEvent, this);
                    break;
            }
            return canMessageType;
        }


        private bool isPDOMessage()
        {
            // Checks if message is a Transmit PDO

            if ((msgId > 0x180) && (msgId < 0x580) && ((msgId & 0x80) == 0x80))
            {
                NodeId node = (NodeId)((0xFF & msgId) - 0x80);

                UInt32 pdoNumber = msgId >> 8;

                switch (pdoNumber)
                {
                    case DigitalInputPDONumber:

                        messageDetails.Add(messageTypeString, "DigitalInput_PDO");
                        messageDetails.Add(nodeIdString, node.ToString());
                        // To do
                        break;

                    case AnalogInputPDONumber:

                        messageDetails.Add(messageTypeString, "AnalogInput_PDO");
                        messageDetails.Add(nodeIdString, node.ToString());
                        handleAnalogInputPDOMessage();
                        break;

                    case StatusPDONumber:

                        messageDetails.Add(messageTypeString, "Status_PDO");
                        messageDetails.Add(nodeIdString, node.ToString());

                        handleStatusPDOMessage();
                        break;
                }
                return true;
            }
            else
                return false;
        }

        protected void handleAnalogInputPDOMessage()
        {
            UInt16 inputNumber = getDataUInt16(0);

            switch ((AnalogInputNumbers)inputNumber)
            {
                case AnalogInputNumbers.ActualPositionInput:
                    messageDetails.Add(commandTypeString, AnalogInputNumbers.ActualPositionInput.ToString());
                    messageDetails.Add(valueString, getDataInt32(2).ToString());
                    // To do
                    //latchedPosition = convertFromPositionCounts(message.getDataInt32(2), true);
                    break;

                case AnalogInputNumbers.ActualVelocityInput:
                    messageDetails.Add(commandTypeString, AnalogInputNumbers.ActualVelocityInput.ToString());
                    messageDetails.Add(valueString, getDataInt32(2).ToString());

                    // To do
                    break;

                case AnalogInputNumbers.ActualCurrentInput:
                    messageDetails.Add(commandTypeString, AnalogInputNumbers.ActualCurrentInput.ToString());
                    messageDetails.Add(valueString, getDataInt32(2).ToString());

                    // To do
                    break;

                case AnalogInputNumbers.ContinuousCurrentInput:
                    messageDetails.Add(commandTypeString, AnalogInputNumbers.ContinuousCurrentInput.ToString());
                    messageDetails.Add(valueString, getDataInt32(2).ToString());

                    // To do
                    break;
            }
        }


        protected void handleStatusPDOMessage()
        {
            errorCode = (ErrorCodeFlags)getDataUInt16(6);

            messageDetails.Add(commandTypeString, "ErrorCode - ActualPosition - StatusWord");
            statusWord = (StatusWordFlags)getDataUInt16(0);
            messageDetails.Add(valueString, errorCode.ToString() + " - " + getDataInt32(2).ToString() + " - " + statusWord.ToString());
            //ActualPosition = convertFromPositionCounts(getDataInt32(2), true);

            handleStatusWordUpdate((StatusWordFlags)getDataUInt16(0));
        }

        protected void handleStatusWordUpdate(StatusWordFlags newStatusWord)
        {
            StatusWordFlags previousStatusWord = statusWord;
            statusWord = newStatusWord;

            // Check for faults first, so that the motor will go to error immediately
            // Error code must be set to latest error code before this call.
            if (((statusWord & StatusWordFlags.Fault) == StatusWordFlags.Fault)
                && (((previousStatusWord & StatusWordFlags.Fault) != StatusWordFlags.Fault)))
            {
                if ((errorCode & ErrorCodeFlags.BoardTempError) == ErrorCodeFlags.BoardTempError)
                {
                    //////queueEvent(boardTempErrorEvent, this);
                }
                else if ((errorCode & ErrorCodeFlags.EStop) == ErrorCodeFlags.EStop)
                {
                    //queueEvent(eStopEvent, this);
                }
                else if ((errorCode & ErrorCodeFlags.FirmwareError) == ErrorCodeFlags.FirmwareError)
                {
                    //queueEvent(firmwareErrorEvent, this);
                }
                else if ((errorCode & ErrorCodeFlags.FlashWriteError) == ErrorCodeFlags.FlashWriteError)
                {
                    //queueEvent(flashWriteErrorEvent, this);
                }
                else if ((errorCode & ErrorCodeFlags.FollowingError) == ErrorCodeFlags.FollowingError)
                {
                    //queueEvent(followingErrorEvent, this);
                }
                else if ((errorCode & ErrorCodeFlags.HBridgeFault) == ErrorCodeFlags.HBridgeFault)
                {
                    //hBridgeFaultCounter.Value++;
                    //queueEvent(hBridgeFaultEvent, this);
                }
                else if ((errorCode & ErrorCodeFlags.InputFault) == ErrorCodeFlags.InputFault)
                {
                    //queueEvent(inputFaultEvent, this);
                }

                // ErrorCodeFlags.InvalidHallState and ErrorCodeFlags.AlignmentError are overloaded uses
                // of the same flag. Let the commutation type provide the context when determining which
                // specific error event to //queue.
                else if ((errorCode & ErrorCodeFlags.InvalidHallState) == ErrorCodeFlags.InvalidHallState)
                //& (this.commutationType == Commutation.Hall))
                {
                    //queueEvent(invalidHallStateEvent, this);
                }
                else if ((errorCode & ErrorCodeFlags.AlignmentError) == ErrorCodeFlags.AlignmentError)
                //& (this.commutationType == Commutation.EncoderDetent))
                {
                    //queueEvent(alignmentErrorEvent, this);
                }
                else if ((errorCode & ErrorCodeFlags.AlignmentError) == ErrorCodeFlags.AlignmentError)
                //& (this.commutationType == Commutation.EncoderIndex))
                {
                    //queueEvent(alignmentErrorEvent, this);
                }

                // ErrorCodeFlags.InvalidHallTransition and ErrorCodeFlags.CommutationError are overloaded
                // uses of the same flag. Let the commutation type provide the context when determining which
                // specific error event to //queue.
                else if ((errorCode & ErrorCodeFlags.InvalidHallTransition) == ErrorCodeFlags.InvalidHallTransition)
                // & (this.commutationType == Commutation.Hall))
                {
                    //queueEvent(invalidHallTransitionEvent, this);
                }
                else if ((errorCode & ErrorCodeFlags.CommutationError) == ErrorCodeFlags.CommutationError)
                //  & (this.commutationType == Commutation.EncoderDetent))
                {
                    //queueEvent(commutationErrorEvent, this);
                }
                else if ((errorCode & ErrorCodeFlags.CommutationError) == ErrorCodeFlags.CommutationError)
                //  & (this.commutationType == Commutation.EncoderIndex))
                {
                    //queueEvent(commutationErrorEvent, this);
                }

                else if ((errorCode & ErrorCodeFlags.MotorTempError) == ErrorCodeFlags.MotorTempError)
                {
                    //queueEvent(motorTempErrorEvent, this);
                }
                else if ((errorCode & ErrorCodeFlags.PositionLimit) == ErrorCodeFlags.PositionLimit)
                {
                    //queueEvent(positionLimitEvent, this);
                }
                else
                {
                    // Unrecognized error code, just indicate generic drive fault
                    //queueEvent(driveFaultEvent, this);
                }
            }

            // Check for servo status second, so that it will go to servo on/off immediately
            // Note: The Servo On state corresponds to the CiA DSP 402 state "Operation Enable".
            //       The Servo Off state with brake enabled corresponds to the CiA DSP 402 state "Ready to Switch On".
            //       The Servo Off state with brake disabled corresponds to the CiA DSP 402 state "Switched On".

            if (((statusWord & StatusWordFlags.ReadyToSwitchOn) == StatusWordFlags.ReadyToSwitchOn)
                && ((previousStatusWord & StatusWordFlags.ReadyToSwitchOn) != StatusWordFlags.ReadyToSwitchOn))
            {
                //queueEvent(readyEvent, this);
            }

            if (((statusWord & StatusWordFlags.OperationEnabled) == StatusWordFlags.OperationEnabled)
                && ((previousStatusWord & StatusWordFlags.OperationEnabled) != StatusWordFlags.OperationEnabled))
            {
                //queueEvent(servoOnEvent, this);
            }

            if (((statusWord & StatusWordFlags.OperationEnabled) != StatusWordFlags.OperationEnabled)
                && ((previousStatusWord & StatusWordFlags.OperationEnabled) == StatusWordFlags.OperationEnabled))
            {
                //queueEvent(servoOffEvent, this);
            }

            // Check motion status last

            if (((statusWord & StatusWordFlags.SetPointAck) == StatusWordFlags.SetPointAck)
                && ((previousStatusWord & StatusWordFlags.SetPointAck) != StatusWordFlags.SetPointAck))
            {
                //clearSetPointAckTimers();
                //queueEvent(newTargetEvent, this);
            }

            if (((statusWord & StatusWordFlags.TargetReached) == StatusWordFlags.TargetReached)
                && ((previousStatusWord & StatusWordFlags.TargetReached) != StatusWordFlags.TargetReached))
            {
                //queueEvent(targetReachedEvent, this);
            }

            if (((statusWord & StatusWordFlags.TrajectoryRunning) != StatusWordFlags.TrajectoryRunning)
                && ((previousStatusWord & StatusWordFlags.TrajectoryRunning) == StatusWordFlags.TrajectoryRunning))
            {
                //queueEvent(trajectoryCompleteEvent, this);
            }
        }


        public bool isSDOResponse()
        {
            // Check id range first
            if ((msgId > TransmitSDOBaseId) || (msgId > ReceiveSDOBaseId))
            {

                if (msgId < ReceiveSDOBaseId)
                {
                    NodeId node = (NodeId)(msgId - TransmitSDOBaseId);
                    messageDetails.Add(messageTypeString, "SDOFromBoard");
                    messageDetails.Add(nodeIdString, node.ToString());
                }
                else
                {
                    NodeId node = (NodeId)(msgId - ReceiveSDOBaseId);
                    messageDetails.Add(messageTypeString, "SDOToBoard");
                    messageDetails.Add(nodeIdString, node.ToString());
                }

                CommandSpecifier response = (CommandSpecifier)getDataUInt8(0);

                // If response should contain object dictionary index/subindex, check that as well
                if (response == CommandSpecifier.ExpeditedDownloadResponse)
                    messageDetails.Add(commandTypeString, CommandSpecifier.ExpeditedDownloadResponse.ToString());
                else if (response == CommandSpecifier.NormalDownloadResponse)
                    messageDetails.Add(commandTypeString, CommandSpecifier.NormalDownloadResponse.ToString());
                else if (response == CommandSpecifier.ExpeditedUploadResponse)
                    messageDetails.Add(commandTypeString, CommandSpecifier.ExpeditedUploadResponse.ToString());
                else if (response == CommandSpecifier.NormalUploadResponse)
                    messageDetails.Add(commandTypeString, CommandSpecifier.NormalUploadResponse.ToString());
                else if (response == CommandSpecifier.InitiateBlockDownloadResponse)
                    messageDetails.Add(commandTypeString, CommandSpecifier.InitiateBlockDownloadResponse.ToString());
                else
                    messageDetails.Add(commandTypeString, response.ToString());

                SDOIndex index = (SDOIndex)(getDataInt16(1));

                messageDetails.Add(indexString, index.ToString());

                switch (index)
                {
                    case SDOIndex.IdentityIndex:
                        IdentitySubIndex iSubindex = (IdentitySubIndex)getDataUInt8(3);
                        messageDetails.Add(subIndexString, iSubindex.ToString());
                        break;
                    case SDOIndex.CurrentControlParametersIndex:
                        CurrentControlParametersSubIndex cSubindex = (CurrentControlParametersSubIndex)(getDataUInt8(3));
                        messageDetails.Add(subIndexString, cSubindex.ToString());
                        break;
                    case SDOIndex.HistoryParametersIndex:
                        HistoryParametersSubIndex hSubindex = (HistoryParametersSubIndex)(getDataUInt8(3));
                        messageDetails.Add(subIndexString, hSubindex.ToString());
                        break;
                    case SDOIndex.PositionControlParametersIndex:
                        PositionControlParametersSubIndex pSubindex = (PositionControlParametersSubIndex)(getDataUInt8(3));
                        messageDetails.Add(subIndexString, pSubindex.ToString());
                        break;
                    default:
                        messageDetails.Add(subIndexString, getDataUInt8(3).ToString());
                        //MessageBox.Show(string.Format("Issue with Index: {0} != {1}", index1, index2));
                        break;
                }

                return true;
            }
            else
                return false;
        }


        public bool isAbortMessage()
        {
            // Check id range first
            if ((msgId < TransmitSDOBaseId) || (msgId > ReceiveSDOBaseId))
            {
                return false;
            }
            char[] id = message[0].ToCharArray();

            // Check if this is block download abort
            return ((CommandSpecifier)id[0] == CommandSpecifier.BlockDownloadAbort);
        }



        public Int8 getDataInt8(UInt32 index)
        {
            if (index > 7)
            {
                throw new Exception("CANMessage message index out of range");
            }
            return (Int8)(data[index]);
        }

        public UInt8 getDataUInt8(UInt32 index)
        {
            if (index > 7)
            {
                throw new Exception("CANMessage message index out of range");
            }
            return (UInt8)(data[index]);
        }

        public Int16 getDataInt16(UInt32 index)
        {
            if (index > 6)
            {
                throw new Exception("CANMessage message index out of range");
            }
            return (Int16)(data[index] + ((UInt16)data[index + 1] << 8));
        }

        public UInt16 getDataUInt16(UInt32 index)
        {
            if (index > 6)
            {
                throw new Exception("CANMessage message index out of range");
            }
            return (UInt16)(data[index] + ((UInt16)data[index + 1] << 8));
        }

        public Int32 getDataInt32(UInt32 index)
        {
            if (index > 4)
            {
                throw new Exception("CANMessage message index out of range");
            }
            return (Int32)(data[index] + ((UInt32)data[index + 1] << 8) + ((UInt32)data[index + 2] << 16) + ((UInt32)data[index + 3] << 24));
        }

        public UInt32 getDataUInt32(UInt32 index)
        {
            if (index > 4)
            {
                throw new Exception("CANMessage message index out of range");
            }
            return (UInt32)(data[index] + ((UInt32)data[index + 1] << 8) + ((UInt32)data[index + 2] << 16) + ((UInt32)data[index + 3] << 24));
        }


    }
}

