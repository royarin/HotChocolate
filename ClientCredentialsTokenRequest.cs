using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace graphqlgateway
{
    public class ClientCredentialsTokenRequest
    {
        public string Address { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }
}
