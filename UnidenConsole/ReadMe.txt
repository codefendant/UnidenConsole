Uniden Scanner Get/Set Channel Info

Description:

This program reads and writes channel information to and from your radio.

The program allows you to enter channel information with a computer keyboard by editing channel information copied from your radio, which is placed into a comma delimited text file, which can then be modified using something like notepad or excel.  The modified channel information can then be written back into your scanner.  This saves you time since you can use a computer instead of your radio key pad to enter the information.  You can also share your channel information files with others, so they can save that channel information into their radios.  You can also save channel files with information specific to, for example, race radio frequencies used at an event.  In this case you could save several event files ahead of time, and then load your specific event file before each event and/or share your files with others. 

Usage Examples:

To read channel information and write to a file: UnidenConsole.exe read
	File created: CIN_Responses.csv
To write channel information from a file to the scanner: UnidenConsole.exe write
	Reading file: CIN_Responses.csv

Technical Information:

This is a Windows command line program that reads channel information from your radio and writes it into a comma delimited text file.  That file can then be modified in a text editor such as notepad or excel.  Once the changes are made to the file and saved back, using the same comma delimited file format, the file can then be read by the program and the channel information is written into your radio.

The program communicates with the scanner via a serial port, sending commands and receiving responses.

Features:

Gets and sets channel information on a Uniden scanner, such as a Uniden BC125AT.
Dynamically selects an available COM port, defaulting to COM1 if available.
Reads and writes configuration settings from/to a config file that can then be modified to work with your individual setup.

Sample Program Output:

C:\UnidenConsole.exe
Welcome to Get/Set Uniden Radio Channel Information!
Application Version: 1.0.0.0

Trying initial com port: COM1
Initial com port COM1 not found...Trying com port COM3
If COM3 doesn't work remove all serial devices, or try a different com port in config.txt like COM2, or COM3, or COM4..., if COM3 works (you'll see your Radio Model and Version Listed) ** if so, go in and update config.txt with COM3 and try again.** If not read on for additional options to try.
Serial Port Open
PRG
PRG,OK
MDL
MDL,BC125AT
VER
VER,Version 1.06.06

Let's get started, to run this application use the read and write options listed below.  To read this help information run UnidenConsole.exe

Option: 'read' Example: UnidenConsole read
  Option: 'read' - Reads Channel Info from your radio and puts it into a file CIN_Responses.csv (placed in the directory where you ran this program) that can then be modified in a text editor like notepad or excel.

Option: write - Example: UnidenConsole write
  Option: 'write', reads Channel Info from the file CIN_Responses.csv and puts it into your radio.

If you are not familiar with the 'BC75XLT Operation Specification' then read and understand the <COMMAND CIN> and make sure it works with your radio before proceeding with the write option.

You should back up your settings beforehand and understand this could brick your radio, use at your own risk!

A config.txt file has been written to this directory. You can change modem settings, like COM1 to the com port of your radio.  In Windows, run 'Device Manager' under 'ports' look for your port number and enter it into the config.txt file located in the directory where you ran this program.

This was only tested on a Uniden BC125A.

The advantage of using this program is that it allows you to enter Channel Info with a computer keyboard using something like notepad or excel, then you can import that file into your scanner, instead of using your radio keys.

If you get creative you can share your CIN_Responses.csv file(s) with other enthusiasts and help the community. ;)

To get started try the Read Option: 'read' Example: UnidenConsole read
  Then look for a file CIN_Responses.csv in the same directory you ran this from, if you find it open it up, you should see the channels from your radio.

EPG
EPG,OK
Serial Port Closed
Program Ended
Usage:

The program initializes serial port settings based on a configuration file or defaults.
Checks for an available COM port and sets it as the default (excluding COM1 if available).
Communicates with the Uniden scanner, sending commands (e.g., "PRG," "CIN,") and receiving responses.
Optionally reads or writes channel information to a file compatible with Microsoft Excel.

Important Note:

Users are advised to familiarize themselves with the Uniden BC75XLT Operation Specification before using the write option, as it may affect scanner settings.

The program uses the BC75XLT Operation Specification:
< BC75XLT Operation Specification > 

<COMMAND CIN> 
 Get/Set Channel Info
 
Controller → Radio 
① CIN,[INDEX][¥r] 
② CIN,[INDEX],[RSV],[FRQ],[RSV],[RSV],[DLY],[LOUT],[PRI][¥r]
 
Radio → Controller 
① CIN,[INDEX],[RSV],[FRQ],[RSV],[RSV],[DLY],[LOUT],[PRI][¥r] 
② CIN,OK[¥r] 

[INDEX] : Channel Index(1-300) 
[FRQ] : Channel Frequency ex) 290000 
[DLY] : Delay Time (0:OFF / 1:ON) 
[LOUT] : Lockout (0:Unlocked / 1:Lockout) 
[PRI] : Priority (0:OFF / 1:ON)
 
Get/Set Channel Information. 
In set command, only "," parameters are not changed. 
The set command is aborted if any format error is detected. 
This command is only acceptable in Programming Mode. 

Disclaimer:

Use at your own risk, and backup settings before attempting write operations.

Requirements:

Windows Computer such as Windows 11