package json;

import com.google.gson.GsonBuilder;

public class JsonUtil {
	public static String serialize(Object obj) {
		return new GsonBuilder().create().toJson(obj);
	}

	public static String prettySerialize(Object obj) {
		return new GsonBuilder().setPrettyPrinting().create().toJson(obj);
	}

	public static <T> T deserialize(String strContent, Class<T> objType) {
		return new GsonBuilder().create().fromJson(strContent, objType);
	}
}
