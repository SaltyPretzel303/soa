using System.Collections.Generic;

public class CacheRecord
{
	public List<string> Header { get; set; }
	public List<string> Records { get; set; }
	// each element in this list represents one row from csv file
	// string of comma separated values

	public CacheRecord(List<string> header, string newRecord)
	{
		Header = header;
		Records = new List<string>();
		this.Records.Add(newRecord);
	}

	public CacheRecord(List<string> header, List<string> newRecords)
	{
		Header = header;
		Records = new List<string>(newRecords);
	}

}
