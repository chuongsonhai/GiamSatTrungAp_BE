﻿using EVN.Core.Domain;
using EVN.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.IServices
{
    public interface INgayNghiLeService : FX.Data.IBaseService<NgayNghiLe, int>
    {
        NgayNghiLe GetNgayLe(string key);
    }
  
}
