using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PinedaApp.Contracts
{
    public record ExpertiseRequest
    (
        int UserId,
        string Skills
    );
}
