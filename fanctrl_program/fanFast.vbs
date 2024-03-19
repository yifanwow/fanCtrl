Set objShell = CreateObject("WScript.Shell")
objShell.Run "C:\Windows\System32\schtasks.exe /run /tn fanFast", 0, False
