
using Godot;
using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Security.Principal;

public partial class Main : Node
{
	public override void _Ready()
	{
		string[] argumentos = OS.GetCmdlineUserArgs();

		if (argumentos.Length == 0)
		{
			GetTree().Quit();
			return;
		}

		string comando = argumentos[0].ToLowerInvariant();

		Cleaner cleaner = new Cleaner();

		switch (comando)
		{
			case "clean-temp":
			{
				string resultado = cleaner.CleanTemporaryFiles();

				GD.Print(resultado);
				WindowsNotifier.Show("NexClean", resultado);
				break;
			}

			case "clean-ram":
			{
				string resultado = RamCleaner.ClearStandbyList();

				GD.Print(resultado);
				WindowsNotifier.Show("NexClean", resultado);
				break;
			}

			case "clean-cache":
			{
				if (!IsAdministrator())
				{
					try
					{
						Process.Start(new ProcessStartInfo
						{
							FileName = OS.GetExecutablePath(),
							Arguments = "--headless -- clean-cache",
							UseShellExecute = true,
							Verb = "runas"
						});
					}
					catch (Win32Exception ex)
						when (ex.NativeErrorCode == 1223)
					{
						WindowsNotifier.Show(
							"NexClean",
                            "Operação cancelada pelo usuário."
						);
					}
					catch (Exception ex)
					{
						WindowsNotifier.Show(
							"NexClean",
							"Erro ao solicitar administrador: " +
							ex.Message
						);
					}

					GetTree().Quit();
					return;
				}

				string resultado = CacheCleaner.ClearStandbyList();

				GD.Print(resultado);
				WindowsNotifier.Show("NexClean", resultado);
				break;
			}

			default:
				GD.Print("Comando desconhecido: " + comando);
				break;
		}

		GetTree().Quit();
	}

	private static bool IsAdministrator()
	{
		if (!OperatingSystem.IsWindows())
			return false;

		using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
		{
			WindowsPrincipal principal =
				new WindowsPrincipal(identity);

			return principal.IsInRole(
				WindowsBuiltInRole.Administrator
			);
		}
	}
}
