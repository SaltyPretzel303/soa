using Newtonsoft.Json.Linq;

namespace SensorService.src
{
	public class CliArgsParser
	{

		public JObject json_version { get; private set; }

		public bool success { get; private set; }

		public static string UNKNOWN_CLI_ARG = "unknown";

		public CliArgsParser(string[] args)
		{

			string arg_name = "";
			string arg_value = "";
			string temp_arg = "";

			int i = 0;
			while (i < args.Length)
			{

				temp_arg = args[i];
				if (temp_arg.Substring(0, 2).Equals("--"))
				{
					// temp_arg= --some_arg
					// arg_name= some_arg
					arg_name = temp_arg.Substring(2);
				}
				else
				{
					success = false;
					return;
				}

				if (i + 1 < args.Length)
				{
					arg_value = args[i + 1];
				}

				json_version.Add(arg_name, arg_value);

				i += 2;

			}

			success = true;

		}

		public string getOne(string arg_name)
		{

			JToken arg_value = null;
			if (this.json_version.TryGetValue(arg_name, out arg_value))
			{

				return arg_value.ToString();

			}

			return CliArgsParser.UNKNOWN_CLI_ARG;
		}

	}
}