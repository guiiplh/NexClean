
extends Node

func _ready():
	var cleaner = SystemCleaner.new()

	print("===== SYSTEM CLEANER =====")

	for directory in cleaner.get_temp_directories():
		print(directory)

	print("==========================")