using System;
using System.Runtime.Serialization;

namespace Deploy.Procedures.Arguments
{
    [Serializable]
    public class MissingProcedureArgumentException : Exception
    {
        public MissingProcedureArgumentException(string message)
            : base(message)
        {
        }

        public MissingProcedureArgumentException(string projectName, string argumentName, Type type)
            : base(string.Format("Project configuration for project '{0}' is missing the argument named '{1}'", projectName, argumentName))
        {
        }

        public MissingProcedureArgumentException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected MissingProcedureArgumentException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}