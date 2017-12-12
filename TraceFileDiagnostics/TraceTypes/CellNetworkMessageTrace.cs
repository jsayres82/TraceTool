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
using System.Windows.Forms;
using Parata.Controls.Enumerations.ErrorCodes;

namespace TraceFileReader.TraceTypes
{
    class CellNetworkMessageTrace : Trace
    {
        #region Network protocol
        public enum NetworkBoardLEDState : byte
        {
            Off = 0,
            On = 1
        }

        public enum NetworkCommands : byte
        {
            Ping = 0x01,
            ReportNVMData = 0x02,
            ReportFaults = 0x03,
            SetBITControl = 0x04,
            GetBITStatus = 0x05,
            SetLED = 0x06,
            GetCellPresentStatus = 0x07,
            //Reserved = 0x08,
            PerformPeriodicBIT = 0x09,
            ReadHardwareRevision = 0x0A,
            ReadNetworkAddress = 0x0B,
            ReadCellNetworkCurrent = 0x0C,
            SetDiagnosticsControl = 0x0D,
            GetDiagnosticsStatus = 0x0E,
            ProgramData = 0x12,
            ProgramEnd = 0x13,
            SetDownloadFlag = 0x14,
            Reboot = 0x15,
            //Reserved = 0x16,
            ClearBIT = 0x17,
            Echo = 0x18,
            ReadProtocolVersion = 0x18,
            ReadRegister = 0x80,
            WriteRegister = 0x81,
            // Flash storage control commands.
            ClearFaults = 0xFD,
            EraseNonvolatileMemory = 0xFE,
            ProgramNetworkAddress = 0xFF
        }

        #endregion Network protocol
        
        #region Cell protocol

        /// <summary>
        /// The colors for the cell's general purpose LED.
        /// </summary>
        public enum CellLEDColor
        {
            Green = 0,
            Red = 1
        }

        /// <summary>
        /// The optional states for the cell's general purpose LED.
        /// </summary>
        public enum CellLEDState : byte
        {
            Off = 0,
            On = 1
        }

        public enum CellCommands : byte
        {
            Ping = 0x01,
            ReportNVMData = 0x02,
            ReportFaults = 0x03,
            ActivateSolenoid = 0x04,
            SetCellParam = 0x05,
            SetLED = 0x06,
            SetEmitterPwm = 0x07,
            ReadDetector = 0x08,
            SetDetectorEnable = 0x09,
            ReadHardwareRevision = 0x0A,
            ReadSerialNumber = 0x0B,
            CalibrateDetector = 0x0C,
            PrepareCount = 0x0D,
            StartCount = 0x0E,
            AbortCount = 0x0F,
            GetCellStatus = 0x10,
            GetCellParam = 0x11,
            ProgramData = 0x12,
            ProgramEnd = 0x13,
            SetDownloadFlag = 0x14,
            Reboot = 0x15,
            PauseCount = 0x16,
            ResumeCount = 0x17,
            WriteParameterSequence = 0x18,
            ReadParameterSequence = 0x19,
            PulseSolenoid = 0x1A,
            // Built-in test control.  These commands are only available on Beta boards and above.
            SetBuiltInTestControl = 0x1B,
            GetBuiltInTestStatus = 0x1C,
            PerformBuiltInTest = 0x1D,
            ClearBuiltInTest = 0x1E,
            ContinueCount = 0x1F, //< "Keep Alive" command for use during counting on Beta only
            CellInsertNotification = 0x20,
            ReadProtocolVersion = 0x21,
            SetEndOfSequenceAction = 0x22,
            GetErrorFlags = 0x23,
            ClearErrorFlags = 0x24,
            // Flash storage control commands.
            ClearFaults = 0xFD,
            EraseNonvolatileMemory = 0xFE,
            ProgramCellSerialNumber = 0xFF
        }

        public enum NVMDataFlags : byte
        {
            BootCodePartNumber = 0x00,
            BootCodeRevision = 0x01,
            ProgCodePartNumber = 0x02,
            ProgCodeRevision = 0x03,
            FirmwareRevision = 0x04
        }

        /// <summary>The solenoids available on the Cell.</summary>
        public enum SolenoidTypes : byte
        {
            ForwardValve = 0x00,
            ReverseValve = 0x01,
            DamperDoor = 0x02,
            DoorLock = 0x03
        }

        /// <summary>The pill sensor detectors.</summary>
        public enum DetectorTypes : byte
        {
            Count = 0x00,
            Jam = 0x01
        }

        /// <summary>The pill sensor emitters.</summary>
        public enum EmitterTypes : byte
        {
            Count = 0x00,
            Jam = 0x01
        }

        public enum GetCellParameters : byte
        {
            PillsCounted = 0x00,
            CountEmitterCalibratedPWM = 0x01,
            JamEmitterCalibratedPWM = 0x02,
            JamRetries = 0x03,
            CountTime = 0x04,
            CountDetectorThreshold = 0x05,
            JamDetectorThreshold = 0x06,
            JamPillsCounted = 0x07,
            FragmentsCounted = 0x08,
            Bin1GapCount = 0x09,
            Bin2GapCount = 0x0A,
            OvercountGapTime = 0x0B
        }

        public enum SetCellParameters : byte
        {
            AirSolenoidHardTimeOn = 0x00,
            AirSolenoidPWMDutyCycle = 0x01,
            DetectorCalMin = 0x02,
            DetectorCalMax = 0x03,
            JamTime = 0x04,
            ClearJamTime = 0x05,
            PillDebounceTime = 0x06,
            PillStuckTime = 0x07,
            TubeClearTime = 0x08,
            DoorUnlockTime = 0x09,
            PillGateSolenoidHardTimeOn = 0x0C,
            PillGateSolenoidPWMDutyCycle = 0x0D,
            MaximumFragmentCount = 0x0E
        }

        public enum CellParameter : byte
        {
            // Parameters used by ADC module
            /// The running average reading of the count sensor, scaled to 0..1024 (read-only)
            CountSensorBaseline = 0x00,
            /// The running average reading of the jam sensor, scaled to 0..1024 (read-only)
            JamSensorBaseline = 0x01,
            /// Above this value, the count or jam sensors will be considered to be saturated (max: 1024).
            /// Default: 720
            SensorSaturationThreshold = 0x02,
            /// The debounce time used for the count and jam sensors, measured in samples
            /// (10th's of a ms).  Default 3 tenths of a ms.
            SensorDebounceTime = 0x03,

            // 0x04 Reserved
            // 0x05 Reserved
            // 0x06 Reserved

            // Parameters used by the Port module
            /// The time, in ms, that the air solenoid will be provided with 100% PWM.
            AirSolenoid100PctDelay = 0x07,
            /// The time, in ms, that the pill gate solenoid will be provided with 100% PWM.
            PillGateSolenoid100PctDelay = 0x08,
            /// The duty cycle that the air solenoid will be driven.  Scaled such that 13
            /// will provide 100% PWM.
            AirSolenoidDutyCycle = 0x09,
            /// The duty cycle that the pill gate will be driven.  Scaled such that 250 will
            /// provide 100% PWM.
            DoorLockDutyCycle = 0x0A,
            /// The duty cycle used to drive the door lock.  Scaled such that 250 will provide
            /// 100% PWM.
            DamperDoorDutyCycle = 0x0B,

            // 0x0C Reserved
            // 0x0D Reserved
            // 0x0E Reserved

            // Parameters used by the calibration module
            /// The minimum allowable calibrated collector reading after calibration is complete.  In sensor units
            /// (1024 == 3.3 V)
            MinAllowedCalibratedCollectorReading = 0x0F,
            /// The maximum allowable calibrated collector reading after calibration is complete. In sensor units (1024 == 3.3)
            MaxAllowedCalibratedCollectorReading = 0x10,
            /// The number of calibration jam retries that were made during the last sensor calibration. (read-only)
            TotalCalibrationRetries = 0x11,
            /// The calibrated emitter PWM for the count sensor (read-only), in emitter units (255 == 100%)
            CountEmitterCalibratedDutyCycle = 0x12,
            /// The calibrated emitter PWM for the jam sensor (read-only), in emitter untis (255 == 100%)
            JamEmitterCalibratedDutyCycle = 0x13,
            /// The maximum difference between calibrated emitter PWM for the count and jam sensors during
            /// calibration (default: 6), in emitter units.
            MaximumCalibratedSensorPwmDifference = 0x14,
            /// The amount of time to wait after changing PWM during calibration before re-reading the sensor,
            /// in ms.  Default: 3
            CalibrationStepSettlingTime = 0x15,
            /// The amount of time to allow the pill gate to open before starting the reverse jet during
            /// calibration, in ms.  Also the time to wait for the pill gate to close when recalibrating
            /// the cell.
            PillGateOperatingDelay = 0x16,

            // 0x17 Reserved
            // 0x18 Reserved
            // 0x19 Reserved

            // Parameters used by counting module
            /// The number of pills to count.
            NumberOfPillsToCount = 0x1A,

            // 0x1B Reserved, door unlock time removed as it is not implemented in the cell firmware

            /// The number of pills that were counted (read-only)
            TotalPillsCounted = 0x1C,
            /// The number of active->inactive transistions on the count sensor during forward flow (read-only)
            CountSensorForwardCounts = 0x1D,
            /// The number of active->inactive transistions on the jam sensor during forward flow (read-only)
            JamSensorForwardCounts = 0x1E,
            /// The number of active->inactive transistions on the count sensor during reverse flow (read-only)
            CountSensorReverseCounts = 0x1F,
            /// The number of active->inactive transistions on the jam sensor during reverse flow (read-only)
            JamSensorReverseCounts = 0x20,
            /// If no pills have passed the jam sensor in this amount of time (in ms), the
            /// cell will consider the agitation chamber to be jammed and will initiate a jam clear.
            /// Default: 400 ms
            CellJammedTimeLimit = 0x21,
            /// The first pill after a backjet is allowed this much time to appear at the jam
            /// sensor before the cell is considered to be jammed.  Typically larger than the CellJammedTimeLimit.
            /// Default: 750 ms
            InitialCellJammedTimeLimit = 0x22,
            /// If a pill has presented itself to a sensor, and not passed by this amount of time,
            /// (in ms) then the pill will be considered to be stuck.  Default: 100 ms
            PillStuckTimeLimit = 0x23,
            /// When blowing the backjets to clear a jam, resume forward flow after this time (in ms).  Default: 100 ms
            CellJamClearTime = 0x24,
            /// When blowing the backjets after a count is complete, add forward flow after this 
            /// amount of time (in ms).  Default: 150 ms
            CellClearTubeTime = 0x25,
            /// When blowing out the flapper gate at the end-of-count, terminate all flow and close
            /// the pill gate after this amount of time (in ms).  Default: 100 ms
            CellClearFlapperTime = 0x26,
            /// The total number of jam retries that have occurred during the most recent count. (read-only)
            TotalJamRetries = 0x27,
            /// The number of jam retries that are allowed to occur during a count (default: 7).
            MaximumSerialJamRetries = 0x28,
            /// The amount of time to initially blow the backjet before beginning calibration, in ms.  Default: 200 ms
            CalibrationClearTubeTime = 0x29,
            /// The number of calibration jam retries that may be made during the next sensor calibration
            /// before the cell considers calibration to have failed (default: 6).
            MaximumCalibrationRetries = 0x2A,
            /// The total time consumed during the count, in hundreths of a second (read-only)
            TotalCountTime = 0x2B,
            /// The time between observing the Nth pill at the Jam Sensor and activating the reverse jet in 
            /// yellow-light mode, in tenths of a millisecond.  Default: 30 tenths of a millisecond.
            YellowLightTimeDelay = 0x2C,
            /// If the Nth pill does not make it to the count sensor in this amount of time (in ms), then re-initiate
            /// forward flow.  Default: 100 ms.
            YellowLightTimeAllowance = 0x2D,
            /// After blowing both jets together at the end-of-count, blow the reverse jet alone for this amount
            /// of time.  Default: 50 ms.
            ClearTubeFinalDuration = 0x2E,
            /// At the start (and resumption) of the count, pulse the reverse jet for this amount of time before
            /// initiating forward flow in order to clear out any possible preloaded pills.  Default: 30ms.
            StartCountReverseJetTime = 0x2F,
            /// The amount of time that the cell will allow between continue count commands before it aborts
            /// the count.
            CountingWatchdogTimeout = 0x30,
            // How many 16ths of the baseline is the count sensor threshold
            CountSensorThesholdNumerator = 0x31,
            // How many 16ths of the baseline is the jam sensor threshold
            JamSensorThesholdNumerator = 0x32,
            // The median time for a pill to pass by the count sensor
            CountSensorMedianTravelTime = 0x33,
            //fraction, in 8ths, of the median count sensor travel time that is considered to be a fragment
            FragmentSize = 0x34,
            /// Maximum nuber of fragments permitted before count is aborted
            FragmentLimit = 0x35,
            ///  Number of fragments that were actually counted
            FragmentCount = 0x36,
            // Number of pill gaps with lengths falling into bin 1
            Bin1GapCount = 0x37,
            // Number of pill gaps with lengths falling into bin 2
            Bin2GapCount = 0x38,
            // Length of the N+1 gap time in 100us units.
            OvercountGapTime = 0x39
            // PARAM_END
        }

        public enum CellAction : byte
        {
            NoFlow,
            ReverseFlow,
            ForwardFlow,
            End
        }

        public enum CellStatusBitMask1 : byte
        {
            CountingComplete = 0x01,
            CountingFailed = 0x02,
            CountingAborted = 0x04,
            ErrorFlagsUpdated = 0x10,
            CountingPaused = 0x20
        }

        public enum CellStatusBitMask2 : byte
        {
            Idle = 0x01,
            PreparingCount = 0x02,
            PreparingCountComplete = 0x04,
            PreparingCountFailed = 0x08,
            Calibrating = 0x10,
            CalibrationComplete = 0x20,
            CalibrationFailed = 0x40,
            Counting = 0x80
        }

        /// <summary>
        /// The list of status codes for the cell.  The codes' values form a bitmask: more than one status code may be set at a time in a valid
        /// status code.
        /// </summary>
        [Flags]
        public enum CellStatusCode : ushort
        {
            Idle = 0x0001,
            PreparingCount = 0x0002,
            PreparingCountComplete = 0x0004,
            PreparingCountFailed = 0x0008,
            Calibrating = 0x0010,
            CalibrationComplete = 0x0020,
            CalibrationFailed = 0x0040,
            Counting = 0x0080,
            CountingComplete = 0x0100,
            CountingFailed = 0x0200,
            CountingAborted = 0x0400,
            ErrorFlagsUpdated = 0x0800,
            CountingPaused = 0x1000
        }

        /// <summary>The firmware-level error codes for the cell.</summary>
        /// <remarks>Maintain synchronization between this list and the firmware projects' fault.h header file.</remarks>
        public enum CellErrorCode : byte
        {
            TimeSliceOverrun = 0x01,
            IllegalCommand = 0x02,
            IllegalSubCommand = 0x03,
            I2CRecieveCountError = 0x04,
            I2CCrcError = 0x05,
            FlashProgramProtectionError = 0x06,
            FlashProgramAccessError = 0x07,
            CountSensorCalibrationError = 0x08,
            JamSensorCalibrationError = 0x09,
            PrepareCountError = 0x0A,
            SerialNumberReadError = 0x0C,
            RS232ReceiveFifoOverrun = 0x0D,
            RS232TransmitFifiOverrun = 0x0E,
            RS232ReceiveBufferOverrun = 0x0F,
            RS232NoiseFlag = 0x10,
            RS232FramingError = 0x11,
            RS232ParityError = 0x12,
            RS232CrcError = 0x13,
            I2CMuxBusBusyTimeout = 0x14,
            I2CMuxReceiveMessageTimeout = 0x15,
            I2CCellBusBusyTimeout = 0x16,
            I2CCellReceiveMessageTimeout = 0x17,
            RS232ReceiveMessageTimeout = 0x18,
            FaultRequestError = 0x19,
            WatchdogReset = 0x1A,
            I2CLedBusBusyTimeout = 0x1B,
            I2CLedRxMsgTimeout = 0x1C,
            RS232InvalidPacket = 0x1D,
            JamSensorSaturated = 0x1E,
            CountSensorSaturated = 0x1F,
            CalibrationAgreementError = 0x20,
            /// <summary>
            /// Only those bits of the FaultMask that are set are relevant to the error message (the high two bits indicate the error source)
            /// </summary>
            FaultMask = 0x3F,
            /// <summary>
            /// The bits of the error code that indicate the source of the error
            /// </summary>
            SourceMask = 0xC0,
            SourceCellBoard = 0x40,
            SourceConnectorBoard = 0x80,
            SourceNetworkBoard = 0xC0,
            /// <summary>When an error code is not one of the above, it will be returned as this value when converting from
            /// a List&lt;byte&gt; -> List&lt;CellErrorCode&gt;.  Note that it is also used as the sequence terminator in the 
            /// protocol itself.</summary>
            Unknown = 0xFF
        }

        [Flags]
        public enum CellErrorFlags : ushort
        {
            FaultHistoryUpdated = 0x0001,
            IllegalCommandReceived = 0x0002,
            IllegalSubCommandReceived = 0x0004,
            I2CReceiveCountError = 0x0008,
            I2CReceiveCrcError = 0x0010,
            CountSensorCalibrationError = 0x0020,
            JamSensorCalibrationError = 0x0040,
            CalibrationAgreementError = 0x0080,
            PillStuckInClearTube = 0x0100,
            PillStuckInReverseFlow = 0x0200,
            UnhandledEventInForwardFlow = 0x0400,
            UnhandledEventInReverseFlow = 0x0800,
            UnhandledEventInYellowLight = 0x1000,
            UnhandledEventInClearTube = 0x2000,
            UnhandledEventInIdle = 0x4000,
            UnhandledEventInPrepareCountComplete = 0x8000
        }

        /// <summary>
        /// The list of cause flags for the cell.  Cause flags accompany status flags in status messages. The cause
        /// flags indicate the cause(s) of the status change.
        /// </summary>
        [Flags]
        public enum CellCauseFlags : ushort
        {
            CountSensorSaturated = 0x0001,
            JamSensorSaturated = 0x0002,
            JamRetryLimitExceededInClearTubeState = 0x0004,
            JamRetryLimitExceededInCountingState = 0x0008,
            CalibrationRetryLimitExceeded = 0x0010,
            OverCountDetected = 0x0020,
            FragmentLimitExceeded = 0x0040,
            CountWatchdogExpired = 0x0080,
            AbortCommandReceived = 0x0100,
            PauseCommandReceived = 0x0200
        }

        /// <summary>
        /// Special values for the number of pills that were counted
        /// </summary>
        public enum CellPillsCounted : ushort
        {
            /// <summary>
            /// The value reported by the cell to report an unknown number of pills counted
            /// </summary>
            Unknown = UInt16.MaxValue
        };

        [Flags]
        public enum CellResetStatus : ushort
        {
            None = 0x00,
            // Reserved 0x01
            LowVoltageDetect = 0x02,
            InternalClock = 0x04,
            IllegalAddress = 0x08,
            IllegalOpcode = 0x010,
            WatchdogTimeout = 0x20,
            ExternalResetPin = 0x40,
            PowerOnReset = 0x80
        }

        #endregion

        #region Connector protocol
        public enum CellConnectorCommands : byte
        {
            Ping = 0x01,
            ReportNVMData = 0x02,
            ReportFaults = 0x03,
            SetBITControl = 0x04,
            GetBITStatus = 0x05,
            SetLEDControl = 0x06,
            GetCellPresentStatusFlags = 0x07,
            RESERVED1 = 0x08,
            PerformPeriodicBIT = 0x09,
            ReadHardwareRevision = 0x0A,
            ReadSubnetId = 0x0B,
            RESERVED2 = 0x0C,
            SetDiagnosticsControl = 0x0D,
            GetDiagnosticsStatus = 0x0E,
            GetNetworkIRQStatus = 0x16,
            ClearBITStatus = 0x17,
            ReadProtocolVersion = 0x19,
            SetChuteLedControl = 0x40,
            GetChuteSensorStatus = 0x41,
            GetHopperGateSensorStatus = 0x41,
            SetChuteLockControl = 0x42,
            GetHopperStatus = 0x43,
            SetHopperVibratorControl = 0x44,
            ReadRegister = 0x80,
            SetRegister = 0x81,
            ProgramSubnetWithoutTool = 0x90,
            EraseNVM = 0xFE,
            ProgramSubnetWithTool = 0xFF
        }

        // Keep synchronized with CNConnectorBITStatusResponse.parseBitCodes().
        public enum ConnectorBITCode : ushort
        {
            ProcessorOverTemperature = 1 << 3,
            BITFailed = 1 << 2,
            BITRunning = 1 << 1,
            BITComplete = 1 << 0,
            UnknownFailure = 0xFFFF
        }

        public enum ConnectorLEDColor : byte { Red = 0x00, Blue = 0x01, Green = 0x02 }
        public enum ConnectorLEDState : byte { Solid = 0x00, Off = 0x01, BlinkSlow = 0x02, BlinkFast = 0x03, Default = Off }
        public enum ConnectorPillGateStatusBit : byte { Closed = 1 << 0, Open = 1 << 1 }
        public enum ConnectorGateStatusBit : byte { ClosedSwitch = 1 << 0, OpenSwitch = 1 << 1, VibratorMotor = 1 << 2 }
        public enum ConnectorHopperStatusBit : byte { LowerBridgeSensor = 1 << 0, UpperBridgeSensor = 1 << 1 }
        public enum ConnectorHopperVibratorState : byte { Off = 0, On = 1, Default = Off }
        public enum ConnectorChuteLockState : byte { Locked = 0, Unlocked = 1, Default = Locked }
        public enum ConnectorCellPresentStatusBit : byte { Cell1 = 0x01, Cell2 = 0x02, Cell3 = 0x04, Cell4 = 0x08 }

        #endregion


        #region Members and Properties

        public const UInt8 STX = 0x02;
        public const UInt8 ETX = 0x03;

        protected const UInt8 MinLength = 8;

        public enum msgFormat
        {
            STXIndex = 0,
            NetworkIdIndex = 1,
            SubnetIdIndex = 2,
            NodeIdIndex = 3,
            CommandIdIndex = 4,
            DataLengthIndex = 5,
            DataIndex = 6
        }

        // Message indices
        protected const UInt8 STXIndex = 0;
        protected const UInt8 NetworkIdIndex = 1;
        protected const UInt8 SubnetIdIndex = 2;
        protected const UInt8 NodeIdIndex = 3;
        protected const UInt8 CommandIdIndex = 4;
        protected const UInt8 DataLengthIndex = 5;
        protected const UInt8 DataIndex = 6;
        // Note that the current revision of the cell network interface and communications document
        // states the max data length is 12.  However, several messages are of length 14.
        // 6/6/2007 - jdb
        protected const UInt8 MaxDataLength = 14;

        #endregion
        protected const UInt8 SerialNumberBytes = 5;
        #region Members

        // Message is maintained in a list for convenience of iterating through
        protected List<UInt8> message = new List<UInt8>();
        public UInt8[] Message
        {
            get { return message.ToArray(); }
        }

        private List<ConnectorBITCode> bitErrorCodes;

        public UInt8 NetworkId
        {
            get { return (UInt8)message[NetworkIdIndex]; }
        }

        public UInt8 SubnetId
        {
            get { return (UInt8)message[SubnetIdIndex]; }
        }

        public UInt8 NodeId
        {
            get { return (UInt8)message[NodeIdIndex]; }
        }

        public UInt8 CommandId
        {
            get { return (UInt8)message[CommandIdIndex]; }
        }

        public UInt8 DataLength
        {
            get { return (UInt8)message[DataLengthIndex]; }
        }

        public UInt8[] Data
        {
            // To do - make this more efficient
            get { return message.GetRange(DataIndex, message[DataLengthIndex]).ToArray(); }
        }

        public UInt8 CRC
        {
            get { return (UInt8)message[DataIndex + message[DataLengthIndex]]; }
        }

        public List<ConnectorBITCode> BitErrorCodes
        {
            get
            {
                if (bitErrorCodes == null)
                    parseBitCodesData();
                return bitErrorCodes;
            }
        }

        #endregion Members

        #region Constructors
        public CellNetworkMessageTrace()
            : base()
        {

        }

        public CellNetworkMessageTrace(XPathNavigator trace)
            : base(trace)
        {
            base.DerivedType = this.GetType();
           
            if (this.TraceType.Equals(NSFTraceTags.messageSentTag))
                TagAndData.Add("Sender", "PC");
            else if (this.TraceType.Equals(NSFTraceTags.messageReceivedTag))
                TagAndData.Add("Sender", "Board");
            TagAndData.Add("TraceSource", TagAndData[NSFTraceTags.sourceTag]);

            TagAndData.Remove(NSFTraceTags.sourceTag);

            TraceType = "CellMessaging";
            setMessage(TagAndData[NSFTraceTags.messageTag]);

            //foreach (KeyValuePair<string, string> kvp in parseMessage())
            //    TagAndData.Add(kvp.Key, kvp.Value);
        }

        #endregion Constructors

        #region Methods
        private void setMessage(string msg)
        {

            string[] messageString = msg.Split(' ');
            string dataString = string.Empty;

            foreach (string s in messageString)
                if(s != "")
                    message.Add(UInt8.Parse(s, NumberStyles.HexNumber));
            
            foreach (UInt8 d in Data)
                dataString += d.ToString() + " ";

            TagAndData.Add("Protocol", "");           
            TagAndData.Add("Address", "");
            TagAndData.Add("MessageType", "");

            if(SubnetId == 0)
            {
                TagAndData["Protocol"] = "CellNetworkBoard";
                TagAndData["Address"] = NetworkId.ToString();
                if (TagAndData["Sender"].Equals("Board"))
                {
                    TagAndData["MessageType"] = ((NetworkCommands)CommandId).ToString() + " Response";

                    TagAndData.Add("Data", dataString);
                }
                else if (TagAndData["Sender"].Equals("PC"))
                {
                    TagAndData["MessageType"] = ((NetworkCommands)CommandId).ToString() + " Sent";


                    TagAndData.Add("Data", dataString);
                }

            }
            else if(NodeId == 0)
            {
                TagAndData["Protocol"] = "ConnectorBoard";                
                TagAndData["Address"] = SubnetId.ToString();
                if (TagAndData["Sender"].Equals("Board"))
                {
                    TagAndData["MessageType"] = ((CellConnectorCommands)CommandId).ToString() + " Response";
                    
                    TagAndData.Add("Data", ParseConnectorMessageData((CellConnectorCommands)CommandId));
                }
                else if (TagAndData["Sender"].Equals("PC"))
                {
                    TagAndData["MessageType"] = ((CellConnectorCommands)CommandId).ToString() + " Sent";

                    TagAndData.Add("Data", ParseConnectorMessageData((CellConnectorCommands)CommandId));
                }

            }
            else
            {
                TagAndData["Protocol"] = "CellBoard";                
                TagAndData["Address"] = NodeId.ToString();
                if (TagAndData["Sender"].Equals("Board"))
                {
                    TagAndData["MessageType"] = ((CellCommands)CommandId).ToString() + " Response";
                    TagAndData.Add("Data", ParseCellMessageData((CellCommands)CommandId));                
                }
                else if (TagAndData["Sender"].Equals("PC"))
                {
                    TagAndData["MessageType"] = ((CellCommands)CommandId).ToString() + " Sent";
                    TagAndData.Add("Data", ParseCellMessageData((CellCommands)CommandId));     
                }
            }

          

            //msgId = UInt32.Parse(message[0], NumberStyles.HexNumber);
        }

        #region Cell Board Message Parsing
        public string ParseCellMessageData(CellCommands msg)
        {
            string dataString = string.Empty;

            switch (msg)
            {
                case CellCommands.AbortCount:
					dataString += parseCellCommand();
				break;
                case CellCommands.ActivateSolenoid:
					dataString += parseCellCommand();
				break;
                case CellCommands.CalibrateDetector:
					dataString += parseCellCommand();
				break;
                case CellCommands.CellInsertNotification:
					dataString += parseCellCommand();
				break;
                case CellCommands.ClearBuiltInTest:
					dataString += parseCellCommand();
				break;
                case CellCommands.ClearErrorFlags:
					dataString += parseCellCommand();
				break;
                case CellCommands.ClearFaults:
					dataString += parseCellCommand();
				break;
                case CellCommands.ContinueCount:
					dataString += parseCellCommand();
				break;
                case CellCommands.EraseNonvolatileMemory:
					dataString += parseCellCommand();
				break;
                case CellCommands.GetBuiltInTestStatus:
					dataString += parseCellCommand();
				break;
                case CellCommands.GetCellParam:
					dataString += parseCellCommand();        
                    if (TagAndData["Sender"].Equals("PC"))
                        dataString += (CellCommands)getDataUInt8(5);
                    else
                        dataString += getDataString(6, (byte)(DataLength - 6));
				break;
                case CellCommands.GetCellStatus:
					dataString += parseCellCommand();
                    if (TagAndData["Sender"].Equals("Board"))
                    {
                        //dataString += translateStatus(getDataUInt16(statusWordIndex));
                        dataString += "Status: " + statusCodeToString();
                    }
				break;
                case CellCommands.GetErrorFlags:
					dataString += parseCellCommand();
				break;
                case CellCommands.PauseCount:
					dataString += parseCellCommand();
				break;
                case CellCommands.PerformBuiltInTest:
					dataString += parseCellCommand();
				break;
                case CellCommands.Ping:
					dataString += parseCellCommand();
				break;
                case CellCommands.PrepareCount:
					dataString += parseCellCommand();
                    if (TagAndData["Sender"].Equals("PC"))
                        dataString += "Count: " + getDataUInt16(5);
                    break;
                case CellCommands.ProgramCellSerialNumber:
					dataString += parseCellCommand();
				break;
                case CellCommands.ProgramData:
					dataString += parseCellCommand();
				break;
                case CellCommands.ProgramEnd:
					dataString += parseCellCommand();
				break;
                case CellCommands.PulseSolenoid:
					dataString += parseCellCommand();
				break;
                case CellCommands.ReadDetector:
					dataString += parseCellCommand();
				break;
                case CellCommands.ReadHardwareRevision:
					dataString += parseCellCommand();
                    if (TagAndData["Sender"].Equals("Board"))
                    {
                        getDataUInt8(6).ToString();
                        string rev = getDataString(6, (byte)(DataLength - 6));
                        if (rev[0] == '0')
                        {
                            dataString += (Convert.ToInt32(rev)).ToString();
                        }
                        else if (rev[0] == 'P')
                        {
                            dataString += rev;
                        }
                        else
                        {
                            dataString += "Major: " + getDataUInt8(6);
                            dataString += " - Minor: " + getDataUInt8(7);
                        }
                    }
				break;
                case CellCommands.ReadParameterSequence:
					dataString += parseCellCommand();
                    if (TagAndData["Sender"].Equals("PC"))
                    {
                        if (DataLength - SerialNumberBytes >= 6)
                        {
                            dataString += "Parameter :" + ((CellParameter)getDataUInt8(5)).ToString();
                            dataString += "Value :" + getDataUInt8(6).ToString().ToString();
                            dataString += "Parameter :" + ((CellParameter)getDataUInt8(7)).ToString();
                            dataString += "Value :" + getDataUInt8(8).ToString().ToString();
                            dataString += "Parameter :" + ((CellParameter)getDataUInt8(9)).ToString();
                            dataString += "Value :" + getDataUInt8(10).ToString().ToString();
                        }
                        else if (DataLength - SerialNumberBytes >= 4)
                        {
                            dataString += "Parameter :" + ((CellParameter)getDataUInt8(5)).ToString();
                            dataString += "Value :" + getDataUInt8(6).ToString().ToString();
                            dataString += "Parameter :" + ((CellParameter)getDataUInt8(7)).ToString();
                            dataString += "Value :" + getDataUInt8(8).ToString().ToString();
                        }
                        else
                        {
                            dataString += "Parameter :" + ((CellParameter)getDataUInt8(5)).ToString();
                            dataString += "Value :" + getDataUInt8(6).ToString().ToString();
                        }
                    }
                    else
                    {
                        dataString += getDataString(6, (byte)(DataLength - 6)).ToString();
                    }
				break;
                case CellCommands.ReadProtocolVersion:
					dataString += parseCellCommand();
                    if (TagAndData["Sender"].Equals("Board"))
                    {
                        dataString += "Version: " + getDataUInt8(6).ToString();
                        dataString += " - Age: " + getDataUInt8(7).ToString();
                    }
				break;
                case CellCommands.ReadSerialNumber:
					dataString += "Serial: " + getDataUInt40(6).ToString();
				break;
                case CellCommands.Reboot:
					dataString += parseCellCommand();
				break;
                case CellCommands.ReportFaults:
					dataString += parseCellCommand();
				break;
                case CellCommands.ReportNVMData:
					dataString += parseCellCommand();
                    if (TagAndData["Sender"].Equals("PC"))
                        dataString += "NVM Data: " + (NVMDataFlags)getDataUInt8(5);
                    else
                        dataString += "NVM Data: " + getDataString(6, (byte)(DataLength - 6));
				break;
                case CellCommands.ResumeCount:
					dataString += parseCellCommand();
				break;
                case CellCommands.SetBuiltInTestControl:
					dataString += parseCellCommand();
				break;
                case CellCommands.SetCellParam:
					dataString += parseCellCommand();
                    if (TagAndData["Sender"].Equals("PC"))
                        dataString += "Parameter: " + (NVMDataFlags)getDataUInt8(5);
                    else
                        dataString += getDataString(6, (byte)(DataLength - 6));
				break;
                case CellCommands.SetDetectorEnable:
					dataString += parseCellCommand();
				break;
                case CellCommands.SetDownloadFlag:
					dataString += parseCellCommand();
				break;
                case CellCommands.SetEmitterPwm:
					dataString += parseCellCommand();
				break;
                case CellCommands.SetEndOfSequenceAction:
					dataString += parseCellCommand();
                    if(TagAndData["Sender"].Equals("PC"))
                    {
                        dataString += "Index: " + getDataUInt8(5).ToString();
                        dataString += " - Action: " + ((CellAction)getDataUInt8(6)).ToString();
                        dataString += " - Duration: " + getDataUInt8(7).ToString();
                    }
				break;
                case CellCommands.SetLED:
					dataString += parseCellCommand();
                    if (TagAndData["Sender"].Equals("PC"))
                    {
                        dataString += "Color: " + ((CellLEDColor)getDataUInt8(5)).ToString();
                        dataString += " - State: " + ((CellLEDState)getDataUInt8(6)).ToString();
                    }
				break;
                case CellCommands.StartCount:
					dataString += parseCellCommand();
                    break;
                case CellCommands.WriteParameterSequence:
					dataString += parseCellCommand();
                    if (TagAndData["Sender"].Equals("PC"))
                    {
                        if (DataLength - SerialNumberBytes >= 6)
                        {
                            dataString += "Parameter :" + ((CellParameter)getDataUInt8(5)).ToString();
                            dataString += "Value :" + getDataUInt8(6).ToString().ToString();
                            dataString += "Parameter :" + ((CellParameter)getDataUInt8(7)).ToString();
                            dataString += "Value :" + getDataUInt8(8).ToString().ToString();
                            dataString += "Parameter :" + ((CellParameter)getDataUInt8(9)).ToString();
                            dataString += "Value :" + getDataUInt8(10).ToString().ToString();
                        }
                        else if (DataLength - SerialNumberBytes >= 4)
                        {
                            dataString += "Parameter :" + ((CellParameter)getDataUInt8(5)).ToString();
                            dataString += "Value :" + getDataUInt8(6).ToString().ToString();
                            dataString += "Parameter :" + ((CellParameter)getDataUInt8(7)).ToString();
                            dataString += "Value :" + getDataUInt8(8).ToString().ToString();
                        }
                        else
                        {
                            dataString += "Parameter :" + ((CellParameter)getDataUInt8(5)).ToString();
                            dataString += "Value :" + getDataUInt8(6).ToString().ToString();
                        }

                    }
                    else
                    {
                        dataString += getDataString(6, (byte)(DataLength - 6));
                    }
				break;
                default:
					dataString += parseCellCommand();
				break;

            }
            return dataString;
        }

        public string parseCellCommand()
        {
            string parsedMsg = string.Empty;
            parsedMsg += "Serial: " + getCellSerialNumber() + "  ";
       
            return parsedMsg;
        }

        private string getCellSerialNumber()
        {
            if (TagAndData["Sender"].Equals("Board"))
                return getDataUInt40(1).ToString();
            else
                return getDataUInt40(0).ToString();
        }

        #region Constants

        const int statusWordIndex = 6;
        const int statusWordMSB = statusWordIndex;
        const int statusWordLSB = statusWordMSB + 1;

        const int causeWordIndex = 8;
        const int causeWordMSB = causeWordIndex;
        const int causeWordLSB = causeWordMSB + 1;

        #endregion Constants
        #region Properties

        public List<CellStatusCode> StatusCode
        {
            get
            {
                return translateStatus(getDataUInt16(statusWordIndex));
            }
        }

        public List<CellCauseFlags> CauseCode
        {
            get
            {
                return translateCause(getDataUInt16(causeWordIndex));
            }
        }

        public bool CellCountingPaused
        {
            get { return getDataBit(statusWordMSB, 4); }
        }

        public bool CellErrorFlagsUpdated
        {
            get { return getDataBit(statusWordMSB, 3); }
        }

        public bool CellCountingAborted
        {
            get { return getDataBit(statusWordMSB, 2); }
        }

        public bool CellCountingFailed
        {
            get { return getDataBit(statusWordMSB, 1); }
        }

        public bool CellCountingComplete
        {
            get { return getDataBit(statusWordMSB, 0); }
        }

        public bool CellCounting
        {
            get { return getDataBit(statusWordLSB, 7); }
        }

        public bool CellCalibrationFailed
        {
            get { return getDataBit(statusWordLSB, 6); }
        }

        public bool CellCalibrationComplete
        {
            get { return getDataBit(statusWordLSB, 5); }
        }

        public bool CellIsCalibrating
        {
            get { return getDataBit(statusWordLSB, 4); }
        }

        public bool CellPrepareCountFailed
        {
            get { return getDataBit(statusWordLSB, 3); }
        }

        public bool CellPrepareCountComplete
        {
            get { return getDataBit(statusWordLSB, 2); }
        }

        public bool CellIsPreparingCount
        {
            get { return getDataBit(statusWordLSB, 1); }
        }

        public bool CellIsIdle
        {
            get { return getDataBit(statusWordLSB, 0); }
        }

        public bool CellPauseCommandReceived
        {
            get { return getDataBit(causeWordMSB, 1); }
        }

        public bool CellAbortCommandReceived
        {
            get { return getDataBit(causeWordMSB, 0); }
        }

        public bool CellCountWatchdogExpired
        {
            get { return getDataBit(causeWordLSB, 7); }
        }

        public bool CellFragmentLimitExceeded
        {
            get { return getDataBit(causeWordLSB, 6); }
        }

        public bool CellOvercountDetected
        {
            get { return getDataBit(causeWordLSB, 5); }
        }

        public bool CellCalibrationRetryLimitExceeded
        {
            get { return getDataBit(causeWordLSB, 4); }
        }

        public bool CellJamRetryLimitInCountingState
        {
            get { return getDataBit(causeWordLSB, 3); }
        }

        public bool CellJamRetryLimitInClearTubeState
        {
            get { return getDataBit(causeWordLSB, 2); }
        }

        public bool CellJamSensorSaturated
        {
            get { return getDataBit(causeWordLSB, 1); }
        }

        public bool CellCountSensorSaturated
        {
            get { return getDataBit(causeWordLSB, 0); }
        }

        #endregion Properties

        private List<CellStatusCode> translateStatus(UInt16 code)
        {
            List<CellStatusCode> ret = new List<CellStatusCode>();
            // Mask out the irrelevant bits.
            // The extra casting is needed because typeof(code) == int rather than uint.
            foreach (Object value in Enum.GetValues(typeof(CellStatusCode)))
            {

                if (((ushort)(CellStatusCode)value & code) == (ushort)(CellStatusCode)value)
                    ret.Add((CellStatusCode)value);
            }
            return ret;
        }

        private List<CellCauseFlags> translateCause(UInt16 code)
        {
            List<CellCauseFlags> ret = new List<CellCauseFlags>();
            // Mask out the irrelevant bits.
            // The extra casting is needed because typeof(code) == int rather than uint.
            foreach (Object value in Enum.GetValues(typeof(CellCauseFlags)))
            {
                if (((ushort)(CellCauseFlags)value & code) == (ushort)(CellCauseFlags)value)
                    ret.Add((CellCauseFlags)value);
            }
            return ret;
        }

        public String statusCodeToString()
        {
            return ((CellStatusCode)getDataUInt16(statusWordIndex)).ToString();
        }

        public String causeCodeToString()
        {
            return ((CellCauseFlags)getDataUInt16(causeWordIndex)).ToString();
        }

        public string causeCodeToErrorStatus()
        {
            return causeCodeToErrorStatus(null);
        }

        public string causeCodeToErrorStatus(string errorStatus)
        {
            if (CellCountSensorSaturated)
            {
                errorStatus = buildCauseChain(errorStatus, CellNetworkErrorCode.CellCountSensorSaturated);
            }

            if (CellJamSensorSaturated)
            {
                errorStatus = buildCauseChain(errorStatus, CellNetworkErrorCode.CellJamSensorSaturated);
            }

            if (CellJamRetryLimitInClearTubeState)
            {
                errorStatus = buildCauseChain(errorStatus, CellNetworkErrorCode.CellJamRetryLimitExceededInClearTube);
            }

            if (CellJamRetryLimitInCountingState)
            {
                errorStatus = buildCauseChain(errorStatus, CellNetworkErrorCode.CellJamRetryLimitExceededInCounting);
            }

            if (CellCalibrationRetryLimitExceeded)
            {
                errorStatus = buildCauseChain(errorStatus, CellNetworkErrorCode.CellCalibrationRetryLimitExceeded);
            }

            if (CellOvercountDetected)
            {
                errorStatus = buildCauseChain(errorStatus, CellNetworkErrorCode.CellOvercountDetected);
            }

            if (CellFragmentLimitExceeded)
            {
                errorStatus = buildCauseChain(errorStatus, CellNetworkErrorCode.CellFragmentLimitExceeded);
            }

            if (CellCountWatchdogExpired)
            {
                errorStatus = buildCauseChain(errorStatus, CellNetworkErrorCode.CellCountWatchdogExpired);
            }

            if (CellAbortCommandReceived)
            {
                errorStatus = buildCauseChain(errorStatus, CellNetworkErrorCode.CellAbortCommandReceived);
            }

            if (CellPauseCommandReceived)
            {
                errorStatus = buildCauseChain(errorStatus, CellNetworkErrorCode.CellPauseCommandReceived);
            }

            return errorStatus;
        }

        private string buildCauseChain(string status, CellNetworkErrorCode cause)
        {
            if (status == null)
            {
                status = cause.ToString();
            }
            else
            {
                status += cause.ToString();
            }

            return status;
        }
        #endregion Cell Board Message Parsing

        #region Connector Board Message Parsing
        public string ParseConnectorMessageData(CellConnectorCommands msg)
        {
            string dataString = string.Empty;

            switch (msg)
            {
                case CellConnectorCommands.GetHopperStatus:
                    dataString = parseGetChuteSensorStatusData();
                    break;
                case CellConnectorCommands.GetHopperGateSensorStatus:
                    dataString = parseGetHopperGateSensorStatusData();
                    break;
                case CellConnectorCommands.GetCellPresentStatusFlags:
                    dataString = parseGetCellPresentStatusFlagsData();
                    break;
                case CellConnectorCommands.GetBITStatus:
                    dataString = parseBitCodesData();
                    break;
                case CellConnectorCommands.ClearBITStatus:
                    foreach (UInt8 d in Data)
                        dataString += d.ToString() + " ";
                    break;
                case CellConnectorCommands.EraseNVM:
                    foreach (UInt8 d in Data)
                        dataString += d.ToString() + " ";
                    break;
                //case CellConnectorCommands.GetChuteSensorStatus:
                //    foreach (UInt8 d in Data)
                //        dataString += d.ToString() + " ";
                //    break;
                case CellConnectorCommands.GetDiagnosticsStatus:
                    foreach (UInt8 d in Data)
                        dataString += d.ToString() + " ";
                    break;
                case CellConnectorCommands.GetNetworkIRQStatus:
                    foreach (UInt8 d in Data)
                        dataString += d.ToString() + " ";
                    break;
                case CellConnectorCommands.PerformPeriodicBIT:
                    foreach (UInt8 d in Data)
                        dataString += d.ToString() + " ";
                    break;
                case CellConnectorCommands.Ping:
                    foreach (UInt8 d in Data)
                        dataString += d.ToString() + " ";
                    break;
                case CellConnectorCommands.ProgramSubnetWithoutTool:
                    foreach (UInt8 d in Data)
                        dataString += d.ToString() + " ";
                    break;
                case CellConnectorCommands.ProgramSubnetWithTool:
                    foreach (UInt8 d in Data)
                        dataString += d.ToString() + " ";
                    break;
                case CellConnectorCommands.ReadHardwareRevision:
                    foreach (UInt8 d in Data)
                        dataString += d.ToString() + " ";
                    break;
                case CellConnectorCommands.ReadProtocolVersion:
                    foreach (UInt8 d in Data)
                        dataString += d.ToString() + " ";
                    break;
                case CellConnectorCommands.ReadRegister:
                    foreach (UInt8 d in Data)
                        dataString += d.ToString() + " ";
                    break;
                case CellConnectorCommands.ReadSubnetId:
                    foreach (UInt8 d in Data)
                        dataString += d.ToString() + " ";
                    break;
                case CellConnectorCommands.ReportFaults:
                    foreach (UInt8 d in Data)
                        dataString += d.ToString() + " ";
                    break;
                case CellConnectorCommands.ReportNVMData:
                    foreach (UInt8 d in Data)
                        dataString += d.ToString() + " ";
                    break;
                case CellConnectorCommands.SetBITControl:
                    foreach (UInt8 d in Data)
                        dataString += d.ToString() + " ";
                    break;
                case CellConnectorCommands.SetChuteLedControl:
                    foreach (UInt8 d in Data)
                        dataString += d.ToString() + " ";
                    break;
                case CellConnectorCommands.SetChuteLockControl:
                    foreach (UInt8 d in Data)
                        dataString += d.ToString() + " ";
                    break;
                case CellConnectorCommands.SetDiagnosticsControl:
                    foreach (UInt8 d in Data)
                        dataString += d.ToString() + " ";
                    break;
                case CellConnectorCommands.SetHopperVibratorControl:
                    foreach (UInt8 d in Data)
                        dataString += d.ToString() + " ";
                    break;
                case CellConnectorCommands.SetLEDControl:
                    foreach (UInt8 d in Data)
                        dataString += d.ToString() + " ";
                    break;
                case CellConnectorCommands.SetRegister:
                    foreach (UInt8 d in Data)
                        dataString += d.ToString() + " ";
                    break;
                case CellConnectorCommands.RESERVED1:
                    foreach (UInt8 d in Data)
                        dataString += d.ToString() + " ";
                    break;
                case CellConnectorCommands.RESERVED2:
                    foreach (UInt8 d in Data)
                        dataString += d.ToString() + " ";
                    break;
                default:
                    foreach (UInt8 d in Data)
                        dataString += d.ToString() + " ";
                    break;

            }
            return dataString;
        }

        public string parseGetHopperGateSensorStatusData()
        {
            string parsedMsg = string.Empty;

            if (TagAndData["Sender"].Equals("PC"))
                return "Chute: " + getDataUInt8(0).ToString();
            parsedMsg += "Chute: " + getDataUInt8(1).ToString();
            parsedMsg += " - VibrationMotor: " + ((getDataUInt8(2) & (byte)ConnectorGateStatusBit.VibratorMotor) != 0 ? "On" : "Off");
            parsedMsg += " - LowerGateSensor: " + ((getDataUInt8(2) & (byte)ConnectorGateStatusBit.ClosedSwitch) != 0 ? "Closed" : "NotClosed");
            parsedMsg += " - UpperGateSensor: " + ((getDataUInt8(2) & (byte)ConnectorGateStatusBit.OpenSwitch) != 0 ? "Open" : "NotOpen");
            return parsedMsg;
        }

        public string parseGetChuteSensorStatusData()
        {
            string parsedMsg = string.Empty;
            if (TagAndData["Sender"].Equals("PC"))
                return "Chute: " + getDataUInt8(0).ToString();
            parsedMsg += "Chute: " + getDataUInt8(1).ToString();
            parsedMsg += " - LowerBridgeSensor: " + ((getDataUInt8(2) & (byte)ConnectorHopperStatusBit.UpperBridgeSensor) != 0 ? "Active" : "Inactive");
            parsedMsg += " - UpperBridgeSensor: " + ((getDataUInt8(2) & (byte)ConnectorHopperStatusBit.LowerBridgeSensor) != 0 ? "Active" : "Inactive");
            return parsedMsg;
        }

        public string parseGetCellPresentStatusFlagsData()
        {
            string parsedMsg = string.Empty;
            parsedMsg += ConnectorCellPresentStatusBit.Cell1.ToString() + ": " + ((getDataUInt8(1) & (byte)ConnectorCellPresentStatusBit.Cell1) != 0 ? "Present" : "Absent");
            parsedMsg += " - " + ConnectorCellPresentStatusBit.Cell2.ToString() + ": " + ((getDataUInt8(1) & (byte)ConnectorCellPresentStatusBit.Cell2) != 0 ? "Present" : "Absent");
            parsedMsg += " - " + ConnectorCellPresentStatusBit.Cell3.ToString() + ": " + ((getDataUInt8(1) & (byte)ConnectorCellPresentStatusBit.Cell3) != 0 ? "Present" : "Absent");
            parsedMsg += " - " + ConnectorCellPresentStatusBit.Cell4.ToString() + ": " + ((getDataUInt8(1) & (byte)ConnectorCellPresentStatusBit.Cell4) != 0 ? "Present" : "Absent");
            return parsedMsg;
        }
         
        private string parseBitCodesData()
        {
            string parsedMsg = string.Empty;
            List<ConnectorBITCode> codes = new List<ConnectorBITCode>();
            UInt16 statusWord = getDataUInt16(1);
            ConnectorBITCode[] bitCodeBits = { 
                ConnectorBITCode.BITComplete, 
                ConnectorBITCode.BITFailed, 
                ConnectorBITCode.BITRunning, 
                ConnectorBITCode.ProcessorOverTemperature 
            };
            foreach (ConnectorBITCode _bitCode in bitCodeBits)
            {
                ushort bitCode = (ushort)_bitCode;
                if ((statusWord & bitCode) == bitCode)
                {
                    codes.Add((ConnectorBITCode)bitCode);
                    statusWord &= (ushort)~bitCode;
                    parsedMsg += _bitCode.ToString() + " ";
                }
            }

            if (statusWord != 0)
            {
                codes.Add(ConnectorBITCode.UnknownFailure);
                parsedMsg += ConnectorBITCode.UnknownFailure.ToString() + " ";
            }

            bitErrorCodes = codes;
            return parsedMsg;
        }
        #endregion Connector Board Message Parsing






















        public bool CommandSuccessFlag
        {
            get { return getDataBool(0); }
        }

        protected void checkRange(UInt8 index, int width)
        {
            //if (width + index > DataLength)
            //    throw new ArgumentOutOfRangeException("Index out of range");
        }

        protected bool getDataBool(UInt8 index)
        {
            checkRange(index, sizeof(UInt8));
            return message[DataIndex + index] != 0 ? true : false;
        }

        protected bool getDataBit(UInt8 dataByte, UInt8 dataBit)
        {
            checkRange(dataByte, sizeof(UInt8));
            return (message[DataIndex + dataByte] & (1 << dataBit)) != 0;
        }

        protected Int8 getDataInt8(UInt8 index)
        {
            checkRange(index, sizeof(Int8));
            return (Int8)message[DataIndex + index];
        }

        protected UInt8 getDataUInt8(UInt8 index)
        {
            checkRange(index, sizeof(UInt8));
            return message[DataIndex + index];
        }

        protected Int16 getDataInt16(UInt8 index)
        {
            checkRange(index, sizeof(Int16));
            return (Int16)((UInt16)message[DataIndex + index] << 8
                | (UInt16)message[DataIndex + index + 1]);
        }

        protected UInt16 getDataUInt16(UInt8 index)
        {
            checkRange(index, sizeof(UInt16));
            return (UInt16)((UInt16)message[DataIndex + index] << 8
                | (UInt16)message[DataIndex + index + 1]);
        }

        protected Int32 getDataInt32(UInt8 index)
        {
            checkRange(index, sizeof(Int32));
            return (Int32)((UInt32)message[DataIndex + index + 3]
                | (UInt32)message[DataIndex + index + 2] << 8
                | (UInt32)message[DataIndex + index + 1] << 16
                | (UInt32)message[DataIndex + index + 0] << 24);
        }

        protected UInt32 getDataUInt32(UInt8 index)
        {
            checkRange(index, sizeof(UInt32));
            return (UInt32)((UInt32)message[DataIndex + index + 3]
                | (UInt32)message[DataIndex + index + 2] << 8
                | (UInt32)message[DataIndex + index + 1] << 16
                | (UInt32)message[DataIndex + index + 0] << 24);
        }

        protected ulong getDataUInt40(UInt8 index)
        {
            checkRange(index, 5);
            return (ulong)((ulong)message[DataIndex + index + 4]
                | (ulong)message[DataIndex + index + 3] << 8
                | (ulong)message[DataIndex + index + 2] << 16
                | (ulong)message[DataIndex + index + 1] << 24
                | (ulong)message[DataIndex + index + 0] << 32);
        }

        protected string getDataString(UInt8 index, UInt8 length)
        {
            checkRange(index, length);
            byte[] text = new byte[length];
            message.CopyTo(DataIndex + index, text, 0, length);

            ASCIIEncoding ascii = new ASCIIEncoding();
            return new String(ascii.GetChars(text));
        }

        public static UInt8[] serializeCellSerialNumber(ulong serial)
        {
            return new UInt8[] {
                (UInt8)((serial >> 32) & 0xFF),
                (UInt8)((serial >> 24) & 0xFF),
                (UInt8)((serial >> 16) & 0xFF),
                (UInt8)((serial >> 8) & 0xFF),
                (UInt8)((serial >> 0) & 0xFF)
            };
        }

        public override void AddListViewItem(ListView lv)
        {
            DateTime nT;
            ListViewItem item = new ListViewItem();
            ListViewItem.ListViewSubItem subItem = new ListViewItem.ListViewSubItem();
            ListView.ColumnHeaderCollection lvCol = lv.Columns;

            if (!lv.Columns.ContainsKey("Sender"))
            {
                lv.Columns.Add("Sender", "Sender");
                item.SubItems.Add("");
            }

            while (item.SubItems.Count < lvCol.Count)
                item.SubItems.Add("");

             if (lvCol[0].Text == "TimeStamp")
                item.SubItems[0].Text = traceTimeStamp.ToString();
            else
            {
                nT = TraceFileData.TimeSyncDate.AddMilliseconds(traceTimeStamp - TraceFileData.TimeSyncMilliSec);
                subItem.Text = nT.ToShortDateString();
                item.SubItems[0].Text = nT.ToShortDateString();
                item.SubItems[1].Text = nT.ToString("hh:mm:ss.fff");
            }
             item.SubItems[2].Text = TagAndData["Sender"];

            foreach (KeyValuePair<string, string> d in TagAndData)
            {
                if (d.Key != NSFTraceTags.messageTag && d.Key != "TraceSource" && d.Key != "Sender")
                {
                    if (!lv.Columns.ContainsKey(d.Key))
                    {
                        lv.Columns.Add(d.Key, d.Key);
                        item.SubItems.Add("");
                    }

                    item.SubItems[lv.Columns[d.Key].Index].Text = d.Value;
                }
            }

            if (!lv.Columns.ContainsKey(NSFTraceTags.messageTag))
            {
                lv.Columns.Add(NSFTraceTags.messageTag, NSFTraceTags.messageTag);
                item.SubItems.Add("");
            }
            if (!lv.Columns.ContainsKey("TraceSource"))
            {
                lv.Columns.Add("TraceSource", "TraceSource");
                item.SubItems.Add("");
            }
             
            int index = lvCol[NSFTraceTags.messageTag].Index;
            item.SubItems[index].Text = TagAndData[NSFTraceTags.messageTag];

            index = lvCol["TraceSource"].Index;
            item.SubItems[index].Text = TagAndData["TraceSource"];

            lv.Items.Add(item);
        }
        #endregion Methods
    }
}
