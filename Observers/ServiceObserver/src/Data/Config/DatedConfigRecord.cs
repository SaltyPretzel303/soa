using System;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using Newtonsoft.Json.Linq;

public class DatedConfigRecord
{
	public BsonDocument configRecord;
	public DateTime backupDate;

	public DatedConfigRecord(JObject configRecord, DateTime backupDate)
	{
		string txtRecord = configRecord.ToString();

		this.configRecord = BsonDocument.Parse(txtRecord);

		this.backupDate = backupDate;
	}

	public JObject AsJsonConfig()
	{
		if (this.configRecord == null)
		{
			return null;
		}

		// this should remove bson specific fields
		// but this record doesn't have those anyway so ... 
		JsonWriterSettings toStringSett = new JsonWriterSettings
		{
			OutputMode = JsonOutputMode.Strict
		};

		// string txtRecord = this.configRecord.ToJson(toStringSett);
		string txtRecord = this.configRecord.ToString();

		return JObject.Parse(txtRecord);
	}


	public override string ToString()
	{
		return Newtonsoft.Json.JsonConvert.SerializeObject(this);
	}

}