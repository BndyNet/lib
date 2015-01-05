// ==========================================================================
// Copyright (c) 2013 http://www.bndy.net
// Created by Bndy at 3/20/2013 11:50:30
// --------------------------------------------------------------------------
// Customization Attributes.
// ==========================================================================

using System;
using System.ComponentModel;

namespace Net.Bndy
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public class DisplayAttribute : Attribute
	{
		public string Title { get; set; }
		public string Description { get; set; }

		public DisplayAttribute(string title, string description = null)
		{
			this.Title = title;
			this.Description = description;
		}
	}
}
