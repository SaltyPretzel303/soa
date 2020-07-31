namespace SensorRegistry.Logger
{
	public interface ILogger
	{
		string LogMessage(string message);

		string LogError(string error);

	}
}