using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LE.Library.LE.Consul
{
    public struct Authorization
    {
        public string Token { get; }
        public string Method { get; }

        public Authorization(
            string token = null,
            string method = "Bearer"
        )
        {
            Token = token;
            Method = method;
        }

        public static readonly Authorization Empty = new Authorization();

        public static implicit operator Authorization(string token) =>
            new Authorization(token);

        public bool IsEmpty => Token == null;
    }
}
