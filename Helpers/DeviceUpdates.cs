using System.Management;

namespace ANDREICSLIB.Helpers
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class DeviceUpdates
    {
        /// <summary>
        /// Autodetects the COM port.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <returns></returns>
        public static string AutodetectCOMPort(string description)
        {
            var connectionScope = new ManagementScope();
            var serialQuery = new SelectQuery("SELECT * FROM Win32_SerialPort");
            var searcher = new ManagementObjectSearcher(connectionScope, serialQuery);

            try
            {
                foreach (ManagementObject item in searcher.Get())
                {
                    var desc = item["Description"].ToString();
                    var deviceId = item["DeviceID"].ToString();

                    if (desc.Contains(description))
                    {
                        return deviceId;
                    }
                }
            }
            catch (ManagementException)
            {
            }

            return null;
        }
    }
}