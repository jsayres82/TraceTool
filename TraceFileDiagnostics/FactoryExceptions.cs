// © Parata Systems, LLC 2008 
// All rights reserved.

using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace ServiceTool
{
    [Serializable]
    public class MissingConfigurationParameterException : ApplicationException
    {
        public MissingConfigurationParameterException(String parentName, String parameterName)
            : base(String.Format("{0} missing configuration parameter {1}", parentName, parameterName)) { }

        // Constructor to support deserialization of serialized exceptions passed across remote boundaries.
        protected MissingConfigurationParameterException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }

    [Serializable]
    public class MissingComponentException : ApplicationException
    {
        public MissingComponentException(String componentName)
            : base(String.Format("The component '{0}' does not exist in the system.", componentName)) { }

        // Constructor to support deserialization of serialized exceptions passed across remote boundaries.
        protected MissingComponentException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }

    public class ConfigurationException : ApplicationException
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public ConfigurationException()
            : base() { }

        /// <summary>
        /// Initializes with a specified error message.
        /// </summary>
        /// <param name="message">A message that describes the error.</param>
        public ConfigurationException(string message)
            : base(message) { }

        /// <summary>
        /// Initializes with a specified error 
        /// message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">
        /// The error message that explains the reason for the exception.
        /// </param>
        /// <param name="exception">
        /// The exception that is the cause of the current exception. 
        /// If the innerException parameter is not a null reference, the current exception 
        /// is raised in a catch block that handles the inner exception.
        /// </param>
        public ConfigurationException(string message, Exception exception)
            : base(message, exception) { }

        /// <summary>
        /// Initializes with serialized data.
        /// </summary>
        /// <param name="info">The object that holds the serialized object data.</param>
        /// <param name="context">The contextual information about the source or destination.</param>
        protected ConfigurationException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}