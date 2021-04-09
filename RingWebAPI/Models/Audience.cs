using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RingWebAPI.Models
{
    public class AudienceModel
    {
        public string Secret { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
    }
}
