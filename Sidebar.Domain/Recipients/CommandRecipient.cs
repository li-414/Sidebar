﻿using Sidebar.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sidebar.Domain.Recipients
{
    public record CommandRecipient(string Title, CommandEnum CommandEnum);
}