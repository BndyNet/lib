// ==========================================================================
// Copyright (c) 2013 http://www.bndy.net
// Created by Bndy at 3/21/2013 21:51:30
// --------------------------------------------------------------------------
// Math Helper
// ==========================================================================

using System.Collections.Generic;

namespace Net.Bndy
{
    public class MathHelper
    {
        public static List<int> Range(int end)
        {
            return Range(1, end);
        }
        public static List<int> Range(int start, int end, int increase = 1)
        {
            var result = new List<int>();
            for (var i = start; i <= end; i += increase)
            {
                result.Add(i);
            }

            return result;
        }
    }
}
