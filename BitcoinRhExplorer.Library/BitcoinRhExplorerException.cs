using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Web;

namespace BitcoinRhExplorer.Library
{
    public enum ExtendedDialogType
    {
        Info = 0,
        Success = 1
    }

    public enum ExtendedTypes
    {
        Ok = 0,
        YesNo = 1,
    }

    public enum ExceptionPriority
    {
        High = 0,
        Normal = 1,
    }

    public class ExtendedException
    {
        public ExtendedException(string businessExceptionMessage)
        {
            Exception = new Exception(businessExceptionMessage);
        }

        public ExtendedException(Exception businessException)
        {
            Exception = businessException;
        }

        public ExtendedException(ExtendedException extendecException)
        {
            this.Exception = extendecException.Exception;
            this.Type = extendecException.Type;
            this.Priority = extendecException.Priority;
            this.RedirectUrl = extendecException.RedirectUrl;
            this.SessionId = extendecException.SessionId;
            this.Token = extendecException.Token;
            this.DialogType = extendecException.DialogType;
        }

        public Exception Exception { get; set; }
        public ExtendedTypes Type { get; set; }
        public ExceptionPriority Priority { get; set; }
        public string RedirectUrl { get; set; }
        public string SessionId { get; set; }
        public string Token { get; set; }
        public ExtendedDialogType DialogType { get; set; }
    }

    [Serializable]
    public class BitcoinRhExplorerException : Exception
    {
        public List<ExtendedException> Exceptions;

        public BitcoinRhExplorerException()
        {
            Exceptions = new List<ExtendedException>();
        }

        public BitcoinRhExplorerException(string businessException)
        {
            Exceptions = new List<ExtendedException>();
            Add(businessException);
        }

        public BitcoinRhExplorerException(string businessException, ExtendedDialogType dialogType)
        {
            Exceptions = new List<ExtendedException>();
            Add(businessException, dialogType);
        }

        public BitcoinRhExplorerException(string businessException, string redirectUrl,
            ExceptionPriority priority = ExceptionPriority.High)
        {
            Exceptions = new List<ExtendedException>();

            AddOkAndFill(businessException, redirectUrl, priority);
        }

        public void AddWithToken(string businessException, string token)
        {
            var extendException = new ExtendedException(businessException);
            extendException.SessionId = HttpContext.Current.Session.SessionID;
            extendException.Token = token;
            Exceptions.Add(extendException);
        }

        public void AddWithToken(Exception exception, string token)
        {
            var extendException = new ExtendedException(exception);
            extendException.SessionId = HttpContext.Current.Session.SessionID;
            extendException.Token = token;
            Exceptions.Add(extendException);
        }

        public void AddWithToken(ExtendedException exception, string token)
        {
            var extendException = new ExtendedException(exception);
            extendException.SessionId = HttpContext.Current.Session.SessionID;
            extendException.Token = token;
            Exceptions.Add(extendException);
        }


        public void Add(string businessException)
        {
            var extendException = new ExtendedException(businessException);
            extendException.SessionId = HttpContext.Current.Session.SessionID;
            Exceptions.Add(extendException);
        }

        public void Add(string businessException, ExtendedDialogType dialogType)
        {
            var extendException = new ExtendedException(businessException);
            extendException.DialogType = dialogType;
            extendException.SessionId = HttpContext.Current.Session.SessionID;
            Exceptions.Add(extendException);
        }

        public void Add(Exception exception)
        {
            var extendException = new ExtendedException(exception);
            extendException.SessionId = HttpContext.Current.Session.SessionID;
            Exceptions.Add(extendException);
        }

        public void Add(string businessException, string redirectUrl,
            ExceptionPriority priority = ExceptionPriority.High)
        {
            AddOkAndFill(businessException, redirectUrl, priority);
        }

        public void Add(Exception exception, string redirectUrl,
            ExceptionPriority priority = ExceptionPriority.High)
        {
            AddOkAndFill(exception.Message, redirectUrl, priority);
        }

        private void AddOkAndFill(string businessException, string redirectUrl, ExceptionPriority priority)
        {
            var extendException = new ExtendedException(businessException);

            extendException.Type = ExtendedTypes.Ok;
            extendException.RedirectUrl = redirectUrl;
            extendException.Priority = priority;
            extendException.SessionId = HttpContext.Current.Session.SessionID;
            Exceptions.Add(extendException);
        }

        public void AddYesNo(string businessException, string redirectUrl,
            ExceptionPriority priority = ExceptionPriority.High)
        {
            AddYesNoAndFill(businessException, redirectUrl, priority);
        }

        public void AddYesNo(Exception exception, string redirectUrl,
            ExceptionPriority priority = ExceptionPriority.High)
        {
            AddYesNoAndFill(exception.Message, redirectUrl, priority);
        }

        private void AddYesNoAndFill(string businessException, string redirectUrl, ExceptionPriority priority)
        {
            var extendException = new ExtendedException(businessException);

            extendException.Type = ExtendedTypes.YesNo;
            extendException.RedirectUrl = redirectUrl;
            extendException.Priority = priority;
            extendException.SessionId = HttpContext.Current.Session.SessionID;
            Exceptions.Add(extendException);
        }

        public List<ExtendedException> GetAll(string sessionId, string onlyToken = null)
        {
            return Exceptions
                .Where(e => e.SessionId == sessionId
                && ((onlyToken == null) || (e.Token == onlyToken)))
                .ToList();
        }

        public List<ExtendedException> GetAllIgnoreToken(string sessionId, string ignoreToken)
        {
            return Exceptions
                .Where(e => e.SessionId == sessionId
                && ((ignoreToken == null) || (e.Token != ignoreToken)))
                .ToList();
        }

        public List<ExtendedException> GetAllAndClear(string sessionId, string ignoreToken = null)
        {
            var oldExceptions = new ExtendedException[Exceptions.Count];

            Exceptions.CopyTo(oldExceptions);
            Exceptions.RemoveAll(e => e.SessionId == sessionId);

            return oldExceptions
                .Where(e => e.SessionId == sessionId
                && ((ignoreToken == null) || (e.Token != ignoreToken)))
                .ToList();
        }

        public bool Any()
        {
            return Exceptions.Any();
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public new virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
    }
}
