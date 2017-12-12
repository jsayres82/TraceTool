// © Parata Systems, LLC 2008 
// All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml.XPath;

using NorthStateFramework;
using Parata.Controls.Enumerations.ErrorCodes;
using System.Reflection;
using Parata.Controls;

namespace TraceFileReader
{
    /// <summary>
    /// The ControlsObjectFactory manages run-time construction, initialization, and startup of controls objects.
    /// <remarks>
    /// Run-time object construction is performed with the method buildObjects, passing in a filename for the
    /// XML file that defines the objects to be created.  The ControlsObjectFactory uses the XMLFactory class to
    /// perform object construction.  Objects which require coordinated initialization can register themselves at
    /// construction with the method addObject.  Registered objects must implement the IControlsObject interface.
    /// Once all object construction is complete, the method initializeControlsObject is called for each registered
    /// IControlsObject.  Finally, the method startControlsObject is called for each registered IControlsObject.
    /// Through this sequence, the buildup of controls objects can be performed in a coordinated fashion.
    /// 
    /// The list of registered IControlsObjects can be accessed with the methods getObject and getObjects.
    /// </remarks>
    /// <seealso cref="XMLFactory"/>
    /// </summary>
    public class ControlsObjectFactory
    {
        #region String Constants
        private const String TraceFileString = "TraceFile";
        private const String ControlsObjectFactoryString = "ControlsObjectFactory";
        #endregion String Constants

        #region Members
        private static bool systemStarted = false;
        private static Dictionary<String, IControlsObject> controlsObjects = new Dictionary<String, IControlsObject>();
        #endregion Members

        #region Properties
        public static bool SystemStarted
        {
            get { return systemStarted; }
        }
        #endregion Properties

        #region Methods

        #region Public Interface

        public static void startSystem(String configFile)
        {
            try
            {
                buildObjects(configFile);
                systemStarted = true;
                GCTracer.EnableGCTracing();
            }
            catch (Exception e)
            {
                NSFTraceLog.logTrace(NSFTraceTags.exceptionTag, NSFTraceTags.sourceTag, ControlsObjectFactoryString, NSFTraceTags.messageTag, e.ToString());
                TraceManager.saveTraceFile(TraceFileString);
                throw new Exception("Error starting system", e);
            }
        }

        public static void stopSystem()
        {
            try
            {
                clearObjects();
                systemStarted = false;
            }
            catch (Exception e)
            {
                NSFTraceLog.logTrace(NSFTraceTags.exceptionTag, NSFTraceTags.sourceTag, ControlsObjectFactoryString, NSFTraceTags.messageTag, e.ToString());
                TraceManager.saveTraceFile(TraceFileString);
                throw new Exception("Error stopping system", e);
            }
        }

        public static void updateSystem(String configFile)
        {
            try
            {
                updateObjects(configFile);
            }
            catch (Exception e)
            {
                NSFTraceLog.logTrace(NSFTraceTags.exceptionTag, NSFTraceTags.sourceTag, ControlsObjectFactoryString, NSFTraceTags.messageTag, e.ToString());
                TraceManager.saveTraceFile(TraceFileString);
                throw new Exception("Error stopping system", e);
            }
        }

        public static bool resetAllErrors()
        {
            foreach (SubsystemManager subsystem in getObjects(typeof(SubsystemManager)))
            {
                subsystem.resetError();
            }

            // Some configurations do not have subsystems, so this will reset motors in that case
            foreach (ServoMotor motor in getObjects(typeof(ServoMotor)))
            {
                motor.resetError();
            }
            return true;
        }

        /// <summary>
        /// Method that ensures singular initialization of <see cref="IControlsObject"/>s.
        /// </summary>
        /// <param name="controlsObject">The <see cref="IControlsObject"/> to be initialized.</param>
        public static void initializeControlsObject(IControlsObject controlsObject)
        {
            if (!controlsObject.IsInitialized)
            {
                controlsObject.initializeControlsObject();
                controlsObject.IsInitialized = true;
            }
        }

        /// <summary>
        /// Register an IControlsObject with the ControlsObjectFactory.
        /// </summary>
        /// <remarks>
        /// Objects which require coordinated initialization and startup should register themselves at
        /// construction.  Registered objects must implement the IControlsObject interface.  Once the method
        /// buildObjects completes all construction, the method initializeControlsObject is called for each registered
        /// IControlsObject.  Then, the method startControlsObject is called for each registered IControlsObject.
        /// Through this sequence, the buildup of controls objects can be performed in a coordinated fashion.
        /// </remarks>
        /// <param name="controlsObject">The IControlsObject registering.</param>
        public static void addObject(IControlsObject controlsObject)
        {
            lock (controlsObjects)
            {
                controlsObjects.Add(controlsObject.Name, controlsObject);
            }
        }

        public static ControlsProcess getProcess(String processName)
        {
            ControlsProcess process;

            foreach (SubsystemManager subsystem in getObjects(typeof(SubsystemManager)))
            {
                process = subsystem.getProcess(processName);

                if (process != null)
                {
                    return process;
                }
            }

            return null;
        }

        /// <summary>
        /// Get the IControlsObject that registered under the specified by name.
        /// </summary>
        /// <param name="name">Name of the IControlsObject.</param>
        /// <returns>IControlsObject with specified name.</returns>
        public static IControlsObject getObject(String name)
        {
            lock (controlsObjects)
            {
                try
                {
                    return controlsObjects[name];
                }
                catch
                {
                    throw new MissingControlsObjectException(name);
                }
            }
        }

        /// <summary>
        /// Searches through the descendants of the <see cref="IControlsObject"/> to find an <see cref="IControlsBase"/> object with the name specified.
        /// </summary>
        /// <param name="name">The name of the <see cref="IControlsBase"/> object to find.</param>
        /// <returns>A reference to the <see cref="IControlsBase"/> object if found.  Null if the object was not found.</returns>
        public static IControlsBase getControlsBaseObject(String name)
        {
            foreach (IControlsBase rootObject in getRoots())
            {
                IControlsBase searchResult = findControlsBaseObject(rootObject, name);
                if (searchResult != null)
                {
                    return searchResult;
                }
            }

            return null;
        }

        private static IControlsBase findControlsBaseObject(IControlsBase controlsObject, String name)
        {
            lock (controlsObjects)
            {
                if (controlsObject.Name == name)
                {
                    return controlsObject;
                }

                foreach (IControlsBase child in controlsObject.Link.Children.Values)
                {
                    IControlsBase searchResult = findControlsBaseObject(child, name);
                    if (searchResult != null)
                    {
                        return searchResult;
                    }
                }
                return null;
            }
        }


        /// <summary>
        /// Get all the IControlsObjects that registered with the ControlsObjectFactory.
        /// </summary>
        /// <returns>ICollection of registered objects.</returns>
        public static ICollection getObjects()
        {
            lock (controlsObjects)
            {
                return controlsObjects.Values;
            }
        }

        /// <summary>
        /// Get the IControlsObjects that match the specified type.
        /// </summary>
        /// <param name="type">Type of objects.</param>
        /// <returns>List of registered objects of specified type.</returns>
        public static List<Object> getObjects(Type type)
        {
            List<Object> objects = new List<Object>();

            lock (controlsObjects)
            {
                foreach (Object controlsObject in controlsObjects.Values)
                {
                    Type cOType = controlsObject.GetType();
                    if ((cOType.IsSubclassOf(type)) || (cOType == type))
                    {
                        objects.Add(controlsObject);
                    }
                    else
                    {
                        if (type.IsInterface)
                        {
                            foreach (Type interfaceType in cOType.GetInterfaces())
                            {
                                if (interfaceType == type)
                                {
                                    objects.Add(controlsObject);
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            return objects;
        }

        /// <summary>
        /// Get the IControlsObjects that match the specified generic type.
        /// </summary>        
        /// <returns>List of registered objects of specified type.</returns>
        public static List<T> getObjects<T>() where T : class
        {
            List<T> objects = new List<T>();

            lock (controlsObjects)
            {
                foreach (Object controlsObject in controlsObjects.Values)
                {
                    if (controlsObject is T)
                    {
                        objects.Add(controlsObject as T);
                    }
                }
            }

            return objects;
        }

        private static List<IControlsObject> getRoots()
        {
            List<IControlsObject> topLevelObjects = new List<IControlsObject>();

            foreach (IControlsObject controlsObject in controlsObjects.Values)
            {
                if (controlsObject.Link.Parents.Count == 0)
                {
                    topLevelObjects.Add(controlsObject);
                }
            }

            return topLevelObjects;
        }

        #endregion Public Interface

        #region Private Utilities

        /// <summary>
        /// Construct objects from an XML file.  Then initialize and start any registered IControlsObjects.
        /// </summary>
        /// <param name="filename">Name of the XML file describing objects to build.</param>
        /// <remarks>
        /// Construction is handled by XMLFactory class.
        /// </remarks>
        /// <seealso cref="XMLFactory"/>
        private static void buildObjects(String filename)
        {
            try
            {
                // Build up system objects
                XMLFactory.buildObjects(filename);

                lock (controlsObjects)
                {
                    // Initialize control objects
                    // Control objects registered themselves when created above
                    // The initialize function performs any work that could not be handled in the constructor, such as establishing relationships
                    foreach (IControlsObject controlsObject in controlsObjects.Values)
                    {
                        try
                        {
                            initializeControlsObject(controlsObject);
                        }
                        catch (Exception ex)
                        {
                            throw new InvalidOperationException(string.Format("Error while initializing {0}", controlsObject.ToString()), ex);
                        }
                    }

                    // Start control objects
                    // The start function kicks off any behaviors the object may have, such as state machine behaviors
                    foreach (IControlsObject controlsObject in controlsObjects.Values)
                    {
                        controlsObject.startControlsObject();
                    }
                }
            }
            catch (Exception exception)
            {
                Exception newException = new Exception("Error building controls objects", exception);
                newException.Source = typeof(ControlsObjectFactory).ToString();

                Debug.Print("{0} : {1} : {2}", newException.Source, newException.TargetSite, newException.Message);

                try
                {
                    clearObjects();
                }
                catch (Exception e)
                {
                    //If there is a problem clearing the objects, then it is a bug in the code that we ought to know about.  
                    //It should not, however, mask the original exception that brought us to the point where we ought to call clearObjects().
                    NSFTraceLog.logTrace(NSFTraceTags.exceptionTag, NSFTraceTags.sourceTag, "ControlsObjectFactory", NSFTraceTags.messageTag, e.ToString());
                }

                throw newException;
            }
        }

        /// <summary>
        /// Update objects based on data in an xml file.
        /// </summary>
        /// <param name="filename">Full path name of xml</param>
        /// <remarks>
        /// Uses the same file format as buildObjects.
        /// <seealso cref="buildObjects"/>
        /// </remarks>
        private static void updateObjects(String filename)
        {
            XPathDocument doc = new XPathDocument(filename);
            XPathNavigator docNavigator = doc.CreateNavigator();

            // Update objects from elements with "Type" tag
            XPathNodeIterator typeElementIterator = docNavigator.SelectDescendants("Type", String.Empty, false);

            foreach (XPathNavigator typeElement in typeElementIterator)
            {
                String typeString = typeElement.Value;
                typeElement.MoveToParent();
                String objectName = typeElement.Name;

                lock (controlsObjects)
                {
                    // Only attempt to update controls objects at this time
                    if (controlsObjects.ContainsKey(objectName))
                    {
                        IControlsObject controlsObject = getObject(objectName);
                        controlsObject.updateControlsObject(typeElement);
                    }
                }


            }

            // Update objects from elements with "Filename" tag
            XPathNodeIterator filenameElementIterator = docNavigator.SelectDescendants("Filename", String.Empty, false);
            foreach (XPathNavigator filenameElement in filenameElementIterator)
            {
                updateObjects(Path.Combine(Path.GetDirectoryName(filename), filenameElement.Value));
            }
        }

        /// <summary>
        /// Clear the list of registered objects.
        /// </summary>
        public static void clearObjects()
        {
            lock (controlsObjects)
            {
                foreach (IControlsObject controlsObject in controlsObjects.Values)
                {
                    controlsObject.stopControlsObject();
                }

                controlsObjects.Clear();
            }
        }

        /// <summary>
        /// Remove the object with specified name from the list of registered objects.
        /// </summary>
        /// <param name="name">Name of the object to remove.</param>
        /// <returns>Boolean true if the object was found, otherwise false.</returns>
        public static bool removeObject(String name)
        {
            lock (controlsObjects)
            {
                return controlsObjects.Remove(name);
            }
        }

        #endregion Private Utilities

        #endregion Methods
    }
}
