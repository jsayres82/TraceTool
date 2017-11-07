// © Parata Systems, LLC 2008 
// All rights reserved.

using System;
using Int8 = System.SByte;
using UInt8 = System.Byte;

namespace ServiceTool
{
    public enum CANMessageType { Standard = 1, Extended };

    /// <summary>
    /// The CANMessage class describes a CAN message packet, which consists of a message id and up to 8 bytes of data.
    /// </summary>
    /// <remarks>See the CAN spec for details.</remarks>
    public class CANMessage : EventArgs
    {
        #region Members

        private CANMessageType type;
        public CANMessageType Type
        {
            get { return type; }
            set { type = value; }
        }

        private UInt32 id;
        public UInt32 Id
        {
            get { return id; }
            set { id = value; }
        }

        private UInt8 dataLength;
        public UInt8 DataLength
        {
            get { return dataLength; }
        }

        private UInt8[] data = new UInt8[8];
        public UInt8[] Data
        {
            get { return (UInt8[])data.Clone(); }
            set
            {
                if (value.Length > 8)
                {
                    throw new Exception("CANMessage data length too long");
                }
                value.CopyTo(this.data, 0);
                dataLength = (UInt8)data.Length;
            }
        }

        #endregion

        #region Constructors

        public CANMessage(UInt32 id, UInt8 dataLength, params UInt8[] data)
        {
            construct(CANMessageType.Standard, id, dataLength, data);
        }

        public CANMessage(UInt32 id, UInt8 dataLength, UInt64 data)
        {
            construct(CANMessageType.Standard, id, dataLength, data);
        }

        public CANMessage(CANMessageType type, UInt32 id, UInt8 dataLength, params UInt8[] data)
        {
            construct(type, id, dataLength, data);
        }

        public CANMessage(CANMessageType type, UInt32 id, UInt8 dataLength, UInt64 data)
        {
            construct(type, id, dataLength, data);
        }

        public CANMessage(CANMessage message)
        {
            construct(message.type, message.id, message.dataLength, message.data);
        }

        protected void construct(CANMessageType type, UInt32 id, UInt8 dataLength, params UInt8[] data)
        {
            if (dataLength > 8)
            {
                throw new Exception("CANMessage data length too long");
            }

            this.type = type;
            this.id = id;
            this.dataLength = dataLength;
            data.CopyTo(this.data, 0);
        }

        protected void construct(CANMessageType type, UInt32 id, UInt8 dataLength, UInt64 data)
        {
            UInt8[] messageData = new UInt8[8];
            messageData[7] = (UInt8)((data >> 56) & 0xFF);
            messageData[6] = (UInt8)((data >> 48) & 0xFF);
            messageData[5] = (UInt8)((data >> 40) & 0xFF);
            messageData[4] = (UInt8)((data >> 32) & 0xFF);
            messageData[3] = (UInt8)((data >> 24) & 0xFF);
            messageData[2] = (UInt8)((data >> 16) & 0xFF);
            messageData[1] = (UInt8)((data >> 8) & 0xFF);
            messageData[0] = (UInt8)(data & 0xFF);

            construct(type, id, dataLength, messageData);
        }

        #endregion

        #region Methods

        public CANMessage copy()
        {
            return new CANMessage(this);
        }

        public Int8 getDataInt8(UInt32 index)
        {
            if (index > 7)
            {
                throw new Exception("CANMessage data index out of range");
            }
            return (Int8)(data[index]);
        }

        public UInt8 getDataUInt8(UInt32 index)
        {
            if (index > 7)
            {
                throw new Exception("CANMessage data index out of range");
            }
            return (UInt8)(data[index]);
        }

        public Int16 getDataInt16(UInt32 index)
        {
            if (index > 6)
            {
                throw new Exception("CANMessage data index out of range");
            }
            return (Int16)(data[index] + ((UInt16)data[index + 1] << 8));
        }

        public UInt16 getDataUInt16(UInt32 index)
        {
            if (index > 6)
            {
                throw new Exception("CANMessage data index out of range");
            }
            return (UInt16)(data[index] + ((UInt16)data[index + 1] << 8));
        }

        public Int32 getDataInt32(UInt32 index)
        {
            if (index > 4)
            {
                throw new Exception("CANMessage data index out of range");
            }
            return (Int32)(data[index] + ((UInt32)data[index + 1] << 8) + ((UInt32)data[index + 2] << 16) + ((UInt32)data[index + 3] << 24));
        }

        public UInt32 getDataUInt32(UInt32 index)
        {
            if (index > 4)
            {
                throw new Exception("CANMessage data index out of range");
            }
            return (UInt32)(data[index] + ((UInt32)data[index + 1] << 8) + ((UInt32)data[index + 2] << 16) + ((UInt32)data[index + 3] << 24));
        }

        public void setData(UInt32 index, UInt8 data)
        {
            if (index > 7)
            {
                throw new Exception("CANMessage data index out of range");
            }
            this.data[index] = data;
        }

        #endregion
    }
}
