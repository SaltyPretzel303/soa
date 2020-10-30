namespace CommunicationModel.RestModels
{
	public class UpdateConfigArg
	{

		public string AdditionalData { get; set; }
		public string TxtConfig { get; set; }

		public UpdateConfigArg(string additionalData, string txtConfig)
		{
			this.AdditionalData = additionalData;
			this.TxtConfig = txtConfig;
		}

		public UpdateConfigArg()
		{

		}

	}
}