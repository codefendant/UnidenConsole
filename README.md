# Uniden Scanner Get/Set Channel Info

## Description
This command-line program facilitates the reading and writing of channel information to and from your Uniden radio. It allows you to input channel details using a computer keyboard, edit information in a comma-delimited text file, and write the modified information back into your scanner. This process saves time and enables sharing of channel information files among users.

## Table of Contents
- [Introduction](#introduction)
- [Usage Examples](#usage-examples)
- [Technical Information](#technical-information)
- [Features](#features)
- [Sample Program Output](#sample-program-output)
- [Usage](#usage)
- [Important Note](#important-note)
- [Disclaimer](#disclaimer)
- [Requirements](#requirements)
- [Installation](#installation)
- [Configuration](#configuration)
- [Compatibility](#compatibility)
- [Contribution](#contribution)
- [Backup and Risk Warning](#backup-and-risk-warning)
- [Updates](#updates)
- [License](#license)

## Usage Examples
To read channel information and write to a file: `UnidenConsole.exe read`

File created: `CIN_Responses.csv`

To write channel information from a file to the scanner: `UnidenConsole.exe write`

Reading file: `CIN_Responses.csv`

## Technical Information
This Windows command-line program communicates with the Uniden scanner through a serial port, sending commands and receiving responses. It reads channel information, writes it into a comma-delimited text file, which can be edited using a text editor like Notepad or Excel. Modified files can be read by the program, and the channel information is written back into the radio.

## Features
- Gets and sets channel information on a Uniden scanner, such as a Uniden BC125AT.
- Dynamically selects an available COM port, defaulting to COM1 if available.
- Reads and writes configuration settings from/to a config file that can be modified to work with individual setups.

## Sample Program Output
```plaintext
C:\UnidenConsole.exe
Welcome to Get/Set Uniden Radio Channel Information!
Application Version: 1.0.0.0

Trying initial com port: COM1
Initial com port COM1 not found...Trying com port COM3
Serial Port Open
PRG
PRG,OK
MDL
MDL,BC125AT
VER
VER,Version 1.06.06
```

## Usage
The program initializes serial port settings based on a configuration file or defaults.
Checks for an available COM port and sets it as the default (excluding COM1 if available).
Communicates with the Uniden scanner, sending commands (e.g., "PRG," "CIN,") and receiving responses.
Optionally reads or writes channel information to a file compatible with Microsoft Excel.

## Important Note
Users are advised to familiarize themselves with the [Uniden BC75XLT Operation Specification](https://www.uniden.com.au/wp-content/uploads/2017/06/BC75XLT-Operation-Specification.pdf) before using the write option, as it may affect scanner settings.
The program uses the BC75XLT Operation Specification: < BC75XLT Operation Specification > <COMMAND CIN> Get/Set Channel Info

## Disclaimer
Use at your own risk, and backup settings before attempting write operations.

## Requirements
Windows Computer such as Windows 11

## Installation
To publish the application in the TERMINAL: `dotnet publish -c Release -r win-x64 /p:PublishSingleFile=true /p:PublishTrimmed=true`

## Configuration
A `config.txt` file has been written to the program directory. Modify it to match the COM port of your radio.

Configuration File: config.txt
The config.txt file is a plain text file located in the same directory as the program (UnidenConsole.exe). It contains key-value pairs, where each line corresponds to a specific setting.

Parameters:
PortName:

Description: Specifies the name of the COM port used for communication with the Uniden scanner.
Default Value: COM1
Example: PortName=COM3
BaudRate:

Description: Sets the baud rate for serial communication between the program and the scanner.
Default Value: 57600
Example: BaudRate=9600
Parity:

Description: Defines the parity checking scheme for serial communication.
Default Value: None
Options: None, Odd, Even
Example: Parity=None
DataBits:

Description: Specifies the number of data bits in each character of the serial communication.
Default Value: 8
Example: DataBits=7
StopBits:

Description: Determines the number of stop bits used to indicate the end of a character.
Default Value: One
Options: One, Two, None
Example: StopBits=Two
ReadTimeout:

Description: Sets the maximum time (in milliseconds) the program will wait for a response from the scanner before considering it a timeout.
Default Value: 10000 (10 seconds)
Example: ReadTimeout=5000
WriteTimeout:

Description: Sets the maximum time (in milliseconds) the program will wait for a command to be written to the scanner before considering it a timeout.
Default Value: 10000 (10 seconds)
Example: WriteTimeout=3000
RadioCommandWait:

Description: Specifies the wait time (in milliseconds) between sending commands to the scanner. Helps avoid potential communication issues.
Default Value: 100
Example: RadioCommandWait=200
Modifying the Configuration:
Open the config.txt file using a text editor such as Notepad.
Update the values after the equal sign (=) to customize the settings according to your preferences.
Save the changes to the file.
Example Configuration:
```plaintext
Copy code
PortName=COM1
BaudRate=57600
Parity=None
DataBits=8
StopBits=One
ReadTimeout=10000
WriteTimeout=10000
RadioCommandWait=100
```

## Compatibility
This program was tested on a Uniden BC125A. Ensure compatibility with your specific Uniden scanner model.

## Contribution
Share your `CIN_Responses.csv` files with other enthusiasts to contribute to the community.

## Backup and Risk Warning
It is crucial to back up settings before attempting write operations, as it could potentially brick your radio. Use at your own risk!

## Updates
Check the [GitHub repository](https://github.com/UnidenConsole/UnidenConsole) for the latest updates and bug fixes.

## License
This project is licensed under the [MIT License](https://opensource.org/licenses/MIT).
