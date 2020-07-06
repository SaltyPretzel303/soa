using System.Collections.Generic;
using SensorService.Configuration;

namespace SensorService.Data
{
	public interface IReader
	{

		List<List<string>> getDataFrom(int index);

		List<string> getHeader();

		FromTo getSamplesRange();

	}
}