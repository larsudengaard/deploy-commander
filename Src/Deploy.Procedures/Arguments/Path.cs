using System.Linq;

namespace Deploy.Procedures.Arguments
{
    public struct Path
    {
        readonly string value;

        public string Value
        {
            get { return value + (value.Last() != '\\' ? "\\" : ""); }
        }

        public Path(string path) : this()
        {
            value = path;
        }

        public override string ToString()
        {
            return Value;
        }

        public static implicit operator string(Path path)
        {
            return path.ToString();
        }
    }
}