using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace front_end.Models
{
    [System.Serializable]
    public class Data
    {
        public string name;
        public string visual;
        public int screenLocation;
        public bool modifiable;
        public List<object> values;
        public Labels labels;
        public string dataType;
        public List<int> arg;
        public string text;
        public string readableName;
    }

    [System.Serializable]
    public class Configuration
    {
        public string link { get; set; }
        public List<Data> configuration { get; set; }
    }

    [System.Serializable]
    public class ResponseModel
    {
        public bool success { get; set; }
        public List<Data> data { get; set; }
    }

    [System.Serializable]
    public class ChartData
    {
        public string name { get; set; }
        public object value { get; set; }
        public int arg { get; set; }
    }

    [System.Serializable]
    public class Labels
    {
        public string horizontal { get; set; }
        public string vertical { get; set; }
    }
}
