﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs
{
    public class ResponseDTOs
    {
        public string Status { get; set; }
        public string  Message { get; set; }
        public object? Data { get; set; }

    }
}
