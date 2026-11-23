
using Godot;

public partial class TestCleaner : Node
{
	public override void _Ready()
	{
		Cleaner cleaner = new Cleaner();

		string resultado = cleaner.CleanTemporaryFiles();

		GD.Print(resultado);

		WindowsNotifier.Show(
			"NexClean",
            "A limpeza de arquivos temporários foi concluída!"
		);

		GetTree().Quit();
	}
}
