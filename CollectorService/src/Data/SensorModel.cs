using System.Collections.Generic;
using CommunicationModel;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CollectorService.Data
{
	public class SensorModel
	{
		[BsonId]
		public ObjectId id { get; set; }

		public string sensorName { get; set; }

		public List<SensorValues> records { get; set; }

		public SensorModel()
		{
			this.records = new List<SensorValues>();
		}

		public SensorModel(string sensorName, List<SensorValues> records)
		{
			this.sensorName = sensorName;
			this.records = records;
		}
	}
}