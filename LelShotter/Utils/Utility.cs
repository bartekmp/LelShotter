using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LelShotter.Utils
{
    public static class Utility
    {
        public static bool CreateDirectory(string path)
        {
            try
            {
                Directory.CreateDirectory(path);
                return true;
            }
            catch (IOException ioException)
            {
                Logger.LogError($"Cannot create directory {path}, reason: {ioException.Message}");
            }
            catch (UnauthorizedAccessException unauthorizedAccess)
            {
                Logger.LogError($"Insufficient privileges to create directory {path}, reason: {unauthorizedAccess.Message}");
            }

            return false;
        }
    }
}
