
extends RefCounted
class_name SystemCleaner

const TEMP_DIRECTORIES = [
	"TEMP",
	"TMP",
	"C:/Windows/Temp",
    "C:/Windows/Prefetch"
]

func get_temp_directories() -> Array[String]:
	var directories: Array[String] = []

	directories.append(OS.get_environment("TEMP"))
	directories.append(OS.get_environment("TMP"))
	directories.append("C:/Windows/Temp")
	directories.append("C:/Windows/Prefetch")

	return directories
