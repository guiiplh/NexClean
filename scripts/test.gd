
extends SceneTree

func _initialize():
	var cleaner = SystemCleaner.new()

	print("===== SYSTEM CLEANER =====")
	print("Diretórios encontrados:")

	for directory in cleaner.get_temp_directories():
		print(directory)

	print("==========================")

	quit()
