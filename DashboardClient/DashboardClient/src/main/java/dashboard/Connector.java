package dashboard;

import java.util.Date;
import java.util.List;

import org.json.JSONObject;

/**
 * @author pereca
 *
 *	Used as API for DashboardService
 *
 */
public interface Connector {

	/**
	 * Returns all events from all sensors.
	 * 
	 */
	List<JSONObject> getEvents();

	/**
	 * @param fromMomemnt
	 * @param toMoment
	 * @return List of json objects representing events from all sensors that happened between fromMoment to toMoment
	 */
	List<JSONObject> getEvents(Date fromMoment, Date toMoment);

	/**
	 * @param sensorName
	 * @return List of json objects representing events from single sensor (sensorName).
	 */
	List<JSONObject> getEvents(String sensorName);

	/**
	 * @param sensorName
	 * @param fromMoment
	 * @param toMoment
	 * @return List of json objects representint events from single sensor (sensorName) that happened between fromMoment to toMoment
	 */
	List<JSONObject> getEvents(String sensorName, Date fromMoment, Date toMoment);

}
