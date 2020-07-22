
// TODO this class should be extracted to communication model 
// or some similar project included in all other projects
public class ServiceLog
{

	public string Tag { get; set; }
	public string Time { get; set; }
	public string Content { get; set; }

	public ServiceLog(string tag, string time, string content)
	{
		Tag = tag;
		Time = time;
		Content = content;
	}
}