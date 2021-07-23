using System.Collections.Generic;
using CommunicationModel;

public class CacheRecord
{
	public string CsvHeader { get; set; }
	public List<string> CsvRecords { get; set; }
	// each element in this list represents one row from csv file

	public CacheRecord(string header, string newRecord)
	{
		CsvHeader = header;
		CsvRecords = new List<string>();
		this.CsvRecords.Add(newRecord);
	}

	public CacheRecord(string header, List<string> newRecords)
	{
		CsvHeader = header;
		CsvRecords = new List<string>(newRecords);
	}

}
