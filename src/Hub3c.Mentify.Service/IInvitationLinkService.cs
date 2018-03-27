using System;
using System.Collections.Generic;
using System.Text;

namespace Hub3c.Mentify.Service
{
    public interface IInvitationLinkService
    {
        string AdminInvitationLink(int systemUserId);
    }
}
