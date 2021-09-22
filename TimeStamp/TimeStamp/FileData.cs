using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TimeStamp
{
    public class FileData
    {
        public string Name { get; set; }
        public string MD5HashFile { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}