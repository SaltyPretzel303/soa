import java.io.IOException;
import java.nio.file.Files;
import java.nio.file.Paths;
import java.util.concurrent.TimeoutException;

import org.json.JSONObject;

import com.rabbitmq.client.AMQP.Channel;
import com.rabbitmq.client.Connection;
import com.rabbitmq.client.ConnectionFactory;

public class Launcher {

	public static void main(String[] args) {

		try {

			String s_config = new String(Files.readAllBytes(Paths.get("./config")));
			JSONObject j_config = new JSONObject(s_config);

			String rabbit_address = j_config.getString("rabbit_address");

			System.out.println("Rabbit address: " + rabbit_address);

			ConnectionFactory conn_factory = new ConnectionFactory();
			conn_factory.setHost("localhost");
			conn_factory.setPort(5672);

			try (Connection connection = conn_factory.newConnection()) {

				System.out.println("Connection established ... ");

				Channel channel = (Channel) connection.createChannel();

				System.out.println("Ok connection and channel ... ");

			} catch (TimeoutException e) {
				// Auto-generated catch block
				e.printStackTrace();
			}

		} catch (IOException e) {
			// Auto-generated catch block
			e.printStackTrace();

		}

	}

}
