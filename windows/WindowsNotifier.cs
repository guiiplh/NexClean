
using System;
using System.Diagnostics;

public static class WindowsNotifier
{
	public static void Show(string title, string message)
	{
		try
		{
			string safeTitle = EscapePowerShell(title);
			string safeMessage = EscapePowerShell(message);

			string script =
				"Add-Type -AssemblyName System.Windows.Forms; " +
				"$n = New-Object System.Windows.Forms.NotifyIcon; " +
				"$n.Icon = [System.Drawing.SystemIcons]::Information; " +
				"$n.Visible = $true; " +
				"$n.BalloonTipTitle = '" + safeTitle + "'; " +
				"$n.BalloonTipText = '" + safeMessage + "'; " +
				"$n.BalloonTipIcon = [System.Windows.Forms.ToolTipIcon]::Info; " +
				"$n.ShowBalloonTip(5000); " +
				"Start-Sleep -Seconds 6; " +
				"$n.Dispose();";

			ProcessStartInfo info = new ProcessStartInfo
			{
				FileName = "powershell.exe",
				Arguments = "-NoProfile -WindowStyle Hidden -Command \"" +
					script.Replace("\"", "\\\"") + "\"",
				UseShellExecute = false,
				CreateNoWindow = true
			};

			Process.Start(info);
		}
		catch (Exception)
		{
			// A falha na notificação não deve interromper a limpeza.
		}
	}

	private static string EscapePowerShell(string value)
	{
		return (value ?? "").Replace("'", "''");
	}
}
