// ==========================================================================
// Copyright (c) 2013 http://www.bndy.net
// Created by Bndy at 3/20/2013 11:51:30
// --------------------------------------------------------------------------
// Enum Helper
// =========================================================================

using System;
using System.Collections.Generic;

namespace Net.Bndy
{
    public static class EnumHelper
    {
        public static Dictionary<string, int> ToDictionary(Type enumType)
        {
            var result = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            foreach(var item in Enum.GetValues(enumType))
            {
                result.Add(item.ToString(), (int)item);
            }

            return result;
        }
    }
}
