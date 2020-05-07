using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace RazorEngineCore
{
    public class RazorEngineCompilationException : RazorEngineException
    {
        public RazorEngineCompilationException()
        {
        }

        protected RazorEngineCompilationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public RazorEngineCompilationException(string message) : base(message)
        {
        }

        public RazorEngineCompilationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public List<Diagnostic> Errors { get; set; }
    }
}