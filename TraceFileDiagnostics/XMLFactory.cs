// © Parata Systems, LLC 2008 
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Collections;

namespace ServiceTool
{
    public class XMLFactory
    {
        #region String Constants
        private const String LocalDirectoryString = "LocalDirectory";
        private const String FilenameString = "Filename";
        private const String VariablesDefinitionsString = "VariablesDefinitions";
        private const String HandledString = "Handled";
        private const String FeaturesFileString = "ConfigurationTemplateFile";
        private const String ConfigurationItemsFileString = "ConfigurationItemsFile";
        private const String ConfigurationItemsTemplateFileString = "ConfigurationItemsTemplateFile";
        private const String ConfigurationItemsFilesString = "ConfigurationItemsFiles";
        private const String TrueString = "True";
        #endregion String Constants

        #region Static Fields
        /// <summary>
        /// Static list of keywords to be used when parsing files
        /// </summary>
        private static List<String> XMLFactoryKeywords = new List<string>(new String[] { VariablesDefinitionsString, LocalDirectoryString, FilenameString, ConfigurationItemsFileString, ConfigurationItemsTemplateFileString, ConfigurationItemsFilesString, TrueString });
        private static OrderedDictionary globalVariables = new OrderedDictionary();
        #endregion Static Fields

        #region Methods


        public static void buildObjects(String filename, string[] tags, Dictionary<string, List<object>> oDictionary)
        {
            XPathNavigator docNavigator = getNavigatorForFile(filename);

            traverseNode(docNavigator, filename, null);

            XPathNodeIterator typeElementIterator = docNavigator.SelectDescendants("Type", String.Empty, false);
            try
            {
                foreach (string s in tags)
                {
                    foreach (XPathNavigator typeElement in typeElementIterator)
                    {
                        if (typeElement.Value == s)
                        {
                            TypeIdentifier typeId = new TypeIdentifier(typeElement);

                            typeElement.MoveToParent();

                            try
                            {
                                Object o = GenericFactory.Create(typeId, typeElement);
                                if(!oDictionary.ContainsKey(s))
                                    oDictionary.Add(s, new List<object>());
                                oDictionary[s].Add(o);
                            }
                            catch (Exception ex)
                            {
                                throw new InvalidOperationException(string.Format("Error while building {0} as {1}", typeElement.Name, typeId.TypeName), ex);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(String.Format("Error building objects in '{0}'", filename), e);
            }
        }


        #region Static Build utilities
        /// <summary>
        /// Build objects from an xml file.
        /// </summary>
        /// <param name="filename">Full path name of xml</param>
        /// <remarks>
        /// By convention, the xml for automatic object creation is formatted as:
        /// <![CDATA[
        /// <InstanceName>
        ///   <Type assembly="myAssemblyName" namespace="Parata.MyAssembly">TypeString</Type>
        ///   ... instance data ...
        /// </InstanceName>
        /// ]]>
        /// The constructor for the Type (specified by TypeString) is passed an XPathNavigator pointing to InstanceName.
        /// </remarks>
        public static void buildObjects(String filename)
        {
            XPathNavigator docNavigator = getNavigatorForFile(filename);

            traverseNode(docNavigator, filename, null);

            // Build objects from elements with "Type" tag
            XPathNodeIterator typeElementIterator = docNavigator.SelectDescendants("Type", String.Empty, false);
            try
            {
                foreach (XPathNavigator typeElement in typeElementIterator)
                {
                    TypeIdentifier typeId = new TypeIdentifier(typeElement);

                    typeElement.MoveToParent();

                    try
                    {
                        GenericFactory.Create(typeId, typeElement);
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException(string.Format("Error while building {0} as {1}", typeElement.Name, typeId.TypeName), ex);
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(String.Format("Error building objects in '{0}'", filename), e);
            }
        }

        public static void buildObjects(String filename, String tagType)
        {
            XPathNavigator docNavigator = getNavigatorForFile(filename);

            traverseNode(docNavigator, filename, null);

            XPathNodeIterator typeElementIterator = docNavigator.SelectDescendants("Type", String.Empty, false);
            try
            {
                foreach (XPathNavigator typeElement in typeElementIterator)
                {
                    if (typeElement.Value == tagType)
                    {
                        TypeIdentifier typeId = new TypeIdentifier(typeElement);

                        typeElement.MoveToParent();

                        try
                        {
                            GenericFactory.Create(typeId, typeElement);
                        }
                        catch (Exception ex)
                        {
                            throw new InvalidOperationException(string.Format("Error while building {0} as {1}", typeElement.Name, typeId.TypeName), ex);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(String.Format("Error building objects in '{0}'", filename), e);
            }
        }

        private static XPathNavigator traverseNode(XPathNavigator docNavigator, string currentFilename, OrderedDictionary parentVariables)
        {
            if (docNavigator.Name == ConfigurationItemsFilesString)
            {
                docNavigator = loadConfigurationItems(docNavigator, currentFilename);
            }
            else if (docNavigator.Name == VariablesDefinitionsString)
            {
                docNavigator = loadVariables(docNavigator, currentFilename, null);
            }
            else if (docNavigator.Name == FilenameString)
            {
                docNavigator = expandDataFile(docNavigator, currentFilename, null);
            }
            else
            {
                docNavigator = substituteVariables(docNavigator, null);
            }

            // Note that you cannot use an iterator here because the 
            // members of the collection can be changed out via variable substitution.
            if (docNavigator.HasChildren)
            {
                XPathNavigator childNode = docNavigator.Clone();
                childNode.MoveToFirstChild();

                do
                {
                    childNode = traverseNode(childNode, currentFilename, null);
                } while (childNode.MoveToNext());
            }

            return docNavigator;
        }

        private static XPathNavigator expandDataFile(XPathNavigator docNavigator, string currentFilename, OrderedDictionary variables)
        {
            string currentPath = Path.GetDirectoryName(currentFilename);
            String dataFileName = Path.Combine(currentPath, docNavigator.Value);
            XPathNavigator dataFileNavigator = getNavigatorForFile(dataFileName);

            if (docNavigator.GetAttribute(HandledString, String.Empty) != TrueString)
            {
                docNavigator.CreateAttribute(String.Empty, HandledString, String.Empty, TrueString);
            }
            else
            {
                return docNavigator;
            }


            // Move to the child of the mandatory root node of the document and select the children of that node.  
            // This is done so that all the sub nodes of the child will be inserted without an extraneous parent node.
            dataFileNavigator.MoveToChild(XPathNodeType.Element);

            dataFileNavigator = traverseNode(dataFileNavigator, dataFileName, null);

            XPathNodeIterator dataFileNodes = dataFileNavigator.SelectChildren(XPathNodeType.Element);
            docNavigator.InsertElementBefore(String.Empty, LocalDirectoryString, String.Empty, Path.GetDirectoryName(dataFileName));

            // Interate through all the nodes in the data file adding each of them to the document
            foreach (XPathNavigator dataFileNode in dataFileNodes)
            {
                docNavigator.InsertBefore(dataFileNode.ReadSubtree());
            }

            return docNavigator;
        }

        private static XPathNavigator loadVariables(XPathNavigator docNavigator, string currentFilename, OrderedDictionary variables)
        {
            XPathNodeIterator childIterator = docNavigator.SelectChildren(XPathNodeType.Element);

            foreach (XPathNavigator childNode in childIterator)
            {
                traverseNode(childNode, currentFilename, null);
            }

            foreach (XPathNavigator childNode in childIterator)
            {
                // Ignore this keywords, they are not variables.
                if (!XMLFactoryKeywords.Contains(childNode.Name))
                {
                    // If the varialbe already exist then write the new value.
                    if (globalVariables.Contains(childNode.Name))
                    {
                        globalVariables[childNode.Name] = childNode.Value;
                    }
                    // Otherwise just add it to the list of variables.
                    else
                    {
                        globalVariables.Insert(0, childNode.Name, childNode.Value);
                    }

                    // Update any configuration item that may be associated with the variable.
                    ConfigurationItem.updateConfigurationItem(childNode.Name, globalVariables[childNode.Name].ToString());
                }
            }

            return docNavigator;
        }

        private static XPathNavigator loadConfigurationItems(XPathNavigator parentFileNavigator, string parentFilename)
        {
            string currentPath = Path.GetDirectoryName(parentFilename);

            bool fileModified = false;

            String configurationItemsTemplateFilename = Path.Combine(currentPath, Read<String>(parentFileNavigator, ConfigurationItemsTemplateFileString));
            String configurationItemsFilename = Path.Combine(currentPath, Read<String>(parentFileNavigator, ConfigurationItemsFileString));

            XPathNavigator templateNavigator = getNavigatorForFile(configurationItemsTemplateFilename);
            XPathNavigator configNavigator;
            try
            {
                configNavigator = getNavigatorForFile(configurationItemsFilename);
            }
            catch (System.IO.FileNotFoundException)
            {
                fileModified = true;
                configNavigator = templateNavigator.Clone();
            }

            // Move into the root element
            templateNavigator.MoveToFirstChild();
            configNavigator.MoveToFirstChild();

            // Make sure all the template nodes are in the config file
            XPathNodeIterator templateNodes = templateNavigator.SelectChildren(XPathNodeType.Element);
            foreach (XPathNavigator templateNode in templateNodes)
            {
                if (!configNavigator.MoveToChild(templateNode.Name, String.Empty))
                {
                    fileModified = true;
                    configNavigator.AppendChild(templateNode);
                }
                else
                {
                    configNavigator.MoveToParent();
                }

                new ConfigurationItem(templateNode, configurationItemsFilename);
            }

            // Make sure only the template node are in the config file
            XPathNodeIterator configNodes = configNavigator.SelectChildren(XPathNodeType.Element);
            foreach (XPathNavigator configNode in configNodes)
            {
                if (!templateNavigator.MoveToChild(configNode.Name, String.Empty))
                {
                    fileModified = true;
                    configNode.DeleteSelf();
                }
                else
                {
                    templateNavigator.MoveToParent();
                }
            }

            if (fileModified)
            {
                saveNavigatorToFile(configNavigator, configurationItemsFilename);
            }

            return parentFileNavigator;
        }


        /// <summary>
        /// Revises the document such that any occurrence of the variable key surrounded by "_" 
        /// in tags or values will be substituted with the variable value.
        /// </summary>
        /// <param name="docNavigator">The navigator of the document to perform variable substitution on.</param>
        /// <param name="variables">Variable to perform substitutions on.</param>
        private static XPathNavigator substituteVariables(XPathNavigator docNavigator, OrderedDictionary variables)
        {
            if (globalVariables.Count == 0)
            {
                return docNavigator;
            }

            String doc = docNavigator.OuterXml.ToString();
            foreach (String variable in globalVariables.Keys)
            {
                String variableString = "_" + variable + "_";
                doc = doc.Replace(variableString, (String)globalVariables[variable]);
            }

            docNavigator.ReplaceSelf(doc);
            return docNavigator;
        }

        /// <summary>
        /// Creates a new XmlDocument for the file, 
        /// loads the file ignoring namespaces, 
        /// closes the file and returns a navigator to the document.
        /// </summary>
        /// <param name="filename">Name of file containing XML data.</param>
        /// <returns>A navigator to the xml contained in the file.</returns>
        public static XPathNavigator getNavigatorForFile(String filename)
        {
            // Create a document and navagator for this data file
            XmlDocument document = new XmlDocument();
            using (XmlTextReader reader = new XmlTextReader(filename))
            {
                reader.Namespaces = false;
                document.Load(reader);
                reader.Close();
            }
            return document.CreateNavigator();
        }

        /// <summary>
        /// Saves the XML document pointed to by the navigator to the filename specified.
        /// </summary>
        /// <param name="navigator">A navigator to the XML to save.</param>
        /// <param name="filename">The name of the file to write to. If the file exists, it overwrites it with the new content.</param>
        public static void saveNavigatorToFile(XPathNavigator navigator, String filename)
        {
            using (XmlTextWriter docWriter = new XmlTextWriter(filename, System.Text.Encoding.Default))
            {
                docWriter.Formatting = Formatting.Indented;
                XPathNavigator rootNavigator = navigator.Clone();
                rootNavigator.MoveToRoot();
                docWriter.WriteNode(rootNavigator, true);
                docWriter.Flush();
                docWriter.Close();
            }
        }
        #endregion Static Build utilities

        #region XML Read utilities
        public static String ReadFilename(XPathNavigator xml, String tagToFind)
        {
            String localName = xml.LocalName;
            try
            {
                return Path.Combine(getCurrentDirectory(xml.Clone()), xml.SelectSingleNode(tagToFind).Value);
            }
            catch (NullReferenceException)
            {
                throw new MissingConfigurationParameterException(localName, tagToFind);
            }
        }

        /// <summary>
        /// Reads a dictionary from XML.
        /// </summary>
        /// <typeparam name="T"> The type of values stored in the dictionary.</typeparam>
        /// <param name="xml">XML holding the dictionary to be read.</param>
        /// <param name="tagToFind">The top level tag of the dictionary.</param>
        /// <returns>A dictionary of type string to Ts holding the values associated with each of the keys read in.</returns>
        /// <remarks>
        /// All elements contained within xml are assumed to be members of the dictionary, where the keys are the element names.
        /// Throws an exception if the resulting dictionary has a count of zero.
        /// </remarks>
        public static Dictionary<String, T> ReadDictionary<T>(XPathNavigator xml, String tagToFind)
        {
            return ReadDictionary<T>(xml, tagToFind, false);
        }

        /// <summary>
        /// Reads a dictionary from XML.
        /// </summary>
        /// <typeparam name="T"> The type of values stored in the dictionary.</typeparam>
        /// <param name="xml">XML holding the dictionary to be read.</param>
        /// <param name="tagToFind">The top level tag of the dictionary.</param>
        /// <param name="allowEmptyList">Indicates if the returned dictionary is allowed to have a count of zero.</param>
        /// <returns>A dictionary of type string to Ts holding the values associated with each of the keys read in.</returns>
        /// <remarks>
        /// All elements contained within xml are assumed to be members of the dictionary, where the keys are the element names.
        /// </remarks>
        public static Dictionary<String, T> ReadDictionary<T>(XPathNavigator xml, String tagToFind, Boolean allowEmptyList)
        {
            String localName = xml.LocalName;

            if (!xml.MoveToChild(tagToFind, String.Empty))
            {
                throw new MissingConfigurationParameterException(localName, tagToFind);
            }

            Dictionary<String, T> dictionary = new Dictionary<String, T>();
            XPathNodeIterator childIterator = xml.SelectChildren(XPathNodeType.Element);

            try
            {
                foreach (XPathNavigator childNode in childIterator)
                {
                    dictionary.Add(childNode.Name, Read<T>(xml, childNode.Name));
                }
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("{0}: Error building dictionary element: " + e.Message, tagToFind));
            }
            finally
            {
                xml.MoveToParent();
            }

            if ((dictionary.Count == 0) && (allowEmptyList == false))
            {
                throw new Exception(string.Format("{0}: Contains no elements: ", tagToFind));
            }

            return dictionary;

        }

        /// <summary>
        /// Reads a dictionary from XML.
        /// </summary>
        /// <typeparam name="T"> The type of values stored in the dictionary.</typeparam>
        /// <typeparam name="TKey"> The type of the key for indexing items in the dictionary.</typeparam>
        /// <param name="xml">XML holding the dictionary to be read.</param>
        /// <param name="itemTagsToFind">Tags associated with the dictionary entries.</param>
        /// <param name="keyTag">The tag within each entry that should be used as the key for the entry.</param>
        /// <returns>A dictionary of type string to Ts holding the values associated with each of the keys read in.</returns>
        /// <remarks>        
        /// Throws an exception if the resulting dictionary has a count of zero.
        ///</remarks>
        public static Dictionary<TKey, T> ReadDictionary<TKey, T>(XPathNavigator xml, String keyTag, String itemTagsToFind)
        {
            return ReadDictionary<TKey, T>(xml, keyTag, itemTagsToFind, false);
        }

        /// <summary>
        /// Reads a dictionary from XML.
        /// </summary>
        /// <typeparam name="T"> The type of values stored in the dictionary.</typeparam>
        /// <typeparam name="TKey"> The type of the key for indexing items in the dictionary.</typeparam>
        /// <param name="xml">XML holding the dictionary to be read.</param>
        /// <param name="itemTagsToFind">Tags associated with the dictionary entries.</param>
        /// <param name="keyTag">The tag within each entry that should be used as the key for the entry.</param>
        /// <param name="allowEmptyList">Indicates if the returned dictionary is allowed to have a count of zero.</param>
        /// <returns>A dictionary of type string to Ts holding the values associated with each of the keys read in.</returns>
        public static Dictionary<TKey, T> ReadDictionary<TKey, T>(XPathNavigator xml, String keyTag, String itemTagsToFind, Boolean allowEmptyList)
        {
            String localName = xml.LocalName;

            Dictionary<TKey, T> dictionary = new Dictionary<TKey, T>();
            XPathNodeIterator itemIterator = xml.SelectChildren(itemTagsToFind, String.Empty);

            try
            {
                foreach (XPathNavigator item in itemIterator)
                {
                    dictionary.Add(Read<TKey>(item, keyTag), ReadValue<T>(item));
                }
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("{0}: Error building dictionary element: " + e.Message, itemTagsToFind));
            }

            if ((dictionary.Count == 0) && (allowEmptyList == false))
            {
                throw new MissingConfigurationParameterException(xml.LocalName, itemTagsToFind);
            }

            return dictionary;
        }


        /// <summary>
        /// Reads a dictionary with specified key values from XML.
        /// </summary>
        /// <typeparam name="T"> The type of values stored in the dictionary.</typeparam>
        /// <param name="xml">XML holding the dictionary to be read.</param>
        /// <param name="tagToFind">The top level tag of the dictionary.</param>
        /// <param name="keys">The keys of the values to be read in.</param>
        /// <returns>A dictionary of type string to Ts holding the values associated with each of the keys read in.</returns>
        public static Dictionary<String, T> ReadDictionary<T>(XPathNavigator xml, String tagToFind, IEnumerable<String> keys)
        {
            String localName = xml.LocalName;

            if (!xml.MoveToChild(tagToFind, String.Empty))
            {
                throw new MissingConfigurationParameterException(localName, tagToFind);
            }

            Dictionary<String, T> dictionary = new Dictionary<String, T>();

            try
            {
                foreach (String key in keys)
                {
                    dictionary.Add(key, Read<T>(xml, key));
                }
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("{0}: Error building dictionary element: " + e.Message, tagToFind));
            }
            finally
            {
                xml.MoveToParent();
            }
            return dictionary;
        }

        /// <summary>
        /// Reads a list of item tag values into a list.
        /// </summary>
        /// <typeparam name="T">Type of item found in the list</typeparam>
        /// <param name="xml">XML navigator to the parent of the list list node.</param>
        /// <param name="itemTagsToFind">Tags associated with the list entries.</param>
        /// <param name="listTagToFind">Tag identifing the list itself.</param>
        /// <returns>A list containing all the entries found under the list node.</returns>
        /// <remarks>
        /// Throws a MissingConfigurationParameterException if the list tag cannot be found.
        /// Throws an exception if the resulting list has a count of zero.
        /// </remarks>
        public static List<T> ReadList<T>(XPathNavigator xml, String itemTagsToFind, String listTagToFind)
        {
            return ReadList<T>(xml, itemTagsToFind, listTagToFind, false);
        }

        /// <summary>
        /// Reads a list of item tag values into a list.
        /// </summary>
        /// <typeparam name="T">Type of item found in the list</typeparam>
        /// <param name="xml">XML navigator to the parent of the list list node.</param>
        /// <param name="itemTagsToFind">Tags associated with the list entries.</param>
        /// <param name="listTagToFind">Tag identifing the list itself.</param>
        /// <param name="allowEmptyList">Indicates if the returned list is allowed to have a count of zero.</param>
        /// <returns>A list containing all the entries found under the list node.</returns>
        /// <remarks>Throws a MissingConfigurationParameterException if the list tag cannot be found.</remarks>
        public static List<T> ReadList<T>(XPathNavigator xml, String itemTagsToFind, String listTagToFind, Boolean allowEmptyList)
        {
            String localName = xml.LocalName;

            if (!xml.MoveToChild(listTagToFind, String.Empty))
            {
                throw new MissingConfigurationParameterException(localName, listTagToFind);
            }

            try
            {
                return ReadList<T>(xml, itemTagsToFind, allowEmptyList);
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("{0}: Error building List: " + e.Message, listTagToFind));
            }
            finally
            {
                xml.MoveToParent();
            }
        }

        /// <summary>
        /// Reads a list of item tag values into a list.
        /// </summary>
        /// <typeparam name="T">Type of item found in the list</typeparam>
        /// <param name="xml">XML navigator to the parent of the list list items.</param>
        /// <param name="itemTagsToFind">Tags associated with the list entries.</param>
        /// <returns>A list containing all the entries found.</returns>
        /// <remarks>Throws an exception if the resulting list has a count of zero.</remarks>
        public static List<T> ReadList<T>(XPathNavigator xml, String itemTagsToFind)
        {
            return ReadList<T>(xml, itemTagsToFind, false);
        }

        /// <summary>
        /// Reads a list of item tag values into a list.
        /// </summary>
        /// <typeparam name="T">Type of item found in the list</typeparam>
        /// <param name="xml">XML navigator to the parent of the list list items.</param>
        /// <param name="itemTagsToFind">Tags associated with the list entries.</param>
        /// <param name="allowEmptyList">Indicates if the returned list is allowed to have a count of zero.</param>
        /// <returns>A list containing all the entries found.</returns>
        public static List<T> ReadList<T>(XPathNavigator xml, String itemTagsToFind, Boolean allowEmptyList)
        {
            return ReadList<T>(xml, itemTagsToFind, allowEmptyList, typeof(T));
        }

        /// <summary>
        /// Reads a list of item tag values into a list.
        /// </summary>
        /// <typeparam name="T">Type of item found in the list</typeparam>
        /// <param name="xml">XML navigator to the parent of the list list items.</param>
        /// <param name="itemTagsToFind">Tags associated with the list entries.</param>
        /// <param name="allowEmptyList">Indicates if the returned list is allowed to have a count of zero.</param>
        /// <param name="type">The specific type of to create.</param>
        /// <returns>A list containing all the entries found.</returns>
        public static List<T> ReadList<T>(XPathNavigator xml, String itemTagsToFind, Boolean allowEmptyList, Type type)
        {
            String localName = xml.LocalName;

            List<T> list = new List<T>();

            XPathNodeIterator itemIterator = xml.SelectChildren(itemTagsToFind, String.Empty);

            foreach (XPathNavigator item in itemIterator)
            {
                list.Add((T)ReadValue(item, type));
            }

            if ((list.Count == 0) && (allowEmptyList == false))
            {
                throw new MissingConfigurationParameterException(xml.LocalName, itemTagsToFind);
            }

            return list;
        }

        public static T Read<T>(XPathNavigator xml, String tagToFind)
        {
            String localName = xml.LocalName;
            try
            {
                return (T)ReadValue<T>(xml.SelectSingleNode(tagToFind));
            }
            catch (NullReferenceException)
            {
                throw new MissingConfigurationParameterException(localName, tagToFind);
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("{0}: Unable to convert to {1}.", tagToFind, typeof(T).ToString()), e);
            }
        }

        public static T Read<T>(XPathNavigator xml, String tagToFind, T defaultValue) where T : System.IConvertible
        {
            String localName = xml.LocalName;
            try
            {
                XPathNavigator node = xml.SelectSingleNode(tagToFind);
                if (node != null)
                {
                    return (T)ReadValue<T>(xml.SelectSingleNode(tagToFind));
                }
                else
                {
                    return defaultValue;
                }
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("{0}: Unable to convert to {1}.", tagToFind, typeof(T).ToString()), e);
            }
        }

        private static object ReadValue(XPathNavigator xml, Type type)
        {
            // Enum must be evaluated first because enums are convertible.
            if (type.IsEnum)
            {
                return System.Enum.Parse(type, xml.Value);
            }
            else if (type.GetInterface("System.IConvertible") != null)
            {
                return Convert.ChangeType(xml.Value, type, System.Globalization.CultureInfo.InvariantCulture);
            }
            else
            {
                return GenericFactory.CreateObject(type, xml);
            }
        }

        private static T ReadValue<T>(XPathNavigator xml)
        {
            return (T)ReadValue(xml, typeof(T));
        }

        private static String getCurrentDirectory(XPathNavigator xml)
        {
            if (xml.MoveToChild(LocalDirectoryString, String.Empty))
            {
                return xml.Value;
            }
            while (true)
            {
                while (xml.MoveToPrevious())
                {
                    if (xml.Name == LocalDirectoryString)
                    {
                        return xml.Value;
                    }
                }
                if (!xml.MoveToParent())
                {
                    return String.Empty;
                }
            }
        }
        #endregion XML Read utilities
        #endregion Methods

    }
}
