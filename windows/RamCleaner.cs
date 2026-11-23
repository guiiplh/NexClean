
using System;
using System.Runtime.InteropServices;

public static class RamCleaner
{
	[DllImport("psapi.dll", SetLastError = true)]
	private static extern bool EmptyWorkingSet(IntPtr hProcess);

	[DllImport("kernel32.dll")]
	private static extern IntPtr GetCurrentProcess();

	public static string ClearStandbyList()
	{
		try
		{
			bool success = EmptyWorkingSet(GetCurrentProcess());

			if (!success)
			{
				int error = Marshal.GetLastWin32Error();

				return "Não foi possível otimizar a memória. Código: " + error;
			}

			return "Memória de trabalho do NexClean otimizada com sucesso!";
		}
		catch (Exception ex)
		{
			return "Erro ao otimizar memória: " + ex.Message;
		}
	}
}
