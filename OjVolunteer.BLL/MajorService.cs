﻿using OjVolunteer.Common.Cache;
using OjVolunteer.IBLL;
using OjVolunteer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OjVolunteer.BLL
{
    public partial class MajorService : BaseService<Major>,IMajorService
    {
        public void UpdateCache()
        {
        }
    }
}
