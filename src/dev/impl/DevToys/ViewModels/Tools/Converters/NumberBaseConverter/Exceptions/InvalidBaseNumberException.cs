﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevToys.ViewModels.Tools.Converters.NumberBaseConverter.Exceptions
{
    internal class InvalidBaseNumberException : Exception
    {
        public InvalidBaseNumberException(string message) : base(message)
        {

        }
    }
}
