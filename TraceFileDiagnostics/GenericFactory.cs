// © Parata Systems, LLC 2008 
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Reflection;
using Parata.Controls.CANBus;
using Parata.Controls.Breakaway;
using Parata.Controls;
using Parata.Factory;
using Parata.Controls.Enumerations;
using Parata.Enumerations;

namespace ServiceTool
{
    /// <summary>
    /// Acts as the basic implementation for Factory classes used in the Data library.
    /// We need to create instances based on configuration info.
    /// Since there's much common code for doing Reflection-based activation 
    /// keep that code in one central place.
    /// </summary>
    public sealed class GenericFactory
    {
        #region Constructors

        /// <summary>
        /// Static constructor
        /// </summary>
        static GenericFactory() { }
        private GenericFactory() { }

        #endregion

        #region Create Overloads
        /// <summary>
        /// Create an object using full name type
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="type">Type information to create a class instance</param>
        /// <returns>an instance of the specified type</returns>
        public static object Create(string assembly, string type)
        {
            return Create(assembly, type, (object[])null);
        }

        /// <summary>
        /// Create an object using full name type
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="type">Type information to create a class instance</param>
        /// <param name="args">constructor arguments</param>
        /// <returns>an instance of the specified type</returns>
        public static object Create(string assembly, string type, params object[] args)
        {
            Assembly assemblyInstance = getAssembly(assembly);
            Type typeInstance = assemblyInstance.GetType(assembly + "." + type, true, false);

            return CreateObject(typeInstance, args);
        }

        /// <summary>
        /// Create an object using full name type
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="type">Type information to create a class instance</param>
        /// <param name="args">constructor arguments</param>
        /// <returns>an instance of the specified type</returns>
        public static object Create(string type, params object[] args)
        {

            return CreateDummyObject(type, args);
        }

        /// <summary>
        /// Get the Type specified by a TypeIdentifier.
        /// </summary>
        /// <param name="typeIdentifier">The object used to fully describe the desired type.</param>
        /// <returns>The Type specified by typeIdentifier</returns>
        public static Type getType(TypeIdentifier typeIdentifier)
        {
            Assembly assembly = getAssembly(typeIdentifier.AssemblyName);

            String typeFullName = typeIdentifier.TypeNamespace + "." + typeIdentifier.TypeName;
            Type typeInstance = assembly.GetType(typeFullName, false, false);

            if (typeInstance == null)
            {
                throw new Exception(String.Format(@"Could not find the type '{0}' in the assembly '{1}'", typeFullName, typeIdentifier.AssemblyName));
            }

            return typeInstance;
        }

        /// <summary>
        /// Create an object using the specified type identifier and constructor parameters.
        /// </summary>
        /// <param name="typeIdentifier">The object used to fully describe the desired type.</param>
        /// <param name="args">Constructor arguments</param>
        /// <returns>An instance of the specified type</returns>
        public static object Create(TypeIdentifier typeIdentifier, params Object[] args)
        {
            return CreateObject(getType(typeIdentifier), args);
        }

        private static Assembly getAssembly(String assemblyName)
        {
            Assembly assembly;

            try
            {
                assembly = Assembly.Load(assemblyName);
            }
            catch (Exception e)
            {
                throw new Exception("Unable to load assembly: " + assemblyName, e);
            }

            if (assembly == null)
            {
                throw new Exception("Unable to load assembly: " + assemblyName);
            }

            return assembly;
        }

        public static Object CreateObject(Type type, params Object[] args)
        {
            try
            {
                if (args != null)
                {
                    return Activator.CreateInstance(type, args);
                }
                else
                {
                    return Activator.CreateInstance(type);
                }
            }
            catch (TargetInvocationException e)
            {
                throw new Exception(e.InnerException.ToString());
            }
            catch (Exception e)
            {
                throw new Exception("Unable to create requested type: " + e.Message);
            }
        }

        public static Object CreateDummyObject(String type, params Object[] args)
        {
            Object o = new Object();
            try
            {
                return o;
            }
            catch (TargetInvocationException e)
            {
                throw new Exception(e.InnerException.ToString());
            }
            catch (Exception e)
            {
                throw new Exception("Unable to create requested type: " + e.Message);
            }
        }
        #endregion
    }
}
