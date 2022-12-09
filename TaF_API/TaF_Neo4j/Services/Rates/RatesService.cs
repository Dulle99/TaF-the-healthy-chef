using Neo4jClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaF_Neo4j.Services.Rates
{
    public class RatesService : IRatesService
    {
        private IGraphClient _client;
        public RatesService(IGraphClient client)
        {
            this._client = client;
        }
    }
}
