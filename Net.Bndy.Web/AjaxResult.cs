// =================================================================================
// Copyright (c) 2016 http://www.bndy.net.
// Created by Bndy at 11/10/2016 9:39:44 PM
// ---------------------------------------------------------------------------------
// Summary
// =================================================================================

using System;

namespace Net.Bndy.Web
{
    public class AjaxResult
    {
        public AjaxResultStatus Status { get; set; }
        public string Message { get; set; }

        public object Data { get; set; }

        public object ExtraData { get; set; }

        public AjaxResult()
        {

        }

        public AjaxResult(object data)
        {
            this.Status = AjaxResultStatus.OK;
            this.Data = data;
        }

        public AjaxResult(AjaxResultStatus status, string message)
        {
            this.Status = status;
            this.Message = message;
        }

        public AjaxResult(Exception ex)
        {
            this.Status = AjaxResultStatus.Error;
            this.Message = ex.Message;
            this.Data = ex;
        }
    }
    public enum AjaxResultStatus
    {
        OK = 200,
        Error = 500,
    }
}
