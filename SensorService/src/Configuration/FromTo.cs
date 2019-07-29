namespace SensorService.Configuration
{
	public class FromTo
	{

		public int From { get; set; }
		public int To { get; set; }

		public FromTo(int from, int to)
		{
			this.From = from;
			this.To = to;
		}

	}
}