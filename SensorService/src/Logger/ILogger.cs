namespace SensorService.Logger
{
	public interface ILogger
	{
		void logMessage(string message, bool online = true);

		void logError(string error, bool online = true);

	}
}