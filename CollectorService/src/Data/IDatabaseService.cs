using System;
using Newtonsoft.Json.Linq;

namespace CollectorService.Data
{
	public interface IDatabaseService
	{
		void pushToUser(String user_name, JArray values);

		String getAllRecords();

	}
}