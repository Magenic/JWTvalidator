using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace JwtValidator
{
  public class JwtIdentity : IIdentity
  {
    public JsonWebTokenClaims Claims { get; private set; }

    public JwtIdentity(JsonWebTokenClaims claims)
    {
      Claims = claims;
    }

    public virtual string AuthenticationType
    {
      get { return "JWT"; }
    }

    public bool IsAuthenticated
    {
      get { return (Claims != null); }
    }

    public virtual string Name
    {
      get { return (Claims == null ? string.Empty : Claims.UserId); }
    }
  }
}
