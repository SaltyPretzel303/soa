package dashboard;

import java.util.Date;
import java.util.List;

import org.json.JSONObject;

public class CliConnector implements Connector{

	@Override
	public List<JSONObject> getEvents() {
		
		return null;
	}

	@Override
	public List<JSONObject> getEvents(String sensorName) {
		
		return null;
	}

	@Override
	public List<JSONObject> getEvents(String sensorName, Date fromMoment, Date toMoment) {
		
		return null;
	}

	@Override
	public List<JSONObject> getEvents(Date fromMomemnt, Date toMoment) {
		
		return null;
	}

}
