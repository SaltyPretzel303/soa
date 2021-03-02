using ServiceObserver.Data;

namespace ServiceObserver.Report.Processor
{
	public interface ReportProcessor
	{

		void processReport(ServiceReportEvent report);

	}
}