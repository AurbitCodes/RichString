using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringInterpolation
{
    public interface IRichStringCustomFormat
    {
        public string GetNormalForm();
        public string GetAlternateForm();
    }
}
