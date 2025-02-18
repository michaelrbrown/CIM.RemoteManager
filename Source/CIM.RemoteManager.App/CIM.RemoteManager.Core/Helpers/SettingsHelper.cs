using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace CIM.RemoteManager.Core.Helpers
{
	/// <summary>
	/// This is the Settings static class that can be used in your Core solution or in any
	/// of your client applications. All settings are laid out the same exact way with getters
	/// and setters.
	/// </summary>
	public static class SettingsHelper
	{
		private static ISettings AppSettings => CrossSettings.Current;

	    #region Setting Constants

		private const string SettingsKey = "cim.remotemanager";
		private static readonly string SettingsDefault = string.Empty;

		#endregion


		public static string GeneralSettings
		{
			get => AppSettings.GetValueOrDefault(SettingsKey, SettingsDefault);
		    set => AppSettings.AddOrUpdateValue(SettingsKey, value);
		}

	}
}