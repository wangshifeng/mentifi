using Hub3c.Mentify.Service.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hub3c.Mentify.Service
{
    public interface IInformalExperienceService
    {
        void CreateOrUpdate(UserInformalExperienceModel model);
    }
}
