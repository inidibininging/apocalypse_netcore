﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Apocalypse.Any.Domain.Common.Model
{
    public interface ICurrencyHolder<TCurrency>
    {
        TCurrency Amount { get; set; }
    }
}
