using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CommunicationModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SensorService.Configuration;

namespace SensorService.Data
{
	public class FsReaderCache : IDataCacheManager
	{

		private ServiceConfiguration config;

		private static object dirLock;

		// private static ConcurrentDictionary<string, object> locks;

		private static ConcurrentDictionary<string, FsSensorDataInfo> SensorsInfo;

		public FsReaderCache()
		{
			this.config = ServiceConfiguration.Instance;

			if (dirLock == null)
			{
				dirLock = new object();
			}
			if (SensorsInfo == null)
			{
				SensorsInfo = new ConcurrentDictionary<string, FsSensorDataInfo>();
			}

		}

		private void prepareFs()
		{
			string path = $"{config.fsCachePath}";
			if (!Directory.Exists(path))
			{
				try
				{

					lock (dirLock)
					{
						if (!Directory.Exists(path))
						{
							Directory.CreateDirectory(path);
						}
					}

				}
				catch (UnauthorizedAccessException)
				{
					Console.WriteLine("You don't have permission to use this path for cache: ");
					Console.WriteLine($"\t cache path: {path}");
				}
			}
		}

		private FsSensorDataInfo getSensorInfo(string sensorName)
		{
			FsSensorDataInfo sensorInfo = null;
			bool infoExists = SensorsInfo.TryGetValue(sensorName, out sensorInfo);

			if (!infoExists)
			{
				sensorInfo = new FsSensorDataInfo(sensorName, generatePath(sensorName));
				SensorsInfo.TryAdd(sensorName, sensorInfo);
			}

			return sensorInfo;
		}

		public void AddData(string sensorName, List<string> header, SensorValues newRecord)
		{
			prepareFs();

			var sensorInfo = this.getSensorInfo(sensorName);

			string str_values_line = valuesToString(newRecord);
			string[] arr_values = { str_values_line };
			int valuesLength = str_values_line.Length;

			if (sensorInfo.ReadIndex == 0)
			{
				// sensorInfo.CsvHeader = header;
				string str_header = headerToString(header);
				string[] arr_header = { str_header };
				int headerLength = str_header.Length;

				var borders = new LineBorders()
				{
					StartPos = headerLength + 1,
					EndPos = headerLength + 1 + valuesLength
				};

				lock (sensorInfo.fileLock)
				{
					// this will clear file - handy in development
					File.Delete(sensorInfo.FileName);

					File.AppendAllLines(sensorInfo.FileName, arr_header);
					File.AppendAllLines(sensorInfo.FileName, arr_values);

					sensorInfo.Lines.Add(borders);
				}

			}
			else
			{
				var lastBorders = sensorInfo.getLastLine();
				File.AppendAllLines(sensorInfo.FileName, arr_values);

				var borders = new LineBorders()
				{
					StartPos = lastBorders.EndPos + 1,
					EndPos = lastBorders.EndPos + 1 + valuesLength
				};

				sensorInfo.Lines.Add(borders);
			}

			sensorInfo.ReadIndex++;

		}

		private string headerToString(List<string> header)
		{
			return string.Join(",", header);
		}

		private string valuesToString(SensorValues values)
		{
			return JsonConvert.SerializeObject(values);
		}

		public CacheRecord GetAllSensorRecords(string sensorName)
		{
			return GetSensorRecordsFrom(sensorName, 0);
		}

		public int GetLastReadIndex(string sensorName)
		{
			return getSensorInfo(sensorName).ReadIndex;
		}

		public CacheRecord GetSensorRecordsFrom(string sensorName, int index)
		{
			var info = getSensorInfo(sensorName);
			if (!string.IsNullOrEmpty(sensorName)
				&& info.ReadIndex > 0
				&& index < info.ReadIndex
				&& File.Exists(info.FileName))
			{
				List<string> lines = new List<string>();

				var firstBorder = info.Lines[0];
				int headerLen = firstBorder.StartPos - 1;
				byte[] headerBuff = new byte[headerLen];

				int linesCount = info.ReadIndex - index;
				List<byte[]> valueLines = new List<byte[]>();
				for (int i = 0; i < linesCount; i++)
				{
					var borders = info.Lines[index + i];
					valueLines.Add(new byte[borders.EndPos - borders.StartPos]);
				}

				lock (info.fileLock)
				{
					using (var reader = File.OpenRead(info.FileName))
					{

						reader.Position = 0;
						reader.Read(headerBuff, 0, headerLen);

						for (int i = 0; i < linesCount; i++)
						{
							var borders = info.Lines[index + i];
							var len = borders.EndPos - borders.StartPos;

							reader.Position = borders.StartPos;
							reader.Read(valueLines[i], 0, len);
						}

					}
				}

				List<SensorValues> objList = valueLines.Select((byteValue) =>
				{
					string strValue = Encoding.UTF8.GetString(byteValue);
					return (JObject.Parse(strValue).ToObject<SensorValues>());
				})
				.ToList();

				List<string> listHeader =
					(Encoding.UTF8.GetString(headerBuff))
					.Split(",")
					.ToList();

				// return new CacheRecord(listHeader, objList);
				return null;
			}

			return null;
		}

		private string generatePath(string sensorName)
		{
			return $"{config.fsCachePath}"
				+ $"{sensorName}"
				+ $"{config.fsCacheFileExtension}";
		}

		public void AddData(string sensorName, string csvHeader, string csvValues)
		{
			throw new NotImplementedException();
		}
	}
}