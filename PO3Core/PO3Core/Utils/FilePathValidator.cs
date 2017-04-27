using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PO3Core.Utils
{
    public class FilePathValidator
    {
        public static bool IsValidPath(string path)
        {
            //Simple pattern
            //@"^(([a-zA-Z]:)|(\))(\{1}|((\{1})[^\]([^/:*?<>""|]*))+)$"
            string DmitriyBorysovPattern =
                @"^(([a-zA-Z]:|\\)\\)?(((\.)|(\.\.)|([^\\/:\*\?\|<>\. ](([^\\/:\*\?\|<>\. ])|([^\\/:\*\?\|<>]*[^\\/:\*\?\|<>\. ]))?))\\)*[^\\/:\*\?\|<>\. ](([^\\/:\*\?\|<>\. ])|([^\\/:\*\?\|<>]*[^\\/:\*\?\|<>\. ]))?$";
            Regex r = new Regex(DmitriyBorysovPattern);

            return r.IsMatch(path);
        }
    }
}
