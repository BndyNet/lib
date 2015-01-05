// =================================================================================
// Copyright (c) 2014 http://www.bndy.net
// Created by Bndy at 3/18/2014 11:16:04
// ---------------------------------------------------------------------------------
// Configuration Attribute for Xml Configuration File
// =================================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Net.Bndy
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
	public class XmlConfigurationAttribute : Attribute
	{
		public string MappingName { get; private set; }
		public bool IsRequired { get; private set; }
		public string DefaultValue { get; private set; }
		public string[] Options { get; private set; }

		public XmlConfigurationAttribute(string mappingName = null, 
			bool isRequired = false, 
			string defaultValue = null,
			params string[] options)
		{
			this.MappingName = mappingName;
			this.IsRequired = IsRequired;
			this.DefaultValue = defaultValue;
			this.Options = options;
		}
	}
}
