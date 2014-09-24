using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace JwtValidator
{
  public class JwtPrincipal : IPrincipal
  {
    private JwtIdentity _identity;

    public JwtPrincipal(JwtIdentity identity)
    {
      _identity = identity;
    }

    public JwtIdentity Identity
    {
      get { return _identity; }
    }

    public virtual bool IsInRole(string role)
    {
      return false;
    }

    IIdentity IPrincipal.Identity
    {
      get { throw new NotImplementedException(); }
    }
  }
}
