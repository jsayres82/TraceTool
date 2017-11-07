using System;
using System.Xml.XPath;

namespace ServiceTool
{
    /// <summary>
    /// Class used to fully identify a type, using string representations of assembly name, namespace, and type name.
    /// </summary>
    public sealed class TypeIdentifier
    {
        #region Properties

        /// <summary>
        /// The name of the assembly in which the type can be found.
        /// </summary>
        public String AssemblyName { get; private set; }
        /// <summary>
        /// The namespace of the type.
        /// </summary>
        public String TypeNamespace { get; private set; }
        /// <summary>
        /// The name of the type.
        /// </summary>
        public String TypeName { get; private set; }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Creates an identifier for a particular type.
        /// </summary>
        /// <param name="assemblyName">The name of the assembly in which the type can be found.</param>
        /// <param name="typeNamespace">The namespace of the type.</param>
        /// <param name="typeName">The name of the type.</param>
        public TypeIdentifier(String assemblyName, String typeNamespace, String typeName)
        {
            AssemblyName = assemblyName;
            TypeNamespace = typeNamespace;
            TypeName = typeName;
        }

        /// <summary>
        /// Creates an identifier for a particular type using xml.  See remarks for convention.
        /// </summary>
        /// <param name="xml">The element that represents the TypeIdentifier.</param>
        /// <remarks>
        /// TypeName is specified by the value of the element passed in.
        /// TypeNamespace is specified by the required Namespace attribute in the xml element.
        /// AssemblyName is specified by the required Assembly attribute in the xml element.
        /// 
        /// A typical element in xml looks like this:
        /// <!--
        /// <Type assembly="objectAssembly" namespace="objectNamespace">ObjectTypeName</Type>
        ///  -->
        /// </remarks>
        public TypeIdentifier(XPathNavigator xml)
        {
            TypeName = xml.Value;

            TypeNamespace = xml.GetAttribute("namespace", String.Empty);
            if (String.IsNullOrEmpty(TypeNamespace))
            {
                throw new Exception(@"TypeIdentifier missing 'namespace' attribute: " + xml.Value);
            }
            AssemblyName = xml.GetAttribute("assembly", String.Empty);

            if (String.IsNullOrEmpty(AssemblyName))
            {
                throw new Exception(@"TypeIdentifier missing 'assembly' attribute: " + xml.Value);
            }
        }

        #endregion Constructors
    }
}
