using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PinedaApp.Contracts
{
   public record ExpertiseResponse
   (
       int Id,
       int UserId,
       string Skills,
       DateTime CreatedAt,
       DateTime LastUpdatedAt
   );
}
