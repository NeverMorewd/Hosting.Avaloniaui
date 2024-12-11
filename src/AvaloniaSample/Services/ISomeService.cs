using System.Collections.Generic;

namespace AvaloniaSample.Services
{
    public interface ISomeService
    {
        int GetSomeNumber();
        IEnumerable<KeyValuePair<string,string?>> GetConfigurations();
    }
}
