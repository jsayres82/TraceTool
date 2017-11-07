// © Parata Systems, LLC 2008 
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;

namespace ServiceTool
{
    /// <summary>
    /// Encapsulates the details of a configurable system feature.
    /// </summary>
    public class ConfigurationItem
    {
        #region String Constants
        protected const String OptionsString = "Options";
        #endregion String Constants

        #region Properties
        private static Dictionary<String, ConfigurationItem> configurationItems = new Dictionary<String, ConfigurationItem>();
        /// <summary>
        /// Static list containing all the configuration items for the system.
        /// </summary>
        public static Dictionary<String, ConfigurationItem> ConfigurationItems { get { return configurationItems; } }

        public String Name { get; set; }

        private List<String> options = new List<String>();
        /// <summary>
        /// Valid values that this item can be set to.
        /// </summary>
        public List<String> Options { get { return options; } }

        private String value;
        /// <summary>
        /// Current value of the configuration option.
        /// </summary>
        public String Value
        {
            get { return this.value; }
            set
            {
                if (!Options.Contains(value))
                {
                    throw new Exception("Invalid value for configuration item");
                }
                this.value = value;
            }
        }

        /// <summary>
        /// Filename where the item is stored to disk.
        /// </summary>
        public String Filename { get; set; }
        #endregion Properties

        #region Constructors
        /// <summary>
        /// Creates a ConfigurationItem.
        /// </summary>
        /// <param name="xml">XML navigator pointing to definition of the ConfigurationItem.</param>
        /// <param name="filename">Filename the ConfigurationItem value is stored in.</param>
        /// <remarks>
        /// Note that the file that the value is stored in is not necessarily the same file
        /// that the ConfigurationItem is defined in, and in fact, it is usually not the same file.
        /// </remarks>
        public ConfigurationItem(XPathNavigator xml, String filename)
        {
            Name = xml.Name;
            Options.AddRange(xml.GetAttribute(OptionsString, String.Empty).Split(','));
            Value = xml.Value;
            Filename = filename;
            configurationItems.Add(this.Name, this);
        }

        #endregion Constructors

        #region Methods
        private void writeValueToFile(String newValue)
        {
            Value = newValue;
            XmlDocument document = new XmlDocument();
            XmlTextReader docReader = new XmlTextReader(Filename);
            document.Load(docReader);
            docReader.Close();

            XPathNavigator navigator = document.CreateNavigator();
            XPathNodeIterator nodeIterator = navigator.SelectDescendants(Name, String.Empty, false);
            foreach (XPathNavigator node in nodeIterator)
            {
                node.SetValue(newValue);
            }

            XmlTextWriter docWriter = new XmlTextWriter(Filename, System.Text.Encoding.Default);
            docWriter.Formatting = Formatting.Indented;
            navigator.MoveToRoot();
            docWriter.WriteNode(navigator, true);
            docWriter.Flush();
            docWriter.Close();
        }

        /// <summary>
        /// Gets all the configuration items.
        /// </summary>
        /// <returns>A dictionary of the item names and their current values.</returns>
        public static IDictionary<String, String> getConfigurationItems()
        {
            IDictionary<string, string> items = new Dictionary<string, string>();
            foreach (ConfigurationItem item in ConfigurationItem.ConfigurationItems.Values)
            {
                items.Add(item.Name, item.Value);
            }
            return items;
        }

        /// <summary>
        /// Gets the possible machine configuration options for the given item.
        /// </summary>
        /// <param name="configurationItem">The name of the configuration item.</param>
        /// <returns>A list of the machine configuration options.</returns>
        public static IList<String> getConfigurationOptions(String configurationItem)
        {
            if (ConfigurationItem.ConfigurationItems.ContainsKey(configurationItem))
            {
                return ConfigurationItem.ConfigurationItems[configurationItem].Options;
            }
            else return new List<string>();
        }

        /// <summary>
        /// Updates the file associated with the given configuration item.
        /// </summary>
        /// <param name="configurationItem">The name of the configuration item.</param>
        /// <param name="value">The new value for configuration item.</param>
        public static void updateConfigurationItem(String configurationItem, String value)
        {
            if (ConfigurationItem.ConfigurationItems.ContainsKey(configurationItem))
            {
                ConfigurationItem.ConfigurationItems[configurationItem].writeValueToFile(value);
            }
        }
        #endregion Methods
    }
}
