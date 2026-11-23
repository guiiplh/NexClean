
using Godot;
using System;
using System.IO;
using System.Security;

public partial class Cleaner : RefCounted
{
	public string CleanTemporaryFiles()
	{
		int deletedFiles = 0;
		int deletedFolders = 0;
		int skipped = 0;

		string[] targets =
		{
			System.Environment.GetEnvironmentVariable("TEMP") ?? "",
			System.Environment.GetEnvironmentVariable("TMP") ?? "",
			@"C:\Windows\Temp",
            @"C:\Windows\Prefetch"
		};

		foreach (string target in targets)
		{
			if (string.IsNullOrWhiteSpace(target))
				continue;

			CleanDirectory(
				target,
				ref deletedFiles,
				ref deletedFolders,
				ref skipped
			);
		}

		return $"Limpeza concluída! Arquivos removidos: {deletedFiles}. " +
			   $"Pastas removidas: {deletedFolders}. " +
			   $"Itens preservados: {skipped}.";
	}

	private void CleanDirectory(
		string path,
		ref int deletedFiles,
		ref int deletedFolders,
		ref int skipped)
	{
		try
		{
			if (!Directory.Exists(path))
				return;

			string root = Path.GetFullPath(path);

			foreach (string entry in Directory.EnumerateFileSystemEntries(root))
			{
				try
				{
					FileAttributes attributes = File.GetAttributes(entry);

					// Nunca seguir links simbólicos ou junctions.
					if ((attributes & FileAttributes.ReparsePoint) != 0)
					{
						skipped++;
						continue;
					}

					if ((attributes & FileAttributes.Directory) != 0)
					{
						CleanDirectory(
							entry,
							ref deletedFiles,
							ref deletedFolders,
							ref skipped
						);

						try
						{
							Directory.Delete(entry, false);
							deletedFolders++;
						}
						catch (IOException)
						{
							skipped++;
						}
						catch (UnauthorizedAccessException)
						{
							skipped++;
						}
					}
					else
					{
						try
						{
							File.Delete(entry);
							deletedFiles++;
						}
						catch (IOException)
						{
							skipped++;
						}
						catch (UnauthorizedAccessException)
						{
							skipped++;
						}
					}
				}
				catch (IOException)
				{
					skipped++;
				}
				catch (UnauthorizedAccessException)
				{
					skipped++;
				}
				catch (SecurityException)
				{
					skipped++;
				}
			}
		}
		catch (IOException)
		{
			skipped++;
		}
		catch (UnauthorizedAccessException)
		{
			skipped++;
		}
		catch (SecurityException)
		{
			skipped++;
		}
	}
}
