using System.Collections.Generic;

namespace CommunicationModel
{

	// object representation of a single row of .csv file

	public class SensorValues
	{
		public long timestamp { get; set; }
		public string raw_acc_magnitude_stats_mean { get; set; }
		public string raw_acc_magnitude_stats_std { get; set; }
		public string raw_acc_magnitude_stats_moment3 { get; set; }
		public string raw_acc_magnitude_stats_moment4 { get; set; }
		public string raw_acc_magnitude_stats_percentile25 { get; set; }
		public string raw_acc_magnitude_stats_percentile50 { get; set; }
		public string raw_acc_magnitude_stats_percentile75 { get; set; }
		public string raw_acc_magnitude_stats_value_entropy { get; set; }
		public string raw_acc_magnitude_stats_time_entropy { get; set; }
		public string raw_acc_magnitude_spectrum_log_energy_band0 { get; set; }
		public string raw_acc_magnitude_spectrum_log_energy_band1 { get; set; }
		public string raw_acc_magnitude_spectrum_log_energy_band2 { get; set; }
		public string raw_acc_magnitude_spectrum_log_energy_band3 { get; set; }
		public string raw_acc_magnitude_spectrum_log_energy_band4 { get; set; }
		public string raw_acc_magnitude_spectrum_spectral_entropy { get; set; }
		public string raw_acc_magnitude_autocorrelation_period { get; set; }
		public string raw_acc_magnitude_autocorrelation_normalized_ac { get; set; }
		public string raw_acc_3d_mean_x { get; set; }
		public string raw_acc_3d_mean_y { get; set; }
		public string raw_acc_3d_mean_z { get; set; }
		public string raw_acc_3d_std_x { get; set; }
		public string raw_acc_3d_std_y { get; set; }
		public string raw_acc_3d_std_z { get; set; }
		public string raw_acc_3d_ro_xy { get; set; }
		public string raw_acc_3d_ro_xz { get; set; }
		public string raw_acc_3d_ro_yz { get; set; }
		public string proc_gyro_magnitude_stats_mean { get; set; }
		public string proc_gyro_magnitude_stats_std { get; set; }
		public string proc_gyro_magnitude_stats_moment3 { get; set; }
		public string proc_gyro_magnitude_stats_moment4 { get; set; }
		public string proc_gyro_magnitude_stats_percentile25 { get; set; }
		public string proc_gyro_magnitude_stats_percentile50 { get; set; }
		public string proc_gyro_magnitude_stats_percentile75 { get; set; }
		public string proc_gyro_magnitude_stats_value_entropy { get; set; }
		public string proc_gyro_magnitude_stats_time_entropy { get; set; }
		public string proc_gyro_magnitude_spectrum_log_energy_band0 { get; set; }
		public string proc_gyro_magnitude_spectrum_log_energy_band1 { get; set; }
		public string proc_gyro_magnitude_spectrum_log_energy_band2 { get; set; }
		public string proc_gyro_magnitude_spectrum_log_energy_band3 { get; set; }
		public string proc_gyro_magnitude_spectrum_log_energy_band4 { get; set; }
		public string proc_gyro_magnitude_spectrum_spectral_entropy { get; set; }
		public string proc_gyro_magnitude_autocorrelation_period { get; set; }
		public string proc_gyro_magnitude_autocorrelation_normalized_ac { get; set; }
		public string proc_gyro_3d_mean_x { get; set; }
		public string proc_gyro_3d_mean_y { get; set; }
		public string proc_gyro_3d_mean_z { get; set; }
		public string proc_gyro_3d_std_x { get; set; }
		public string proc_gyro_3d_std_y { get; set; }
		public string proc_gyro_3d_std_z { get; set; }
		public string proc_gyro_3d_ro_xy { get; set; }
		public string proc_gyro_3d_ro_xz { get; set; }
		public string proc_gyro_3d_ro_yz { get; set; }
		public string raw_magnet_magnitude_stats_mean { get; set; }
		public string raw_magnet_magnitude_stats_std { get; set; }
		public string raw_magnet_magnitude_stats_moment3 { get; set; }
		public string raw_magnet_magnitude_stats_moment4 { get; set; }
		public string raw_magnet_magnitude_stats_percentile25 { get; set; }
		public string raw_magnet_magnitude_stats_percentile50 { get; set; }
		public string raw_magnet_magnitude_stats_percentile75 { get; set; }
		public string raw_magnet_magnitude_stats_value_entropy { get; set; }
		public string raw_magnet_magnitude_stats_time_entropy { get; set; }
		public string raw_magnet_magnitude_spectrum_log_energy_band0 { get; set; }
		public string raw_magnet_magnitude_spectrum_log_energy_band1 { get; set; }
		public string raw_magnet_magnitude_spectrum_log_energy_band2 { get; set; }
		public string raw_magnet_magnitude_spectrum_log_energy_band3 { get; set; }
		public string raw_magnet_magnitude_spectrum_log_energy_band4 { get; set; }
		public string raw_magnet_magnitude_spectrum_spectral_entropy { get; set; }
		public string raw_magnet_magnitude_autocorrelation_period { get; set; }
		public string raw_magnet_magnitude_autocorrelation_normalized_ac { get; set; }
		public string raw_magnet_3d_mean_x { get; set; }
		public string raw_magnet_3d_mean_y { get; set; }
		public string raw_magnet_3d_mean_z { get; set; }
		public string raw_magnet_3d_std_x { get; set; }
		public string raw_magnet_3d_std_y { get; set; }
		public string raw_magnet_3d_std_z { get; set; }
		public string raw_magnet_3d_ro_xy { get; set; }
		public string raw_magnet_3d_ro_xz { get; set; }
		public string raw_magnet_3d_ro_yz { get; set; }
		public string raw_magnet_avr_cosine_similarity_lag_range0 { get; set; }
		public string raw_magnet_avr_cosine_similarity_lag_range1 { get; set; }
		public string raw_magnet_avr_cosine_similarity_lag_range2 { get; set; }
		public string raw_magnet_avr_cosine_similarity_lag_range3 { get; set; }
		public string raw_magnet_avr_cosine_similarity_lag_range4 { get; set; }
		public string watch_acceleration_magnitude_stats_mean { get; set; }
		public string watch_acceleration_magnitude_stats_std { get; set; }
		public string watch_acceleration_magnitude_stats_moment3 { get; set; }
		public string watch_acceleration_magnitude_stats_moment4 { get; set; }
		public string watch_acceleration_magnitude_stats_percentile25 { get; set; }
		public string watch_acceleration_magnitude_stats_percentile50 { get; set; }
		public string watch_acceleration_magnitude_stats_percentile75 { get; set; }
		public string watch_acceleration_magnitude_stats_value_entropy { get; set; }
		public string watch_acceleration_magnitude_stats_time_entropy { get; set; }
		public string watch_acceleration_magnitude_spectrum_log_energy_band0 { get; set; }
		public string watch_acceleration_magnitude_spectrum_log_energy_band1 { get; set; }
		public string watch_acceleration_magnitude_spectrum_log_energy_band2 { get; set; }
		public string watch_acceleration_magnitude_spectrum_log_energy_band3 { get; set; }
		public string watch_acceleration_magnitude_spectrum_log_energy_band4 { get; set; }
		public string watch_acceleration_magnitude_spectrum_spectral_entropy { get; set; }
		public string watch_acceleration_magnitude_autocorrelation_period { get; set; }
		public string watch_acceleration_magnitude_autocorrelation_normalized_ac { get; set; }
		public string watch_acceleration_3d_mean_x { get; set; }
		public string watch_acceleration_3d_mean_y { get; set; }
		public string watch_acceleration_3d_mean_z { get; set; }
		public string watch_acceleration_3d_std_x { get; set; }
		public string watch_acceleration_3d_std_y { get; set; }
		public string watch_acceleration_3d_std_z { get; set; }
		public string watch_acceleration_3d_ro_xy { get; set; }
		public string watch_acceleration_3d_ro_xz { get; set; }
		public string watch_acceleration_3d_ro_yz { get; set; }
		public string watch_acceleration_spectrum_x_log_energy_band0 { get; set; }
		public string watch_acceleration_spectrum_x_log_energy_band1 { get; set; }
		public string watch_acceleration_spectrum_x_log_energy_band2 { get; set; }
		public string watch_acceleration_spectrum_x_log_energy_band3 { get; set; }
		public string watch_acceleration_spectrum_x_log_energy_band4 { get; set; }
		public string watch_acceleration_spectrum_y_log_energy_band0 { get; set; }
		public string watch_acceleration_spectrum_y_log_energy_band1 { get; set; }
		public string watch_acceleration_spectrum_y_log_energy_band2 { get; set; }
		public string watch_acceleration_spectrum_y_log_energy_band3 { get; set; }
		public string watch_acceleration_spectrum_y_log_energy_band4 { get; set; }
		public string watch_acceleration_spectrum_z_log_energy_band0 { get; set; }
		public string watch_acceleration_spectrum_z_log_energy_band1 { get; set; }
		public string watch_acceleration_spectrum_z_log_energy_band2 { get; set; }
		public string watch_acceleration_spectrum_z_log_energy_band3 { get; set; }
		public string watch_acceleration_spectrum_z_log_energy_band4 { get; set; }
		public string watch_acceleration_relative_directions_avr_cosine_similarity_lag_range0 { get; set; }
		public string watch_acceleration_relative_directions_avr_cosine_similarity_lag_range1 { get; set; }
		public string watch_acceleration_relative_directions_avr_cosine_similarity_lag_range2 { get; set; }
		public string watch_acceleration_relative_directions_avr_cosine_similarity_lag_range3 { get; set; }
		public string watch_acceleration_relative_directions_avr_cosine_similarity_lag_range4 { get; set; }
		public string watch_heading_mean_cos { get; set; }
		public string watch_heading_std_cos { get; set; }
		public string watch_heading_mom3_cos { get; set; }
		public string watch_heading_mom4_cos { get; set; }
		public string watch_heading_mean_sin { get; set; }
		public string watch_heading_std_sin { get; set; }
		public string watch_heading_mom3_sin { get; set; }
		public string watch_heading_mom4_sin { get; set; }
		public string watch_heading_entropy_8bins { get; set; }
		public string location_num_valid_updates { get; set; }
		public string location_log_latitude_range { get; set; }
		public string location_log_longitude_range { get; set; }
		public string location_min_altitude { get; set; }
		public string location_max_altitude { get; set; }
		public string location_min_speed { get; set; }
		public string location_max_speed { get; set; }
		public string location_best_horizontal_accuracy { get; set; }
		public string location_best_vertical_accuracy { get; set; }
		public string location_diameter { get; set; }
		public string location_log_diameter { get; set; }
		public string location_quick_features_std_lat { get; set; }
		public string location_quick_features_std_long { get; set; }
		public string location_quick_features_lat_change { get; set; }
		public string location_quick_features_long_change { get; set; }
		public string location_quick_features_mean_abs_lat_deriv { get; set; }
		public string location_quick_features_mean_abs_long_deriv { get; set; }
		public string audio_naive_mfcc0_mean { get; set; }
		public string audio_naive_mfcc1_mean { get; set; }
		public string audio_naive_mfcc2_mean { get; set; }
		public string audio_naive_mfcc3_mean { get; set; }
		public string audio_naive_mfcc4_mean { get; set; }
		public string audio_naive_mfcc5_mean { get; set; }
		public string audio_naive_mfcc6_mean { get; set; }
		public string audio_naive_mfcc7_mean { get; set; }
		public string audio_naive_mfcc8_mean { get; set; }
		public string audio_naive_mfcc9_mean { get; set; }
		public string audio_naive_mfcc10_mean { get; set; }
		public string audio_naive_mfcc11_mean { get; set; }
		public string audio_naive_mfcc12_mean { get; set; }
		public string audio_naive_mfcc0_std { get; set; }
		public string audio_naive_mfcc1_std { get; set; }
		public string audio_naive_mfcc2_std { get; set; }
		public string audio_naive_mfcc3_std { get; set; }
		public string audio_naive_mfcc4_std { get; set; }
		public string audio_naive_mfcc5_std { get; set; }
		public string audio_naive_mfcc6_std { get; set; }
		public string audio_naive_mfcc7_std { get; set; }
		public string audio_naive_mfcc8_std { get; set; }
		public string audio_naive_mfcc9_std { get; set; }
		public string audio_naive_mfcc10_std { get; set; }
		public string audio_naive_mfcc11_std { get; set; }
		public string audio_naive_mfcc12_std { get; set; }
		public string audio_properties_max_abs_value { get; set; }
		public string audio_properties_normalization_multiplier { get; set; }
		public string discrete_app_state_is_active { get; set; }
		public string discrete_app_state_is_inactive { get; set; }
		public string discrete_app_state_is_background { get; set; }
		public string discrete_app_state_missing { get; set; }
		public string discrete_battery_plugged_is_ac { get; set; }
		public string discrete_battery_plugged_is_usb { get; set; }
		public string discrete_battery_plugged_is_wireless { get; set; }
		public string discrete_battery_plugged_missing { get; set; }
		public string discrete_battery_state_is_unknown { get; set; }
		public string discrete_battery_state_is_unplugged { get; set; }
		public string discrete_battery_state_is_not_charging { get; set; }
		public string discrete_battery_state_is_discharging { get; set; }
		public string discrete_battery_state_is_charging { get; set; }
		public string discrete_battery_state_is_full { get; set; }
		public string discrete_battery_state_missing { get; set; }
		public string discrete_on_the_phone_is_False { get; set; }
		public string discrete_on_the_phone_is_True { get; set; }
		public string discrete_on_the_phone_missing { get; set; }
		public string discrete_ringer_mode_is_normal { get; set; }
		public string discrete_ringer_mode_is_silent_no_vibrate { get; set; }
		public string discrete_ringer_mode_is_silent_with_vibrate { get; set; }
		public string discrete_ringer_mode_missing { get; set; }
		public string discrete_wifi_status_is_not_reachable { get; set; }
		public string discrete_wifi_status_is_reachable_via_wifi { get; set; }
		public string discrete_wifi_status_is_reachable_via_wwan { get; set; }
		public string discrete_wifi_status_missing { get; set; }
		public string lf_measurements_light { get; set; }
		public string lf_measurements_pressure { get; set; }
		public string lf_measurements_proximity_cm { get; set; }
		public string lf_measurements_proximity { get; set; }
		public string lf_measurements_relative_humidity { get; set; }
		public string lf_measurements_battery_level { get; set; }
		public string lf_measurements_screen_brightness { get; set; }
		public string lf_measurements_temperature_ambient { get; set; }
		public string discrete_time_of_day_between0and6 { get; set; }
		public string discrete_time_of_day_between3and9 { get; set; }
		public string discrete_time_of_day_between6and12 { get; set; }
		public string discrete_time_of_day_between9and15 { get; set; }
		public string discrete_time_of_day_between12and18 { get; set; }
		public string discrete_time_of_day_between15and21 { get; set; }
		public string discrete_time_of_day_between18and24 { get; set; }
		public string discrete_time_of_day_between21and3 { get; set; }
		public string label_LYING_DOWN { get; set; }
		public string label_SITTING { get; set; }
		public string label_FIX_walking { get; set; }
		public string label_FIX_running { get; set; }
		public string label_BICYCLING { get; set; }
		public string label_SLEEPING { get; set; }
		public string label_LAB_WORK { get; set; }
		public string label_IN_CLASS { get; set; }
		public string label_IN_A_MEETING { get; set; }
		public string label_LOC_main_workplace { get; set; }
		public string label_OR_indoors { get; set; }
		public string label_OR_outside { get; set; }
		public string label_IN_A_CAR { get; set; }
		public string label_ON_A_BUS { get; set; }
		public string label_DRIVE___I_M_THE_DRIVER { get; set; }
		public string label_DRIVE___I_M_A_PASSENGER { get; set; }
		public string label_LOC_home { get; set; }
		public string label_FIX_restaurant { get; set; }
		public string label_PHONE_IN_POCKET { get; set; }
		public string label_OR_exercise { get; set; }
		public string label_COOKING { get; set; }
		public string label_SHOPPING { get; set; }
		public string label_STROLLING { get; set; }
		public string label_DRINKING__ALCOHOL_ { get; set; }
		public string label_BATHING___SHOWER { get; set; }
		public string label_CLEANING { get; set; }
		public string label_DOING_LAUNDRY { get; set; }
		public string label_WASHING_DISHES { get; set; }
		public string label_WATCHING_TV { get; set; }
		public string label_SURFING_THE_INTERNET { get; set; }
		public string label_AT_A_PARTY { get; set; }
		public string label_AT_A_BAR { get; set; }
		public string label_LOC_beach { get; set; }
		public string label_SINGING { get; set; }
		public string label_TALKING { get; set; }
		public string label_COMPUTER_WORK { get; set; }
		public string label_EATING { get; set; }
		public string label_TOILET { get; set; }
		public string label_GROOMING { get; set; }
		public string label_DRESSING { get; set; }
		public string label_AT_THE_GYM { get; set; }
		public string label_STAIRS___GOING_UP { get; set; }
		public string label_STAIRS___GOING_DOWN { get; set; }
		public string label_ELEVATOR { get; set; }
		public string label_OR_standing { get; set; }
		public string label_AT_SCHOOL { get; set; }
		public string label_PHONE_IN_HAND { get; set; }
		public string label_PHONE_IN_BAG { get; set; }
		public string label_PHONE_ON_TABLE { get; set; }
		public string label_WITH_CO_WORKERS { get; set; }
		public string label_WITH_FRIENDS { get; set; }
		public string label_source { get; set; }

		public SensorValues(long timestamp, string raw_acc_magnitude_stats_mean, string raw_acc_magnitude_stats_std, string raw_acc_magnitude_stats_moment3, string raw_acc_magnitude_stats_moment4, string raw_acc_magnitude_stats_percentile25, string raw_acc_magnitude_stats_percentile50, string raw_acc_magnitude_stats_percentile75, string raw_acc_magnitude_stats_value_entropy, string raw_acc_magnitude_stats_time_entropy, string raw_acc_magnitude_spectrum_log_energy_band0, string raw_acc_magnitude_spectrum_log_energy_band1, string raw_acc_magnitude_spectrum_log_energy_band2, string raw_acc_magnitude_spectrum_log_energy_band3, string raw_acc_magnitude_spectrum_log_energy_band4, string raw_acc_magnitude_spectrum_spectral_entropy, string raw_acc_magnitude_autocorrelation_period, string raw_acc_magnitude_autocorrelation_normalized_ac, string raw_acc_3d_mean_x, string raw_acc_3d_mean_y, string raw_acc_3d_mean_z, string raw_acc_3d_std_x, string raw_acc_3d_std_y, string raw_acc_3d_std_z, string raw_acc_3d_ro_xy, string raw_acc_3d_ro_xz, string raw_acc_3d_ro_yz, string proc_gyro_magnitude_stats_mean, string proc_gyro_magnitude_stats_std, string proc_gyro_magnitude_stats_moment3, string proc_gyro_magnitude_stats_moment4, string proc_gyro_magnitude_stats_percentile25, string proc_gyro_magnitude_stats_percentile50, string proc_gyro_magnitude_stats_percentile75, string proc_gyro_magnitude_stats_value_entropy, string proc_gyro_magnitude_stats_time_entropy, string proc_gyro_magnitude_spectrum_log_energy_band0, string proc_gyro_magnitude_spectrum_log_energy_band1, string proc_gyro_magnitude_spectrum_log_energy_band2, string proc_gyro_magnitude_spectrum_log_energy_band3, string proc_gyro_magnitude_spectrum_log_energy_band4, string proc_gyro_magnitude_spectrum_spectral_entropy, string proc_gyro_magnitude_autocorrelation_period, string proc_gyro_magnitude_autocorrelation_normalized_ac, string proc_gyro_3d_mean_x, string proc_gyro_3d_mean_y, string proc_gyro_3d_mean_z, string proc_gyro_3d_std_x, string proc_gyro_3d_std_y, string proc_gyro_3d_std_z, string proc_gyro_3d_ro_xy, string proc_gyro_3d_ro_xz, string proc_gyro_3d_ro_yz, string raw_magnet_magnitude_stats_mean, string raw_magnet_magnitude_stats_std, string raw_magnet_magnitude_stats_moment3, string raw_magnet_magnitude_stats_moment4, string raw_magnet_magnitude_stats_percentile25, string raw_magnet_magnitude_stats_percentile50, string raw_magnet_magnitude_stats_percentile75, string raw_magnet_magnitude_stats_value_entropy, string raw_magnet_magnitude_stats_time_entropy, string raw_magnet_magnitude_spectrum_log_energy_band0, string raw_magnet_magnitude_spectrum_log_energy_band1, string raw_magnet_magnitude_spectrum_log_energy_band2, string raw_magnet_magnitude_spectrum_log_energy_band3, string raw_magnet_magnitude_spectrum_log_energy_band4, string raw_magnet_magnitude_spectrum_spectral_entropy, string raw_magnet_magnitude_autocorrelation_period, string raw_magnet_magnitude_autocorrelation_normalized_ac, string raw_magnet_3d_mean_x, string raw_magnet_3d_mean_y, string raw_magnet_3d_mean_z, string raw_magnet_3d_std_x, string raw_magnet_3d_std_y, string raw_magnet_3d_std_z, string raw_magnet_3d_ro_xy, string raw_magnet_3d_ro_xz, string raw_magnet_3d_ro_yz, string raw_magnet_avr_cosine_similarity_lag_range0, string raw_magnet_avr_cosine_similarity_lag_range1, string raw_magnet_avr_cosine_similarity_lag_range2, string raw_magnet_avr_cosine_similarity_lag_range3, string raw_magnet_avr_cosine_similarity_lag_range4, string watch_acceleration_magnitude_stats_mean, string watch_acceleration_magnitude_stats_std, string watch_acceleration_magnitude_stats_moment3, string watch_acceleration_magnitude_stats_moment4, string watch_acceleration_magnitude_stats_percentile25, string watch_acceleration_magnitude_stats_percentile50, string watch_acceleration_magnitude_stats_percentile75, string watch_acceleration_magnitude_stats_value_entropy, string watch_acceleration_magnitude_stats_time_entropy, string watch_acceleration_magnitude_spectrum_log_energy_band0, string watch_acceleration_magnitude_spectrum_log_energy_band1, string watch_acceleration_magnitude_spectrum_log_energy_band2, string watch_acceleration_magnitude_spectrum_log_energy_band3, string watch_acceleration_magnitude_spectrum_log_energy_band4, string watch_acceleration_magnitude_spectrum_spectral_entropy, string watch_acceleration_magnitude_autocorrelation_period, string watch_acceleration_magnitude_autocorrelation_normalized_ac, string watch_acceleration_3d_mean_x, string watch_acceleration_3d_mean_y, string watch_acceleration_3d_mean_z, string watch_acceleration_3d_std_x, string watch_acceleration_3d_std_y, string watch_acceleration_3d_std_z, string watch_acceleration_3d_ro_xy, string watch_acceleration_3d_ro_xz, string watch_acceleration_3d_ro_yz, string watch_acceleration_spectrum_x_log_energy_band0, string watch_acceleration_spectrum_x_log_energy_band1, string watch_acceleration_spectrum_x_log_energy_band2, string watch_acceleration_spectrum_x_log_energy_band3, string watch_acceleration_spectrum_x_log_energy_band4, string watch_acceleration_spectrum_y_log_energy_band0, string watch_acceleration_spectrum_y_log_energy_band1, string watch_acceleration_spectrum_y_log_energy_band2, string watch_acceleration_spectrum_y_log_energy_band3, string watch_acceleration_spectrum_y_log_energy_band4, string watch_acceleration_spectrum_z_log_energy_band0, string watch_acceleration_spectrum_z_log_energy_band1, string watch_acceleration_spectrum_z_log_energy_band2, string watch_acceleration_spectrum_z_log_energy_band3, string watch_acceleration_spectrum_z_log_energy_band4, string watch_acceleration_relative_directions_avr_cosine_similarity_lag_range0, string watch_acceleration_relative_directions_avr_cosine_similarity_lag_range1, string watch_acceleration_relative_directions_avr_cosine_similarity_lag_range2, string watch_acceleration_relative_directions_avr_cosine_similarity_lag_range3, string watch_acceleration_relative_directions_avr_cosine_similarity_lag_range4, string watch_heading_mean_cos, string watch_heading_std_cos, string watch_heading_mom3_cos, string watch_heading_mom4_cos, string watch_heading_mean_sin, string watch_heading_std_sin, string watch_heading_mom3_sin, string watch_heading_mom4_sin, string watch_heading_entropy_8bins, string location_num_valid_updates, string location_log_latitude_range, string location_log_longitude_range, string location_min_altitude, string location_max_altitude, string location_min_speed, string location_max_speed, string location_best_horizontal_accuracy, string location_best_vertical_accuracy, string location_diameter, string location_log_diameter, string location_quick_features_std_lat, string location_quick_features_std_long, string location_quick_features_lat_change, string location_quick_features_long_change, string location_quick_features_mean_abs_lat_deriv, string location_quick_features_mean_abs_long_deriv, string audio_naive_mfcc0_mean, string audio_naive_mfcc1_mean, string audio_naive_mfcc2_mean, string audio_naive_mfcc3_mean, string audio_naive_mfcc4_mean, string audio_naive_mfcc5_mean, string audio_naive_mfcc6_mean, string audio_naive_mfcc7_mean, string audio_naive_mfcc8_mean, string audio_naive_mfcc9_mean, string audio_naive_mfcc10_mean, string audio_naive_mfcc11_mean, string audio_naive_mfcc12_mean, string audio_naive_mfcc0_std, string audio_naive_mfcc1_std, string audio_naive_mfcc2_std, string audio_naive_mfcc3_std, string audio_naive_mfcc4_std, string audio_naive_mfcc5_std, string audio_naive_mfcc6_std, string audio_naive_mfcc7_std, string audio_naive_mfcc8_std, string audio_naive_mfcc9_std, string audio_naive_mfcc10_std, string audio_naive_mfcc11_std, string audio_naive_mfcc12_std, string audio_properties_max_abs_value, string audio_properties_normalization_multiplier, string discrete_app_state_is_active, string discrete_app_state_is_inactive, string discrete_app_state_is_background, string discrete_app_state_missing, string discrete_battery_plugged_is_ac, string discrete_battery_plugged_is_usb, string discrete_battery_plugged_is_wireless, string discrete_battery_plugged_missing, string discrete_battery_state_is_unknown, string discrete_battery_state_is_unplugged, string discrete_battery_state_is_not_charging, string discrete_battery_state_is_discharging, string discrete_battery_state_is_charging, string discrete_battery_state_is_full, string discrete_battery_state_missing, string discrete_on_the_phone_is_False, string discrete_on_the_phone_is_True, string discrete_on_the_phone_missing, string discrete_ringer_mode_is_normal, string discrete_ringer_mode_is_silent_no_vibrate, string discrete_ringer_mode_is_silent_with_vibrate, string discrete_ringer_mode_missing, string discrete_wifi_status_is_not_reachable, string discrete_wifi_status_is_reachable_via_wifi, string discrete_wifi_status_is_reachable_via_wwan, string discrete_wifi_status_missing, string lf_measurements_light, string lf_measurements_pressure, string lf_measurements_proximity_cm, string lf_measurements_proximity, string lf_measurements_relative_humidity, string lf_measurements_battery_level, string lf_measurements_screen_brightness, string lf_measurements_temperature_ambient, string discrete_time_of_day_between0and6, string discrete_time_of_day_between3and9, string discrete_time_of_day_between6and12, string discrete_time_of_day_between9and15, string discrete_time_of_day_between12and18, string discrete_time_of_day_between15and21, string discrete_time_of_day_between18and24, string discrete_time_of_day_between21and3, string label_LYING_DOWN, string label_SITTING, string label_FIX_walking, string label_FIX_running, string label_BICYCLING, string label_SLEEPING, string label_LAB_WORK, string label_IN_CLASS, string label_IN_A_MEETING, string label_LOC_main_workplace, string label_OR_indoors, string label_OR_outside, string label_IN_A_CAR, string label_ON_A_BUS, string label_DRIVE___I_M_THE_DRIVER, string label_DRIVE___I_M_A_PASSENGER, string label_LOC_home, string label_FIX_restaurant, string label_PHONE_IN_POCKET, string label_OR_exercise, string label_COOKING, string label_SHOPPING, string label_STROLLING, string label_DRINKING__ALCOHOL_, string label_BATHING___SHOWER, string label_CLEANING, string label_DOING_LAUNDRY, string label_WASHING_DISHES, string label_WATCHING_TV, string label_SURFING_THE_INTERNET, string label_AT_A_PARTY, string label_AT_A_BAR, string label_LOC_beach, string label_SINGING, string label_TALKING, string label_COMPUTER_WORK, string label_EATING, string label_TOILET, string label_GROOMING, string label_DRESSING, string label_AT_THE_GYM, string label_STAIRS___GOING_UP, string label_STAIRS___GOING_DOWN, string label_ELEVATOR, string label_OR_standing, string label_AT_SCHOOL, string label_PHONE_IN_HAND, string label_PHONE_IN_BAG, string label_PHONE_ON_TABLE, string label_WITH_CO_WORKERS, string label_WITH_FRIENDS, string label_source)
		{
			this.timestamp = timestamp;
			this.raw_acc_magnitude_stats_mean = raw_acc_magnitude_stats_mean;
			this.raw_acc_magnitude_stats_std = raw_acc_magnitude_stats_std;
			this.raw_acc_magnitude_stats_moment3 = raw_acc_magnitude_stats_moment3;
			this.raw_acc_magnitude_stats_moment4 = raw_acc_magnitude_stats_moment4;
			this.raw_acc_magnitude_stats_percentile25 = raw_acc_magnitude_stats_percentile25;
			this.raw_acc_magnitude_stats_percentile50 = raw_acc_magnitude_stats_percentile50;
			this.raw_acc_magnitude_stats_percentile75 = raw_acc_magnitude_stats_percentile75;
			this.raw_acc_magnitude_stats_value_entropy = raw_acc_magnitude_stats_value_entropy;
			this.raw_acc_magnitude_stats_time_entropy = raw_acc_magnitude_stats_time_entropy;
			this.raw_acc_magnitude_spectrum_log_energy_band0 = raw_acc_magnitude_spectrum_log_energy_band0;
			this.raw_acc_magnitude_spectrum_log_energy_band1 = raw_acc_magnitude_spectrum_log_energy_band1;
			this.raw_acc_magnitude_spectrum_log_energy_band2 = raw_acc_magnitude_spectrum_log_energy_band2;
			this.raw_acc_magnitude_spectrum_log_energy_band3 = raw_acc_magnitude_spectrum_log_energy_band3;
			this.raw_acc_magnitude_spectrum_log_energy_band4 = raw_acc_magnitude_spectrum_log_energy_band4;
			this.raw_acc_magnitude_spectrum_spectral_entropy = raw_acc_magnitude_spectrum_spectral_entropy;
			this.raw_acc_magnitude_autocorrelation_period = raw_acc_magnitude_autocorrelation_period;
			this.raw_acc_magnitude_autocorrelation_normalized_ac = raw_acc_magnitude_autocorrelation_normalized_ac;
			this.raw_acc_3d_mean_x = raw_acc_3d_mean_x;
			this.raw_acc_3d_mean_y = raw_acc_3d_mean_y;
			this.raw_acc_3d_mean_z = raw_acc_3d_mean_z;
			this.raw_acc_3d_std_x = raw_acc_3d_std_x;
			this.raw_acc_3d_std_y = raw_acc_3d_std_y;
			this.raw_acc_3d_std_z = raw_acc_3d_std_z;
			this.raw_acc_3d_ro_xy = raw_acc_3d_ro_xy;
			this.raw_acc_3d_ro_xz = raw_acc_3d_ro_xz;
			this.raw_acc_3d_ro_yz = raw_acc_3d_ro_yz;
			this.proc_gyro_magnitude_stats_mean = proc_gyro_magnitude_stats_mean;
			this.proc_gyro_magnitude_stats_std = proc_gyro_magnitude_stats_std;
			this.proc_gyro_magnitude_stats_moment3 = proc_gyro_magnitude_stats_moment3;
			this.proc_gyro_magnitude_stats_moment4 = proc_gyro_magnitude_stats_moment4;
			this.proc_gyro_magnitude_stats_percentile25 = proc_gyro_magnitude_stats_percentile25;
			this.proc_gyro_magnitude_stats_percentile50 = proc_gyro_magnitude_stats_percentile50;
			this.proc_gyro_magnitude_stats_percentile75 = proc_gyro_magnitude_stats_percentile75;
			this.proc_gyro_magnitude_stats_value_entropy = proc_gyro_magnitude_stats_value_entropy;
			this.proc_gyro_magnitude_stats_time_entropy = proc_gyro_magnitude_stats_time_entropy;
			this.proc_gyro_magnitude_spectrum_log_energy_band0 = proc_gyro_magnitude_spectrum_log_energy_band0;
			this.proc_gyro_magnitude_spectrum_log_energy_band1 = proc_gyro_magnitude_spectrum_log_energy_band1;
			this.proc_gyro_magnitude_spectrum_log_energy_band2 = proc_gyro_magnitude_spectrum_log_energy_band2;
			this.proc_gyro_magnitude_spectrum_log_energy_band3 = proc_gyro_magnitude_spectrum_log_energy_band3;
			this.proc_gyro_magnitude_spectrum_log_energy_band4 = proc_gyro_magnitude_spectrum_log_energy_band4;
			this.proc_gyro_magnitude_spectrum_spectral_entropy = proc_gyro_magnitude_spectrum_spectral_entropy;
			this.proc_gyro_magnitude_autocorrelation_period = proc_gyro_magnitude_autocorrelation_period;
			this.proc_gyro_magnitude_autocorrelation_normalized_ac = proc_gyro_magnitude_autocorrelation_normalized_ac;
			this.proc_gyro_3d_mean_x = proc_gyro_3d_mean_x;
			this.proc_gyro_3d_mean_y = proc_gyro_3d_mean_y;
			this.proc_gyro_3d_mean_z = proc_gyro_3d_mean_z;
			this.proc_gyro_3d_std_x = proc_gyro_3d_std_x;
			this.proc_gyro_3d_std_y = proc_gyro_3d_std_y;
			this.proc_gyro_3d_std_z = proc_gyro_3d_std_z;
			this.proc_gyro_3d_ro_xy = proc_gyro_3d_ro_xy;
			this.proc_gyro_3d_ro_xz = proc_gyro_3d_ro_xz;
			this.proc_gyro_3d_ro_yz = proc_gyro_3d_ro_yz;
			this.raw_magnet_magnitude_stats_mean = raw_magnet_magnitude_stats_mean;
			this.raw_magnet_magnitude_stats_std = raw_magnet_magnitude_stats_std;
			this.raw_magnet_magnitude_stats_moment3 = raw_magnet_magnitude_stats_moment3;
			this.raw_magnet_magnitude_stats_moment4 = raw_magnet_magnitude_stats_moment4;
			this.raw_magnet_magnitude_stats_percentile25 = raw_magnet_magnitude_stats_percentile25;
			this.raw_magnet_magnitude_stats_percentile50 = raw_magnet_magnitude_stats_percentile50;
			this.raw_magnet_magnitude_stats_percentile75 = raw_magnet_magnitude_stats_percentile75;
			this.raw_magnet_magnitude_stats_value_entropy = raw_magnet_magnitude_stats_value_entropy;
			this.raw_magnet_magnitude_stats_time_entropy = raw_magnet_magnitude_stats_time_entropy;
			this.raw_magnet_magnitude_spectrum_log_energy_band0 = raw_magnet_magnitude_spectrum_log_energy_band0;
			this.raw_magnet_magnitude_spectrum_log_energy_band1 = raw_magnet_magnitude_spectrum_log_energy_band1;
			this.raw_magnet_magnitude_spectrum_log_energy_band2 = raw_magnet_magnitude_spectrum_log_energy_band2;
			this.raw_magnet_magnitude_spectrum_log_energy_band3 = raw_magnet_magnitude_spectrum_log_energy_band3;
			this.raw_magnet_magnitude_spectrum_log_energy_band4 = raw_magnet_magnitude_spectrum_log_energy_band4;
			this.raw_magnet_magnitude_spectrum_spectral_entropy = raw_magnet_magnitude_spectrum_spectral_entropy;
			this.raw_magnet_magnitude_autocorrelation_period = raw_magnet_magnitude_autocorrelation_period;
			this.raw_magnet_magnitude_autocorrelation_normalized_ac = raw_magnet_magnitude_autocorrelation_normalized_ac;
			this.raw_magnet_3d_mean_x = raw_magnet_3d_mean_x;
			this.raw_magnet_3d_mean_y = raw_magnet_3d_mean_y;
			this.raw_magnet_3d_mean_z = raw_magnet_3d_mean_z;
			this.raw_magnet_3d_std_x = raw_magnet_3d_std_x;
			this.raw_magnet_3d_std_y = raw_magnet_3d_std_y;
			this.raw_magnet_3d_std_z = raw_magnet_3d_std_z;
			this.raw_magnet_3d_ro_xy = raw_magnet_3d_ro_xy;
			this.raw_magnet_3d_ro_xz = raw_magnet_3d_ro_xz;
			this.raw_magnet_3d_ro_yz = raw_magnet_3d_ro_yz;
			this.raw_magnet_avr_cosine_similarity_lag_range0 = raw_magnet_avr_cosine_similarity_lag_range0;
			this.raw_magnet_avr_cosine_similarity_lag_range1 = raw_magnet_avr_cosine_similarity_lag_range1;
			this.raw_magnet_avr_cosine_similarity_lag_range2 = raw_magnet_avr_cosine_similarity_lag_range2;
			this.raw_magnet_avr_cosine_similarity_lag_range3 = raw_magnet_avr_cosine_similarity_lag_range3;
			this.raw_magnet_avr_cosine_similarity_lag_range4 = raw_magnet_avr_cosine_similarity_lag_range4;
			this.watch_acceleration_magnitude_stats_mean = watch_acceleration_magnitude_stats_mean;
			this.watch_acceleration_magnitude_stats_std = watch_acceleration_magnitude_stats_std;
			this.watch_acceleration_magnitude_stats_moment3 = watch_acceleration_magnitude_stats_moment3;
			this.watch_acceleration_magnitude_stats_moment4 = watch_acceleration_magnitude_stats_moment4;
			this.watch_acceleration_magnitude_stats_percentile25 = watch_acceleration_magnitude_stats_percentile25;
			this.watch_acceleration_magnitude_stats_percentile50 = watch_acceleration_magnitude_stats_percentile50;
			this.watch_acceleration_magnitude_stats_percentile75 = watch_acceleration_magnitude_stats_percentile75;
			this.watch_acceleration_magnitude_stats_value_entropy = watch_acceleration_magnitude_stats_value_entropy;
			this.watch_acceleration_magnitude_stats_time_entropy = watch_acceleration_magnitude_stats_time_entropy;
			this.watch_acceleration_magnitude_spectrum_log_energy_band0 = watch_acceleration_magnitude_spectrum_log_energy_band0;
			this.watch_acceleration_magnitude_spectrum_log_energy_band1 = watch_acceleration_magnitude_spectrum_log_energy_band1;
			this.watch_acceleration_magnitude_spectrum_log_energy_band2 = watch_acceleration_magnitude_spectrum_log_energy_band2;
			this.watch_acceleration_magnitude_spectrum_log_energy_band3 = watch_acceleration_magnitude_spectrum_log_energy_band3;
			this.watch_acceleration_magnitude_spectrum_log_energy_band4 = watch_acceleration_magnitude_spectrum_log_energy_band4;
			this.watch_acceleration_magnitude_spectrum_spectral_entropy = watch_acceleration_magnitude_spectrum_spectral_entropy;
			this.watch_acceleration_magnitude_autocorrelation_period = watch_acceleration_magnitude_autocorrelation_period;
			this.watch_acceleration_magnitude_autocorrelation_normalized_ac = watch_acceleration_magnitude_autocorrelation_normalized_ac;
			this.watch_acceleration_3d_mean_x = watch_acceleration_3d_mean_x;
			this.watch_acceleration_3d_mean_y = watch_acceleration_3d_mean_y;
			this.watch_acceleration_3d_mean_z = watch_acceleration_3d_mean_z;
			this.watch_acceleration_3d_std_x = watch_acceleration_3d_std_x;
			this.watch_acceleration_3d_std_y = watch_acceleration_3d_std_y;
			this.watch_acceleration_3d_std_z = watch_acceleration_3d_std_z;
			this.watch_acceleration_3d_ro_xy = watch_acceleration_3d_ro_xy;
			this.watch_acceleration_3d_ro_xz = watch_acceleration_3d_ro_xz;
			this.watch_acceleration_3d_ro_yz = watch_acceleration_3d_ro_yz;
			this.watch_acceleration_spectrum_x_log_energy_band0 = watch_acceleration_spectrum_x_log_energy_band0;
			this.watch_acceleration_spectrum_x_log_energy_band1 = watch_acceleration_spectrum_x_log_energy_band1;
			this.watch_acceleration_spectrum_x_log_energy_band2 = watch_acceleration_spectrum_x_log_energy_band2;
			this.watch_acceleration_spectrum_x_log_energy_band3 = watch_acceleration_spectrum_x_log_energy_band3;
			this.watch_acceleration_spectrum_x_log_energy_band4 = watch_acceleration_spectrum_x_log_energy_band4;
			this.watch_acceleration_spectrum_y_log_energy_band0 = watch_acceleration_spectrum_y_log_energy_band0;
			this.watch_acceleration_spectrum_y_log_energy_band1 = watch_acceleration_spectrum_y_log_energy_band1;
			this.watch_acceleration_spectrum_y_log_energy_band2 = watch_acceleration_spectrum_y_log_energy_band2;
			this.watch_acceleration_spectrum_y_log_energy_band3 = watch_acceleration_spectrum_y_log_energy_band3;
			this.watch_acceleration_spectrum_y_log_energy_band4 = watch_acceleration_spectrum_y_log_energy_band4;
			this.watch_acceleration_spectrum_z_log_energy_band0 = watch_acceleration_spectrum_z_log_energy_band0;
			this.watch_acceleration_spectrum_z_log_energy_band1 = watch_acceleration_spectrum_z_log_energy_band1;
			this.watch_acceleration_spectrum_z_log_energy_band2 = watch_acceleration_spectrum_z_log_energy_band2;
			this.watch_acceleration_spectrum_z_log_energy_band3 = watch_acceleration_spectrum_z_log_energy_band3;
			this.watch_acceleration_spectrum_z_log_energy_band4 = watch_acceleration_spectrum_z_log_energy_band4;
			this.watch_acceleration_relative_directions_avr_cosine_similarity_lag_range0 = watch_acceleration_relative_directions_avr_cosine_similarity_lag_range0;
			this.watch_acceleration_relative_directions_avr_cosine_similarity_lag_range1 = watch_acceleration_relative_directions_avr_cosine_similarity_lag_range1;
			this.watch_acceleration_relative_directions_avr_cosine_similarity_lag_range2 = watch_acceleration_relative_directions_avr_cosine_similarity_lag_range2;
			this.watch_acceleration_relative_directions_avr_cosine_similarity_lag_range3 = watch_acceleration_relative_directions_avr_cosine_similarity_lag_range3;
			this.watch_acceleration_relative_directions_avr_cosine_similarity_lag_range4 = watch_acceleration_relative_directions_avr_cosine_similarity_lag_range4;
			this.watch_heading_mean_cos = watch_heading_mean_cos;
			this.watch_heading_std_cos = watch_heading_std_cos;
			this.watch_heading_mom3_cos = watch_heading_mom3_cos;
			this.watch_heading_mom4_cos = watch_heading_mom4_cos;
			this.watch_heading_mean_sin = watch_heading_mean_sin;
			this.watch_heading_std_sin = watch_heading_std_sin;
			this.watch_heading_mom3_sin = watch_heading_mom3_sin;
			this.watch_heading_mom4_sin = watch_heading_mom4_sin;
			this.watch_heading_entropy_8bins = watch_heading_entropy_8bins;
			this.location_num_valid_updates = location_num_valid_updates;
			this.location_log_latitude_range = location_log_latitude_range;
			this.location_log_longitude_range = location_log_longitude_range;
			this.location_min_altitude = location_min_altitude;
			this.location_max_altitude = location_max_altitude;
			this.location_min_speed = location_min_speed;
			this.location_max_speed = location_max_speed;
			this.location_best_horizontal_accuracy = location_best_horizontal_accuracy;
			this.location_best_vertical_accuracy = location_best_vertical_accuracy;
			this.location_diameter = location_diameter;
			this.location_log_diameter = location_log_diameter;
			this.location_quick_features_std_lat = location_quick_features_std_lat;
			this.location_quick_features_std_long = location_quick_features_std_long;
			this.location_quick_features_lat_change = location_quick_features_lat_change;
			this.location_quick_features_long_change = location_quick_features_long_change;
			this.location_quick_features_mean_abs_lat_deriv = location_quick_features_mean_abs_lat_deriv;
			this.location_quick_features_mean_abs_long_deriv = location_quick_features_mean_abs_long_deriv;
			this.audio_naive_mfcc0_mean = audio_naive_mfcc0_mean;
			this.audio_naive_mfcc1_mean = audio_naive_mfcc1_mean;
			this.audio_naive_mfcc2_mean = audio_naive_mfcc2_mean;
			this.audio_naive_mfcc3_mean = audio_naive_mfcc3_mean;
			this.audio_naive_mfcc4_mean = audio_naive_mfcc4_mean;
			this.audio_naive_mfcc5_mean = audio_naive_mfcc5_mean;
			this.audio_naive_mfcc6_mean = audio_naive_mfcc6_mean;
			this.audio_naive_mfcc7_mean = audio_naive_mfcc7_mean;
			this.audio_naive_mfcc8_mean = audio_naive_mfcc8_mean;
			this.audio_naive_mfcc9_mean = audio_naive_mfcc9_mean;
			this.audio_naive_mfcc10_mean = audio_naive_mfcc10_mean;
			this.audio_naive_mfcc11_mean = audio_naive_mfcc11_mean;
			this.audio_naive_mfcc12_mean = audio_naive_mfcc12_mean;
			this.audio_naive_mfcc0_std = audio_naive_mfcc0_std;
			this.audio_naive_mfcc1_std = audio_naive_mfcc1_std;
			this.audio_naive_mfcc2_std = audio_naive_mfcc2_std;
			this.audio_naive_mfcc3_std = audio_naive_mfcc3_std;
			this.audio_naive_mfcc4_std = audio_naive_mfcc4_std;
			this.audio_naive_mfcc5_std = audio_naive_mfcc5_std;
			this.audio_naive_mfcc6_std = audio_naive_mfcc6_std;
			this.audio_naive_mfcc7_std = audio_naive_mfcc7_std;
			this.audio_naive_mfcc8_std = audio_naive_mfcc8_std;
			this.audio_naive_mfcc9_std = audio_naive_mfcc9_std;
			this.audio_naive_mfcc10_std = audio_naive_mfcc10_std;
			this.audio_naive_mfcc11_std = audio_naive_mfcc11_std;
			this.audio_naive_mfcc12_std = audio_naive_mfcc12_std;
			this.audio_properties_max_abs_value = audio_properties_max_abs_value;
			this.audio_properties_normalization_multiplier = audio_properties_normalization_multiplier;
			this.discrete_app_state_is_active = discrete_app_state_is_active;
			this.discrete_app_state_is_inactive = discrete_app_state_is_inactive;
			this.discrete_app_state_is_background = discrete_app_state_is_background;
			this.discrete_app_state_missing = discrete_app_state_missing;
			this.discrete_battery_plugged_is_ac = discrete_battery_plugged_is_ac;
			this.discrete_battery_plugged_is_usb = discrete_battery_plugged_is_usb;
			this.discrete_battery_plugged_is_wireless = discrete_battery_plugged_is_wireless;
			this.discrete_battery_plugged_missing = discrete_battery_plugged_missing;
			this.discrete_battery_state_is_unknown = discrete_battery_state_is_unknown;
			this.discrete_battery_state_is_unplugged = discrete_battery_state_is_unplugged;
			this.discrete_battery_state_is_not_charging = discrete_battery_state_is_not_charging;
			this.discrete_battery_state_is_discharging = discrete_battery_state_is_discharging;
			this.discrete_battery_state_is_charging = discrete_battery_state_is_charging;
			this.discrete_battery_state_is_full = discrete_battery_state_is_full;
			this.discrete_battery_state_missing = discrete_battery_state_missing;
			this.discrete_on_the_phone_is_False = discrete_on_the_phone_is_False;
			this.discrete_on_the_phone_is_True = discrete_on_the_phone_is_True;
			this.discrete_on_the_phone_missing = discrete_on_the_phone_missing;
			this.discrete_ringer_mode_is_normal = discrete_ringer_mode_is_normal;
			this.discrete_ringer_mode_is_silent_no_vibrate = discrete_ringer_mode_is_silent_no_vibrate;
			this.discrete_ringer_mode_is_silent_with_vibrate = discrete_ringer_mode_is_silent_with_vibrate;
			this.discrete_ringer_mode_missing = discrete_ringer_mode_missing;
			this.discrete_wifi_status_is_not_reachable = discrete_wifi_status_is_not_reachable;
			this.discrete_wifi_status_is_reachable_via_wifi = discrete_wifi_status_is_reachable_via_wifi;
			this.discrete_wifi_status_is_reachable_via_wwan = discrete_wifi_status_is_reachable_via_wwan;
			this.discrete_wifi_status_missing = discrete_wifi_status_missing;
			this.lf_measurements_light = lf_measurements_light;
			this.lf_measurements_pressure = lf_measurements_pressure;
			this.lf_measurements_proximity_cm = lf_measurements_proximity_cm;
			this.lf_measurements_proximity = lf_measurements_proximity;
			this.lf_measurements_relative_humidity = lf_measurements_relative_humidity;
			this.lf_measurements_battery_level = lf_measurements_battery_level;
			this.lf_measurements_screen_brightness = lf_measurements_screen_brightness;
			this.lf_measurements_temperature_ambient = lf_measurements_temperature_ambient;
			this.discrete_time_of_day_between0and6 = discrete_time_of_day_between0and6;
			this.discrete_time_of_day_between3and9 = discrete_time_of_day_between3and9;
			this.discrete_time_of_day_between6and12 = discrete_time_of_day_between6and12;
			this.discrete_time_of_day_between9and15 = discrete_time_of_day_between9and15;
			this.discrete_time_of_day_between12and18 = discrete_time_of_day_between12and18;
			this.discrete_time_of_day_between15and21 = discrete_time_of_day_between15and21;
			this.discrete_time_of_day_between18and24 = discrete_time_of_day_between18and24;
			this.discrete_time_of_day_between21and3 = discrete_time_of_day_between21and3;
			this.label_LYING_DOWN = label_LYING_DOWN;
			this.label_SITTING = label_SITTING;
			this.label_FIX_walking = label_FIX_walking;
			this.label_FIX_running = label_FIX_running;
			this.label_BICYCLING = label_BICYCLING;
			this.label_SLEEPING = label_SLEEPING;
			this.label_LAB_WORK = label_LAB_WORK;
			this.label_IN_CLASS = label_IN_CLASS;
			this.label_IN_A_MEETING = label_IN_A_MEETING;
			this.label_LOC_main_workplace = label_LOC_main_workplace;
			this.label_OR_indoors = label_OR_indoors;
			this.label_OR_outside = label_OR_outside;
			this.label_IN_A_CAR = label_IN_A_CAR;
			this.label_ON_A_BUS = label_ON_A_BUS;
			this.label_DRIVE___I_M_THE_DRIVER = label_DRIVE___I_M_THE_DRIVER;
			this.label_DRIVE___I_M_A_PASSENGER = label_DRIVE___I_M_A_PASSENGER;
			this.label_LOC_home = label_LOC_home;
			this.label_FIX_restaurant = label_FIX_restaurant;
			this.label_PHONE_IN_POCKET = label_PHONE_IN_POCKET;
			this.label_OR_exercise = label_OR_exercise;
			this.label_COOKING = label_COOKING;
			this.label_SHOPPING = label_SHOPPING;
			this.label_STROLLING = label_STROLLING;
			this.label_DRINKING__ALCOHOL_ = label_DRINKING__ALCOHOL_;
			this.label_BATHING___SHOWER = label_BATHING___SHOWER;
			this.label_CLEANING = label_CLEANING;
			this.label_DOING_LAUNDRY = label_DOING_LAUNDRY;
			this.label_WASHING_DISHES = label_WASHING_DISHES;
			this.label_WATCHING_TV = label_WATCHING_TV;
			this.label_SURFING_THE_INTERNET = label_SURFING_THE_INTERNET;
			this.label_AT_A_PARTY = label_AT_A_PARTY;
			this.label_AT_A_BAR = label_AT_A_BAR;
			this.label_LOC_beach = label_LOC_beach;
			this.label_SINGING = label_SINGING;
			this.label_TALKING = label_TALKING;
			this.label_COMPUTER_WORK = label_COMPUTER_WORK;
			this.label_EATING = label_EATING;
			this.label_TOILET = label_TOILET;
			this.label_GROOMING = label_GROOMING;
			this.label_DRESSING = label_DRESSING;
			this.label_AT_THE_GYM = label_AT_THE_GYM;
			this.label_STAIRS___GOING_UP = label_STAIRS___GOING_UP;
			this.label_STAIRS___GOING_DOWN = label_STAIRS___GOING_DOWN;
			this.label_ELEVATOR = label_ELEVATOR;
			this.label_OR_standing = label_OR_standing;
			this.label_AT_SCHOOL = label_AT_SCHOOL;
			this.label_PHONE_IN_HAND = label_PHONE_IN_HAND;
			this.label_PHONE_IN_BAG = label_PHONE_IN_BAG;
			this.label_PHONE_ON_TABLE = label_PHONE_ON_TABLE;
			this.label_WITH_CO_WORKERS = label_WITH_CO_WORKERS;
			this.label_WITH_FRIENDS = label_WITH_FRIENDS;
			this.label_source = label_source;
		}

	}

}