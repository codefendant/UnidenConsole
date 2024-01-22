using System;
using System.IO;
using System.IO.Ports;
using System.Net;
using System.Threading;
using System.Reflection;

// Programmer note TO publish application in TERMINAL
// dotnet publish -c Release -r win-x64 /p:PublishSingleFile=true /p:PublishTrimmed=true


class Program
{
    private const string ProgramCommand = "PRG";
    private const string ModelCommand = "MDL";
    private const string VersionCommand = "VER";

    static void Main(string[] args)
    {
        Console.WriteLine("Welcome to Get/Set Uniden Radio Channel Information!");
        DisplayVersion();
        Console.WriteLine("");
        
        string configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.txt");
        SerialPortSettings serialPortSettings = ReadOrCreateConfigFile(configFilePath);

        // Check if default COM port is available, otherwise find an available port
        string defaultPort = serialPortSettings.PortName;
        if (!IsComPortAvailable(defaultPort))
        {
            Console.WriteLine($"Trying initial com port: {serialPortSettings.PortName}");
            string[] availablePorts = SerialPort.GetPortNames();
            foreach (string port in availablePorts)
            {
                if (port != defaultPort)
                {
                    Console.WriteLine($"Initial com port {defaultPort} not found...Trying com port {port}");
                    defaultPort = port;
                    serialPortSettings.PortName = defaultPort;
                    Console.WriteLine($"If {port} doesn't work remove all serial devices, or try a different com port in config.txt like COM2, or COM3, or COM4..., if {port} works (you'll see your Radio Model and Version Listed) ** if so, go in and update config.txt with {port} and try again.** If not read on for additional options to try.");
                    break;
                }
            }
        }

        SerialPort serialPort = new SerialPort(serialPortSettings.PortName, serialPortSettings.BaudRate)
        {
            Parity = serialPortSettings.Parity,
            DataBits = serialPortSettings.DataBits,
            StopBits = serialPortSettings.StopBits,
            ReadTimeout = serialPortSettings.ReadTimeout,
            WriteTimeout = serialPortSettings.WriteTimeout
        };

        Globals.radioCommandWait = serialPortSettings.RadioCommandWait;

        try
        {
            if (serialPort.IsOpen)
            {
                serialPort.Close();
                Console.WriteLine("Serial Port Closed");
            }

            serialPort.Open();
            Console.WriteLine("Serial Port Open");

            ExecuteCommand(serialPort, ProgramCommand);
            ExecuteCommand(serialPort, ModelCommand);
            ExecuteCommand(serialPort, VersionCommand);

            string filePath = "CIN_Responses.csv";

            if (args.Length > 0)
            {
                if (args[0].ToLower() == "read")
                {
                    ReadChannelInfoAndWriteToFile(serialPort, filePath);
                    Console.WriteLine($"CIN responses written to {filePath}.  Open this file in notepad, or excel, or a text editor of your choosing and change your channel settings.");
                }
                else if (args[0].ToLower() == "write")
                {
                    ReadFromFileAndSendToScanner(serialPort, filePath);
                    Console.WriteLine($"CIN responses read from {filePath} and sent to scanner");
                }
                else
                {
                    ShowUsageInfo();
                }
            }
            else
            {
                ShowUsageInfo();
            }

            ExecuteCommand(serialPort, "EPG");

        }
        catch (TimeoutException ex)
        {
            Console.WriteLine($"Timeout Error: {ex.Message}");
        }
        catch (Exception ex)
        {
            ShowUsageInfo();
            Console.WriteLine($"Error: {ex.Message}");
        }
        finally
        {
            CloseSerialPort(serialPort);
        }
    }

    private static void ShowUsageInfo()
    {
        Console.WriteLine("");
        Console.WriteLine("Let's get started, to run this application use the read and write options listed below.  To read this help information run UnidenConsole.exe");
        Console.WriteLine("");
        Console.WriteLine("Option: 'read' Example: UnidenConsole read ");
        Console.WriteLine("  Option: 'read' - Reads Channel Info from your radio and puts it into a file CIN_Responses.csv (placed in the directory where you ran this program) that can then be modified in a text editor like notepad or excel.");
        Console.WriteLine("");
        Console.WriteLine("Option: write - Example: UnidenConsole write");
        Console.WriteLine("  Option: 'write', reads Channel Info from the file CIN_Responses.csv and puts it into your radio.");
        Console.WriteLine("");
        Console.WriteLine("If you are not familiar with the 'BC75XLT Operation Specification' then read and understand the <COMMAND CIN> and make sure it works with your radio before proceeding with the write option.");
        Console.WriteLine("");
        Console.WriteLine("You should back up your settings beforehand and understand this could brick your radio, use at your own risk!");
        Console.WriteLine("");
        Console.WriteLine("A config.txt file has been written to this directory. You can change modem settings, like COM1 to the com port of your radio.  In Windows, run 'Device Manager' under 'ports' look for your port number and enter it into the config.txt file located in the directory where you ran this program.");
        Console.WriteLine("");
        Console.WriteLine("This was only tested on a Uniden BC125A.");
        Console.WriteLine("");
        Console.WriteLine("The advantage of using this program is that it allows you to enter Channel Info with a computer keyboard using something like notepad or excel, then you can import that file into your scanner, instead of using your radio keys.");
        Console.WriteLine("");
        Console.WriteLine("If you get creative you can share your CIN_Responses.csv file(s) with other enthusiasts and help the community. ;)");
        Console.WriteLine("");
        Console.WriteLine("To get started try the Read Option: 'read' Example: UnidenConsole read");
        Console.WriteLine("  Then look for a file CIN_Responses.csv in the same directory you ran this from, if you find it open it up, you should see the channels from your radio.");
        Console.WriteLine("");

    }

    private static void ExecuteCommand(SerialPort serialPort, string command)
    {
        serialPort.Write($"{command}\r");
        Console.WriteLine($"{command}\r");
        Thread.Sleep(Globals.radioCommandWait);
        string response = serialPort.ReadExisting();
        Console.WriteLine(response);
    }

    private static void ReadChannelInfoAndWriteToFile(SerialPort serialPort, string filePath)
    {
        try
        {
            using (StreamWriter fileWriter = new StreamWriter(filePath))
            {
                fileWriter.WriteLine("Command,ChannelIndex,Reserved1,Frequency,Reserved2,Reserved3,Delay,Lockout,Priority");

                for (int channelIndex = 1; channelIndex <= 500; channelIndex++)
                {
                    string response = ReadChannelInfo(serialPort, channelIndex);
                    fileWriter.Write(response);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    private static string ReadChannelInfo(SerialPort serialPort, int channelIndex)
    {
        try
        {
            string command = $"CIN,{channelIndex}\r";
            serialPort.Write(command);
            Console.WriteLine($"Sent Command: {command}");
            Thread.Sleep(Globals.radioCommandWait);
            string response = serialPort.ReadExisting();
            Console.WriteLine("Received Response:");
            Console.WriteLine(response);
            return $"{response}";
        }
        catch (TimeoutException ex)
        {
            Console.WriteLine($"Timeout Error: {ex.Message}");
            return string.Empty;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return string.Empty;
        }
    }

    private static void ReadFromFileAndSendToScanner(SerialPort serialPort, string filePath)
    {
        try
        {
            string[] lines = File.ReadAllLines(filePath);

            Console.WriteLine("Read in file");

            for (int i = 1; i < lines.Length; i++)
            {
                string[] values = lines[i].Split(',');

                if (values.Length == 9)
                {
                    string command = values[0];
                    int channelIndex = int.Parse(values[1]);
                    string reserved1 = values[2];
                    int frequency = int.Parse(values[3]);
                    string reserved2 = values[4];
                    int reserved3 = int.Parse(values[5]);
                    int delay = int.Parse(values[6]);
                    int lockout = int.Parse(values[7]);
                    int priority = int.Parse(values[8]);

                    string scannerCommand = $"{command},{channelIndex},{reserved1},{frequency},{reserved2},{reserved3},{delay},{lockout},{priority}\r";

                    serialPort.Write(scannerCommand);
                    Console.WriteLine($"Sent Command: {scannerCommand}");
                    Thread.Sleep(Globals.radioCommandWait);
                }
                else
                {
                    Console.WriteLine($"Invalid format in line {i + 1} of the CSV file. Skipping line. {lines[i]}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    private static void CloseSerialPort(SerialPort serialPort)
    {
        if (serialPort.IsOpen)
        {
            serialPort.Close();
            Console.WriteLine("Serial Port Closed");
        }
        Console.WriteLine("Program Ended");
    }

    public static class Globals
    {
        public static int radioCommandWait = 1000;
    }

    class SerialPortSettings
    {
        public string PortName { get; set; }
        public int BaudRate { get; set; }
        public Parity Parity { get; set; }
        public int DataBits { get; set; }
        public StopBits StopBits { get; set; }
        public int ReadTimeout { get; set; }
        public int WriteTimeout { get; set; }
        public int RadioCommandWait { get; set; }

        public SerialPortSettings()
        {
            PortName = "COM1";
            BaudRate = 57600;
            Parity = Parity.None;
            DataBits = 8;
            StopBits = StopBits.One;
            ReadTimeout = 10000;
            WriteTimeout = 10000;
            RadioCommandWait = 100;
        }

        public SerialPortSettings(string[] lines) : this()
        {
            foreach (string line in lines)
            {
                string[] parts = line.Split('=');
                if (parts.Length == 2)
                {
                    string key = parts[0].Trim().ToLower();
                    string value = parts[1].Trim();

                    switch (key)
                    {
                        case "portname":
                            PortName = value;
                            break;
                        case "baudrate":
                            int baudRate;
                            if (int.TryParse(value, out baudRate))
                                BaudRate = baudRate;
                            break;
                        case "parity":
                            Parity parity;
                            if (Enum.TryParse(value, true, out parity))
                                Parity = parity;
                            break;
                        case "databits":
                            int dataBits;
                            if (int.TryParse(value, out dataBits))
                                DataBits = dataBits;
                            break;
                        case "stopbits":
                            StopBits stopBits;
                            if (Enum.TryParse(value, true, out stopBits))
                                StopBits = stopBits;
                            break;
                        case "readtimeout":
                            int readTimeout;
                            if (int.TryParse(value, out readTimeout))
                                ReadTimeout = readTimeout;
                            break;
                        case "writetimeout":
                            int writeTimeout;
                            if (int.TryParse(value, out writeTimeout))
                                WriteTimeout = writeTimeout;
                            break;
                        case "radiocommandwait":
                            int radioCommandWait;
                            if (int.TryParse(value, out radioCommandWait))
                                RadioCommandWait = radioCommandWait;
                            break;
                    }
                }
            }
        }

        public string[] ToConfigFileFormat()
        {
            return new[]
            {
                $"PortName={PortName}",
                $"BaudRate={BaudRate}",
                $"Parity={Parity}",
                $"DataBits={DataBits}",
                $"StopBits={StopBits}",
                $"ReadTimeout={ReadTimeout}",
                $"WriteTimeout={WriteTimeout}",
                $"RadioCommandWait={RadioCommandWait}"
            };
        }
    }
    private static SerialPortSettings ReadOrCreateConfigFile(string filePath)
    {
        try
        {
            if (File.Exists(filePath))
            {
                string[] lines = File.ReadAllLines(filePath);
                return new SerialPortSettings(lines);
            }
            else
            {
                SerialPortSettings defaultSettings = new SerialPortSettings();
                File.WriteAllLines(filePath, defaultSettings.ToConfigFileFormat());
                return defaultSettings;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading or creating config file: {ex.Message}");
            return new SerialPortSettings(); // Return default settings in case of an error
        }
    }
    private static bool IsComPortAvailable(string portName)
        {
            string[] availablePorts = SerialPort.GetPortNames();
            return availablePorts.Contains(portName);
        }
    static void DisplayVersion()
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        Version version = assembly.GetName().Version;

        Console.WriteLine($"Application Version: {version}");
    }
}
