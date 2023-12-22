using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PinedaApp.Contracts
{
    public record Response
    (
        string Status,
        IDictionary<string, object> Data
    );
}
