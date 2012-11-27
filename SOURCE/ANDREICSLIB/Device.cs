using System;
using System.Management;

namespace ANDREICSLIB
{
	public abstract class DeviceUpdates
	{
		public static String AutodetectCOMPort(String description)
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
