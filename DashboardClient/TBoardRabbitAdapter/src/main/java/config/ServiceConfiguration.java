package config;

import java.io.IOException;
import java.nio.file.Files;
import java.nio.file.Paths;

public class ServiceConfiguration {

	public static final String CONFIG_PATH = "./service_config.json";
	public static final String DEV_STAGE = "Development";
	public static final String PROD_STAGE = "Production";

	public String stage;

	public ConfigFields Development;
	public ConfigFields Production;

	private static ServiceConfiguration instance;

	public static ConfigFields getInstance() {
		if (instance == null) {
			instance = readConfig();

			// var strInstance = json.JsonUtil.serialize(instance);
			// System.out.println("\nWe got this config: " + strInstance +
			// "\n");
			System.out.println("Config found, using: "
					+ instance.stage
					+ " configuration ... ");
		}

		return instance.getCurrentStage();
	}

	public static ServiceConfiguration readConfig() {

		var path = Paths.get(CONFIG_PATH);
		try {

			var configLines = Files.readAllLines(path);
			var strContent = String.join("", configLines);

			if (strContent != null && strContent.length() > 0) {
				var config = json.JsonUtil.deserialize(strContent,
						ServiceConfiguration.class);
				return config;
			}

			return null;
		} catch (IOException e) {
			System.out.println("Exception while reading configuration ... ");
			System.out.println("Exc: " + e.getMessage());
		}

		return null;
	}

	public ConfigFields getCurrentStage() {
		if (stage.equals(DEV_STAGE)) {
			return Development;
		} else if (stage.equals(PROD_STAGE)) {
			return Production;
		} else {
			System.out.println("Unknown stage value ("
					+ stage
					+ ") found in config file ...");
			System.out
					.println("Using development configuration as default ... ");
			return this.Development;
		}
	}

}
