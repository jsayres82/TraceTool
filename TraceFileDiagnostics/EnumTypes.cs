using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraceFileReader.EnumTypes
{
    public enum MachineModels
    {
        Unknown,
        Max = 3,
        Mini = 4,
        Express = 5,
        CalibrationStation = 6,
        MaxPlus = 10,
        Uber = 11,
        MaxComplete = 12,
        MaxChute = 13,
        ExpressCountAhead = 14
    }


    public enum SubsystemType
    {
        CapFeeder,
        Capper,
        DropOff,
        Hood,
        Labeler,
        Robot,
        System,
        VialFeeder,
        Holdback,
        Camera,
        Hopper,
        Chute,
        Cell,
        CANBus,
        CellNetwork
    }

    public enum DeviceType
    {
        DropOffBinAnalogInput,
        DropOffBinAnalogOutput,
        DropOffBinDigitalInput,
        DropOffBinDigitalOutput,
        DropOffBinBoard,
        ServoAnalogInput,
        ServoAnalogOutput,
        ServoDigitalInput,
        ServoDigitalOutput,
        ServoMotor,
        CellNetworkBoard,
        CellConnectorBoard,
        CellBoard
    }
         
    public enum MotorDataType
    {
        Undefined,
        ActualPosition,
        AverageStagingCurrent,
        AverageStagingTime,
        CommandedPosition,
        MaxActualPositionError,
        MaxActualAbsolutePositionError,
        MoveAverageCurrent,
        MoveMaxPositionError,            
        TargetPosition,
        ControlWord,
        ErrorCode,
        Move,
        StatusWord
    }

    public enum MotorAxisType
    {
        Position,
        Current,
        Time,
        Error
    }

    public enum LinearMotorType
    {
        RobotX,
        RobotZ,
        RobotGripper
    }

    public enum AngularMotorType
    {
        LeftCapFeeder,
        RightCapFeeder,
        LeftVialFeeder,
        RightVialFeeder,
        LabelerDriveRoller,
        CapperSpinner,
        LabelerIndexWheel
    }

    public enum SensorStateType
    {
        Undefined,
        Inactive,
        Active,
        DeadBand
    }

}
