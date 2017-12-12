using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraceFileReader
{
    /// <summary>
    /// Symbolic constants that represent the Tag value to assign a TreeNode.
    /// </summary>
    internal static class TreeNodeTag
    {
        internal static readonly string CANOpenNode = "CANOpenNode";
        internal static readonly string CANSIODigitalInput = "CANSIODigitalInput";
        internal static readonly string CANSIODigitalOutput = "CANSIODigitalOutput";
        internal static readonly string CANSIOServoMotor = "CANSIOServoMotor";
        internal static readonly string CANSIOAnalogInput = "CANSIOAnalogInput";
        internal static readonly string CANSIOAnalogOutput = "CANSIOAnalogOutput";
        internal static readonly string LockingSolenoid = "LockingSolenoid";

        internal static readonly string ServoMotor = "ServoMotor";
        internal static readonly string ServoMotorPositionControlParameters = "ServoMotorPositionControlParameters";
        internal static readonly string ServoMotorCurrentControlParameters = "ServoMotorCurrentControlParameters";        
        internal static readonly string ServoMotorTrajectoryControlParameters = "ServoMotorTrajectoryControlParameters";

        internal static readonly string ServoMotorVelocityMove = "ServoMotorVelocityMove";
        internal static readonly string ServoMotorRelativePositionMove = "ServoMotorRelativePositionMove";
        internal static readonly string ServoMotorAbsolutePositionMove = "ServoMotorAbsolutePositionMove";
    }
}
