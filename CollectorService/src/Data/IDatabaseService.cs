using System;
using Newtonsoft.Json.Linq;

namespace CRUDService.Data
{
	public interface IDatabaseService
	{
		void pushToUser(String user_name, JArray values);

		String getAllRecords();

	}
}