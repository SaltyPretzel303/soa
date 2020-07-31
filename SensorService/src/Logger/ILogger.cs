namespace SensorService.Logger
{
	// TODO this interface should be renamed, ILogger already exists in default microsoft libs
	public interface ILogger
	{
		void logMessage(string message);

		void logError(string error);

	}
}