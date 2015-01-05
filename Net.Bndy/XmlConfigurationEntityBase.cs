// =================================================================================
// Copyright (c) 2014 http://www.bndy.net
// Created by Bndy at 3/18/2014 11:37:40
// ---------------------------------------------------------------------------------
// The base class of xml configuration entity.
// All entities that can be set up MUST inherit this class.
// =================================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Xml;

namespace Net.Bndy
{
	public abstract class XmlConfigurationEntityBase
	{
		#region Fields
		private Dictionary<string, XmlConfigurationAttribute> _fieldAttributes;
		#endregion

		#region Properties
		/// <summary>
		/// Gets the name of the xml mapping.
		/// </summary>
		/// <value>The name of the mapping.</value>
		public string MappingName
		{
			get
			{
				var attr = this.GetType().GetCustomAttributes(typeof(XmlConfigurationAttribute), true).FirstOrDefault()
					as XmlConfigurationAttribute;
				if (attr == null || string.IsNullOrWhiteSpace(attr.MappingName))
					return this.GetType().Name;

				return attr.MappingName;
			}
		}
		#endregion

		#region Methods
		/// <summary>
		/// Initializes a new instance of the <see cref="XmlConfigurationEntityBase"/> class.
		/// </summary>
		public XmlConfigurationEntityBase()
		{
			_fieldAttributes = new Dictionary<string, XmlConfigurationAttribute>(StringComparer.OrdinalIgnoreCase);

			var selfAttributes = this.GetType().GetCustomAttributes(typeof(XmlConfigurationAttribute), true);
			var properties = this.GetType().GetProperties();

			foreach (var p in properties)
			{
				_fieldAttributes.Add(p.Name,
					p.GetCustomAttributes(typeof(XmlConfigurationAttribute), true).FirstOrDefault()
						as XmlConfigurationAttribute
					);
			}
		}
		/// <summary>
		/// Gets the XML configuration information.
		/// </summary>
		/// <param name="propertyName">Name of the property.</param>
		/// <returns>XmlConfigurationAttribute.</returns>
		public XmlConfigurationAttribute GetXmlConfigurationInfo(string propertyName)
		{
			if (_fieldAttributes.ContainsKey(propertyName))
				return _fieldAttributes[propertyName];
			return null;
		}
		/// <summary>
		/// Saves the instance to xml file.
		/// </summary>
		/// <param name="fileName">Name of the file.</param>
		public void SaveTo(string fileName)
		{
			Convert2Xml(this).Save(fileName);
		}
		/// <summary>
		/// Converts the instance to xml document.
		/// </summary>
		/// <param name="instance">The instance.</param>
		/// <param name="parentNode">The parent node.</param>
		/// <returns>XmlDocument.</returns>
		private XmlDocument Convert2Xml(object instance, XmlNode parentNode = null)
		{
			XmlDocument doc;
			if (parentNode == null) doc = new XmlDocument();
			else doc = parentNode.OwnerDocument;

			if (instance != null)
			{
				XmlNode node = doc.CreateElement((instance as XmlConfigurationEntityBase).MappingName);
				var properties = instance.GetType().GetProperties();
				foreach (var p in properties)
				{
					if (!p.GetType().IsClass)
					{
						var value = p.GetValue(this, null);
						var config = GetXmlConfigurationInfo(p.Name);
						var xmlAttribute = doc.CreateAttribute(config.MappingName);

						if (value == null)
							xmlAttribute.Value = config.DefaultValue;

						node.Attributes.Append(xmlAttribute);
					}
					else
					{
						Convert2Xml(p.GetValue(this, null), node);
					}
				}
				if (parentNode != null)
					parentNode.AppendChild(node);
				else
					doc.AppendChild(node);
			}

			return doc;
		}
		#endregion

		#region Static Members
		/// <summary>
		/// Loads an instance from XML.
		/// </summary>
		/// <typeparam name="T">The type of result</typeparam>
		/// <param name="fileName">Name of the xml file.</param>
		/// <returns>``0.</returns>
		public static T LoadFromXml<T>(string fileName) where T : XmlConfigurationEntityBase
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(fileName);

			return (T)LoadInstanceFromNode(doc.FirstChild, typeof(T));
		}
		/// <summary>
		/// Loads the instance from xml node.
		/// </summary>
		/// <param name="node">The xml node.</param>
		/// <param name="TResult">The result type.</param>
		/// <returns>System.Object.</returns>
		private static object LoadInstanceFromNode(XmlNode node, Type TResult)
		{
			object result = null;

			if (node != null && node.NodeType == XmlNodeType.Element)
			{
				result = System.Activator.CreateInstance(TResult);

				var properties = result.GetType().GetProperties();
				var attributes = node.Attributes;
				foreach (var p in properties)
				{
					object value = null;
					if (!p.GetType().IsClass)
					{
						var mappingName = ((XmlConfigurationEntityBase)result).GetXmlConfigurationInfo(p.Name).MappingName;
						if (attributes[mappingName] != null) value = attributes[mappingName].Value;
					}
					else
					{
						value = LoadInstanceFromNode(node.FirstChild, p.GetType());
					}
					p.SetValue(result, value, null);
				}
			}

			return result;
		}
		#endregion
	}
}
