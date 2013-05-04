using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Grax.fFastMapper;

namespace fFastMapper.Tests.Library
{
    internal static class Functions
    {
        public static SerializationInfo GetExceptionSerializationInfo(Type exceptionType, string errorMessage)
        {
            var info = new SerializationInfo(exceptionType, new FormatterConverter());
            info.AddValue("ClassName", string.Empty);
            info.AddValue("Message", errorMessage);
            info.AddValue("InnerException", new ArgumentException());
            info.AddValue("HelpURL", string.Empty);
            info.AddValue("StackTraceString", string.Empty);
            info.AddValue("RemoteStackTraceString", string.Empty);
            info.AddValue("RemoteStackIndex", 0);
            info.AddValue("ExceptionMethod", string.Empty);
            info.AddValue("HResult", 1);
            info.AddValue("Source", string.Empty);

            return info;
        }

        public static void ResetGlobalSettings()
        {
            fFastMap
                .GlobalSettings()
                .SetAutoInitialize(true)
                .SetDefaultMappingDirection(fFastMap.MappingDirection.Bidirectional);
        }
    }
}
