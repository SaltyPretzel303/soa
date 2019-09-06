using ServiceObserver.Storage;

namespace ServiceObserver.Report.Processor
{
	public interface ReportProcessor
	{

		void processReport(ServiceReportEvent report);

	}
}