using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaSample.Services
{
    public interface ISomeService
    {
        int GetSomeNumber();
        IEnumerable<KeyValuePair<string,string?>> GetConfigurations();
    }
}
