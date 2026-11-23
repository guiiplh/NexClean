
using System;
using System.Runtime.InteropServices;

public static class CacheCleaner
{
	private const int TOKEN_ADJUST_PRIVILEGES = 0x20;
	private const int TOKEN_QUERY = 0x08;
	private const int SE_PRIVILEGE_ENABLED = 0x02;
	private const int ERROR_NOT_ALL_ASSIGNED = 1300;

	private const int SystemMemoryListInformation = 80;
	private const int MemoryPurgeStandbyList = 4;

	[StructLayout(LayoutKind.Sequential)]
	private struct LUID
	{
		public uint LowPart;
		public int HighPart;
	}

	[StructLayout(LayoutKind.Sequential)]
	private struct TOKEN_PRIVILEGES
	{
		public uint PrivilegeCount;
		public LUID Luid;
		public uint Attributes;
	}

	[DllImport("kernel32.dll")]
	private static extern IntPtr GetCurrentProcess();

	[DllImport("kernel32.dll")]
	private static extern void SetLastError(uint dwErrCode);

	[DllImport("kernel32.dll", SetLastError = true)]
	private static extern bool CloseHandle(IntPtr handle);

	[DllImport("advapi32.dll", SetLastError = true)]
	private static extern bool OpenProcessToken(
		IntPtr processHandle,
		int desiredAccess,
		out IntPtr tokenHandle
	);

	[DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
	private static extern bool LookupPrivilegeValue(
		string systemName,
		string name,
		out LUID luid
	);

	[DllImport("advapi32.dll", SetLastError = true)]
	private static extern bool AdjustTokenPrivileges(
		IntPtr tokenHandle,
		bool disableAllPrivileges,
		ref TOKEN_PRIVILEGES newState,
		int bufferLength,
		IntPtr previousState,
		IntPtr returnLength
	);

	[DllImport("ntdll.dll")]
	private static extern int NtSetSystemInformation(
		int systemInformationClass,
		ref int systemInformation,
		int systemInformationLength
	);

	public static string ClearStandbyList()
	{
		IntPtr token = IntPtr.Zero;

		try
		{
			if (!OpenProcessToken(
				GetCurrentProcess(),
				TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY,
				out token))
			{
				return "Erro ao abrir token: " +
					Marshal.GetLastWin32Error();
			}

			if (!LookupPrivilegeValue(
				null,
				"SeProfileSingleProcessPrivilege",
				out LUID luid))
			{
				return "Não foi possível localizar o privilégio necessário.";
			}

			TOKEN_PRIVILEGES privileges = new TOKEN_PRIVILEGES
			{
				PrivilegeCount = 1,
				Luid = luid,
				Attributes = SE_PRIVILEGE_ENABLED
			};

			SetLastError(0);

			if (!AdjustTokenPrivileges(
				token,
				false,
				ref privileges,
				0,
				IntPtr.Zero,
				IntPtr.Zero))
			{
				return "Falha ao habilitar privilégio. Erro: " +
					Marshal.GetLastWin32Error();
			}

			int privilegeError = Marshal.GetLastWin32Error();

			if (privilegeError == ERROR_NOT_ALL_ASSIGNED)
			{
				return "O Windows não concedeu o privilégio necessário. " +
					"É preciso verificar as permissões da conta.";
			}

			int command = MemoryPurgeStandbyList;

			int status = NtSetSystemInformation(
				SystemMemoryListInformation,
				ref command,
				sizeof(int)
			);

			if (status < 0)
			{
				return "O Windows recusou a limpeza da memória cache. " +
					"NTSTATUS: 0x" + status.ToString("X8");
			}

			return "Solicitação de limpeza da Standby List concluída.";
		}
		catch (Exception ex)
		{
			return "Erro na limpeza de cache: " + ex.Message;
		}
		finally
		{
			if (token != IntPtr.Zero)
				CloseHandle(token);
		}
	}
}
