using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LelShotter.Models
{
    public static class DataFormats
    {
        //todo Enable file format selection and preferences screen value update
        public static List<string> SupportedFormatList { get; } = new List<string>{"PNG", "JPG"};
    }
}
