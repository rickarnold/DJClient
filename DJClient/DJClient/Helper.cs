using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DJ
{
    class Helper
    {
        public static string RemoveExtensionFromFileName(string fileName)
        {
            //Find the '.'
            int dotIndex = fileName.LastIndexOf('.');

            if (dotIndex < 1)
                return fileName;

            return fileName.Substring(0, dotIndex);
        }
    }
}
