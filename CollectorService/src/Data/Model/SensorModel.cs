using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CollectorService.Data
{
	public class SensorModel
	{
		[BsonId]
		public ObjectId id { get; set; }

		public string sensorName { get; set; }

		[BsonIgnore]
		public List<SensorValuesModel> values { get; set; }

		public SensorModel()
		{
			this.values = new List<SensorValuesModel>();
		}

		public SensorModel(string sensorName)
		{
			this.sensorName = sensorName;
			this.values = new List<SensorValuesModel>();
		}

		public SensorModel(string sensorName, List<SensorValuesModel> records)
		{
			this.sensorName = sensorName;
			this.values = records;
		}
	}
}