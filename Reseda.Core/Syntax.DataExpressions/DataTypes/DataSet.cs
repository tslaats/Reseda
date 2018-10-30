using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reseda.Core
{
    public class DataSet : DataType
    {
        public HashSet<DataType> value;
        //private IEnumerable<Event> enumerable;

        public DataSet()
        {
            value = new HashSet<DataType>();
        }

        public DataSet(HashSet<DataType> v)
        {
            value = v;
        }

        public DataSet(IEnumerable<DataType> s)
        {
            this.value = new HashSet<DataType>(s);
        }

        public Boolean Contains(DataType d)
        {
            return value.Contains(d);
        }

        public override string ToSource()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            if (value.Count == 0)
                return "{}";

            String result = "{";
            ;
            foreach (var e in value)
                result += e.ToString() + ",";
            return result.Substring(0,result.Length - 1) + "}";
        }

        internal override DataExpression Clone()
        {
            return new DataSet(this.value);
        }
    }
}
