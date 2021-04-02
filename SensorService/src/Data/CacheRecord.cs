using System.Collections.Generic;
using CommunicationModel;

public class CacheRecord
{
	public List<string> Header { get; set; }
	public List<SensorValues> Records { get; set; }
	// each element in this list represents one row from csv file

	public CacheRecord(List<string> header, SensorValues newRecord)
	{
		Header = header;
		Records = new List<SensorValues>();
		this.Records.Add(newRecord);
	}

	public CacheRecord(List<string> header, List<SensorValues> newRecords)
	{
		Header = header;
		Records = new List<SensorValues>(newRecords);
	}

}
