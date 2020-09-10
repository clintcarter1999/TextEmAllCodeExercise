using System;
using System.Collections.Generic;
using System.Text;

namespace School.Data
{
    public class ServiceResponse<T>
    {
        public T Data { get; set; }
        public bool Success { get; set; } = false;

        public string Message { get; set; } = null;
    }
}
