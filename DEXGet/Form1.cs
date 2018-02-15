using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinSCP;

namespace DEXGet
{
    public partial class Form1 : Form
    {
        //Properties file directory variable declarations, combining the path to the whole database folder and the sub folder of the specific directory inside the database

        //Path to folder where Dex files are stored
        static DirectoryInfo DexFolder = new DirectoryInfo(Properties.Settings.Default.Lucifer_2017_Database_Path + Properties.Settings.Default.DexFolderPath);

        //Path to folder where machine information excel file is stored
        static DirectoryInfo ExcelFile = new DirectoryInfo(Properties.Settings.Default.Lucifer_2017_Database_Path + Properties.Settings.Default.ExcelFilePath);

        //Path to latest machine Dex file folder
        static DirectoryInfo MachineDatabase = new DirectoryInfo(Properties.Settings.Default.Lucifer_2017_Database_Path + Properties.Settings.Default.MachineDatabasePath);

        //Path to folder where Dex files are stored before they are converted to luci files for EasiTrax import
        static DirectoryInfo DexFileTray = new DirectoryInfo(Properties.Settings.Default.Lucifer_2017_Database_Path + Properties.Settings.Default.DexFileTray);

        //Path to folder where Dex files are downloaded to before sorting
        static DirectoryInfo TempDexFolder = new DirectoryInfo(Properties.Settings.Default.Lucifer_2017_Database_Path + Properties.Settings.Default.TempDexFolder);

        //Path to folder where machine .DAT files are stored
        static DirectoryInfo MachineArchive = new DirectoryInfo(Properties.Settings.Default.Lucifer_2017_Database_Path + Properties.Settings.Default.MachineArchive);

        //Path to folder where downloaded zip files are stored
        static DirectoryInfo ZipFolder = new DirectoryInfo(Properties.Settings.Default.Lucifer_2017_Database_Path + Properties.Settings.Default.ZipFolder);

        //Path to folder where zip files are downloaded to before unzipping
        static DirectoryInfo TempZipFolder = new DirectoryInfo(Properties.Settings.Default.Lucifer_2017_Database_Path + Properties.Settings.Default.TempZipFolder);

        //Path to where the WinSCP session logs are stored
        static DirectoryInfo SessionLogFolder = new DirectoryInfo(Properties.Settings.Default.Lucifer_2017_Database_Path + Properties.Settings.Default.WinSCP_Session_Logs);

        //Paths to folders where error logs are stored
        static DirectoryInfo MasterErrorLogFolder = new DirectoryInfo(Properties.Settings.Default.Lucifer_2017_Database_Path + Properties.Settings.Default.MasterErrorLogFolder);
        static DirectoryInfo CrashLogs = new DirectoryInfo(Properties.Settings.Default.Lucifer_2017_Database_Path + Properties.Settings.Default.Crash_Logs);

        //Properties file SFTP variable declarations

        //IP of the SFTP server where dex files are to be downloaded from
        static string SFTP_IP = Properties.Settings.Default.SFTP_IP;

        //Username and password for SFTP login
        static string SFTP_Username = Properties.Settings.Default.SFTP_Username;
        static string SFTP_Password = Properties.Settings.Default.SFTP_Password;

        //Port number of the SFTP server
        static string SFTP_PortNumber = Properties.Settings.Default.SFTP_PortNumber;

        //SSH Key for SFTP server
        static string SFTP_SSHKey = Properties.Settings.Default.SFTP_SSHKey;

        //Paths to Nayax and CPI files in the SFTP server
        static string SFTP_NayaxPath = Properties.Settings.Default.SFTP_Path_to_Nayax_folder;
        static string SFTP_CPIPath = Properties.Settings.Default.SFTP_Path_to_CPI_folder;

        //Any errors occuring during run time will be added to these lists
        List<string> masterSessionFailureLog = new List<string>();
        List<string> consoleLog = new List<string>();        

        public Form1()
        {
            //Inititaing stream reader that will capture console output
            StringBuilder consoleInput = new StringBuilder();
            StringWriter consoleData = new StringWriter(consoleInput);

            Console.SetOut(consoleData);

            //Bool variables for checking if excel file exists and is recent
            bool checkExcel = false;
            bool checkExcelDate = false;            

            //Check if machine input excel file exists and check if file was updated in the last 12 hours
            if (File.Exists(ExcelFile.ToString()))
            {
                checkExcel = true;

                DateTime ExcelDate = File.GetLastWriteTimeUtc(ExcelFile.ToString());
                TimeSpan ExcelAge = DateTime.Now - ExcelDate;

                if (ExcelAge.TotalHours < 12)
                {
                    checkExcelDate = true;
                }
            }

            //Checking for directories existence in the database, if they dont exists then make create them
            if (!Directory.Exists(DexFolder.ToString()))
            {
                Directory.CreateDirectory(DexFolder.ToString());
            }

            if (!Directory.Exists(MachineDatabase.ToString()))
            {
                Directory.CreateDirectory(MachineDatabase.ToString());
            }

            if (!Directory.Exists(DexFileTray.ToString()))
            {
                Directory.CreateDirectory(DexFileTray.ToString());
            }

            if (!Directory.Exists(TempDexFolder.ToString()))
            {
                Directory.CreateDirectory(TempDexFolder.ToString());
            }

            if (!Directory.Exists(MachineArchive.ToString()))
            {
                Directory.CreateDirectory(MachineArchive.ToString());
            }

            if (!Directory.Exists(ZipFolder.ToString()))
            {
                Directory.CreateDirectory(ZipFolder.ToString());
            }

            if (!Directory.Exists(TempZipFolder.ToString()))
            {
                Directory.CreateDirectory(TempZipFolder.ToString());
            }

            if (!Directory.Exists(MasterErrorLogFolder + DateTime.Now.ToString("MMM-yyyy") + @"\"))
            {
                Directory.CreateDirectory(MasterErrorLogFolder + DateTime.Now.ToString("MMM-yyyy") + @"\");
            }

            //If excel exists run code, if not don't continue to avoid database damage
            if (checkExcel == true)
            {
                //Console WriteLine for logging motions of the program
                Console.WriteLine("Initizializing...");

                InitializeComponent();

                Console.WriteLine("Initialized");
                Console.WriteLine("Downloading DEX Files from: " + SFTP_IP + " @" + SFTP_CPIPath);

                try
                {
                    //DownloadCPIFiles method opens a WinSCP session, downloads dex files from CPI folder on SFTP server and sorts them into the database
                    DownloadCPIFiles();
                }
                catch (Exception cpiParseError)
                {
                    //If the DownloadCPIFiles method fails, this captures the error and adds it to master session error log list for writing to error logs folder
                    masterSessionFailureLog.Add(cpiParseError.Message);
                }

                Console.WriteLine("Downloading DEX Files from: " + SFTP_IP + " @" + SFTP_NayaxPath);

                try
                {
                    //DownloadNayaxFiles method opens a WinSCP session, downloads dex files from Nayax folder on SFTP server and sorts them into the database
                    DownloadNayaxFiles();
                }
                catch(Exception nayaxParseError)
                {
                    //If the DownloadNayaxFiles method fails, this captures the error and adds it to master session error log list for writing to error logs folder
                    masterSessionFailureLog.Add(nayaxParseError.Message);
                }

                Console.WriteLine("Cleaning up...");

                try
                {
                    //CleanTempFolder method deletes temporary files that have been stored
                    CleanTempFolder();
                }
                catch(Exception cleaningError)
                {
                    //If the CleanTempFolder method fails, this captures the error and adds it to master session error log list for writing to error logs folder
                    masterSessionFailureLog.Add(cleaningError.Message);
                }
            }
            else
            {
                //If the machine information excel file doesn't exist, reports it to the error logs
                masterSessionFailureLog.Add("Machine data excel file not found, Export EasiTrax User Report 6 as CSV file to: " + ExcelFile.ToString());
                Console.WriteLine("Machine data excel file not found, Export EasiTrax User Report 6 as CSV file to: " + ExcelFile.ToString());
            }

            if (checkExcelDate == false)
            {
                //If machine infromation excel file is not up to date, reports it in the error logs
                masterSessionFailureLog.Add("Machine data excel file is out of date, Export EasiTrax User Report 6 as CSV file to: " + ExcelFile.ToString());
                Console.WriteLine("Machine data excel file is out of date, Export EasiTrax User Report 6 as CSV file to: " + ExcelFile.ToString());
            }

            //If there are any errors it will write those errors to an error log file in the error logs folder
            if (masterSessionFailureLog.Count > 0)
            {
                File.WriteAllLines(MasterErrorLogFolder + DateTime.Now.ToString("MMM-yyyy") + @"\Master_Log_" + DateTime.Now.ToString("dd-MM-yy_HH-mm-ss") + ".txt", masterSessionFailureLog);
            }

            //List to store console output for writing to command prompt log file
            List<string> consoleDataList = new List<string>();

            //Compile console output
            consoleData.Close();
            StringReader consoleOutput = new StringReader(consoleInput.ToString());
            string completeString = consoleOutput.ReadToEnd();
            consoleOutput.Close();

            //Add console output to console output list
            consoleDataList.Add(consoleInput.ToString());
            
            //Write console output to file
            File.WriteAllLines(CrashLogs + @"Command Prompt Text Logs\" + "Command_Log_" + DateTime.Now.ToString("dd-MMM-yyyy_HH-mm-ss") + ".txt", consoleDataList);

            //Close program
            Environment.Exit(00);
        }

        private void DownloadNayaxFiles()
        {
            string hostIP = (SFTP_IP);
            string userName = (SFTP_Username);
            string passWord = (SFTP_Password);
            string remotePath = (SFTP_NayaxPath);

            //List of downloaded Nayax file's names
            List<string> nayaxNames = new List<string>();

            //List for storing captured errors for writing to error logs
            List<string> Error = new List<string>();

            try
            {
                //WinSCP session options declaration
                SessionOptions sessionOptions = new SessionOptions
                {
                    Protocol = Protocol.Sftp,
                    HostName = hostIP,
                    UserName = userName,
                    Password = passWord,
                    PortNumber = Int32.Parse(SFTP_PortNumber),
                    SshHostKeyFingerprint = SFTP_SSHKey
                };

                //Start new WinSCP session
                using (Session session = new Session())
                {
                    //Write session logs to session log file
                    session.SessionLogPath = SessionLogFolder + @"Nayax\Session_Log_File_" + DateTime.Now.ToString("dd-MM-yy_HH-mm-ss") + ".txt";

                    session.Open(sessionOptions);
                    TransferOptions transferOptions = new TransferOptions();
                    transferOptions.TransferMode = TransferMode.Ascii;

                    //Download Nayax file to temporary folder and remove files after download
                    session.GetFiles(remotePath, TempDexFolder.ToString(), true).Check();

                    //Nayax file directory on SFTP server gets deleted by WinSCP so directory must be re-created
                    session.CreateDirectory(SFTP_NayaxPath);
                }
            }
            catch(Exception sessionError)
            {
                //Catch any errors from session and add to error log list
                Error.Add(sessionError.ToString());
            }

            //Add downloaded file's names to list
            foreach (FileInfo file in TempDexFolder.GetFiles("*.txt"))
            {
                nayaxNames.Add(file.Name);
            }

            //Get count of downloaded files and write it in console
            int fileCount = nayaxNames.Count;
            if (fileCount > 0)
            {
                Console.WriteLine(fileCount + " Files Found, Processing...");
            }
            else
            {
                Console.WriteLine("No files to process");
            }

            //Add all lines of machine information excel file to a list
            //Each line represents a vending machine and contains data relating to that machine
            string[] ExcelData = File.ReadAllLines(ExcelFile.ToString());

            string databaseData = "";

            //Simple count to log processed files
            int processCount = 0;

            //Get count of entries in excel data list
            int excelLineCount = ExcelData.Length;

            //Foreach loop for every entry in the excel data list 
            //each loop gets data from the entry(line in the excel file) and then loops through each of the dex files looking for matching PHYSID's from the dex file
            //in cases where it does match, it adds the data to the relevant machine in the database
            if (fileCount > 0)
            {
                foreach (string line in ExcelData)
                {
                    //Count of lines processed giving indication of progress in console
                    processCount++;
                    Console.WriteLine(processCount + " out of " + excelLineCount + " machines processed");

                    //DateTime of dex file variable declaration set to be current time and date as default
                    DateTime DexDate = DateTime.Now;

                    //Split the excel line into individual variables and add to an array
                    string[] lineData = line.Split(',');

                    //Machine information variable declaration
                    string machinenumber = "";
                    string machinelocation = "";
                    string telemetrydevice = "";
                    string telemetrynumber = "";
                    string drivername = "";
                    string routenumber = "";
                    string machinecapacity = "";
                    string machinemodel = "";
                    string machinetype = "";
                    string machinesector = "";

                    //Set variables to line data
                    telemetrynumber = lineData[14].Trim('"');
                    machinenumber = lineData[0].Trim('"');
                    machinelocation = lineData[2].Trim('"');
                    string RouteNumberFull = lineData[17].Trim('"');
                    string[] DriverName = lineData[18].Trim('"').Split('(');
                    telemetrydevice = lineData[8].Trim('"');
                    string[] RouteNumberData = RouteNumberFull.Split(' ');
                    drivername = DriverName[0].Trim('"');
                    machinecapacity = lineData[10].Trim('"');
                    machinemodel = lineData[19].Trim('"');
                    machinetype = lineData[20].Trim('"');
                    machinesector = lineData[21].Trim('"');

                    //Dex file meter variable declarations
                    string Meters = "";
                    string CoinMechSerials = "";
                    string CoinMechMeters = "";
                    string TubeMeters = "";
                    string DispenseMeters = "";
                    string DiscountMeters = "";
                    string OverPayMeters = "";
                    string CashFillMeters = "";
                    string TubeContentsValue = "";
                    string CashlessSales = "";

                    try
                    {
                        //Foreach loop for every downloaded Nayax dex file
                        foreach (string file in nayaxNames)
                        {
                            //Dex file names contain the date/time information of the dex file, PHYSID of the telemetry unit used to match the data with the machine it came from and whether it was parsed automatically or from a refill 
                            //Split the name and add each variable to an array
                            string[] fileInfo = file.Split('_', '.');

                            //Variable for path to dex file
                            string downloadedFilesPath = TempDexFolder + file;

                            //Refill variable to check if dex file was parsed automatically or if it was caused by a driver refilling the machine
                            string Refill = "";
                            string originTag = "A1";

                            //Sometimes file names don't contain the automatic or refill information in the name and this will end up throwing an error when checking which one it is
                            //So first it checks the count of the file name information and if it falls below the expected it will default the type to automatic
                            if (fileInfo.Length < 5)
                            {
                                Refill = "AUTOMATIC";
                            }
                            else
                            {
                                //Sometimes the position of the type changes so here it sets the variable to the lower position and checks if this variable matches the type if not it sets the variable to the next position
                                Refill = fileInfo[3];
                                if (fileInfo[3] != "REFILL" && fileInfo[3] != "AUTOMATIC")
                                {
                                    Refill = fileInfo[4];
                                }
                            }

                            //Checks if file is a refill file and if it is it will set the origin tag to F1, the origin tag is used to identify the origin of the record
                            if (Refill == "REFILL")
                            {
                                originTag = "F1";
                            }

                            //Dex PHYSID variable for matching dex file to machine in machine information excel file
                            string dexPHYSID = fileInfo[1];

                            try
                            {
                                //Route number data formatting as it contains irrelevant extra information
                                routenumber = RouteNumberData[1] + " " + RouteNumberData[2];
                            }
                            catch
                            {

                            }

                            //Checks if Dex files matches the machine in the current excel line loop
                            if (dexPHYSID == telemetrynumber)
                            {
                                //Read all lines of the Dex file and add to an array
                                string[] FileData = File.ReadAllLines(downloadedFilesPath);

                                //Set previously declared DexDate DateTime variable to parsed DateTime from Dex file name information array
                                DexDate = DateTime.ParseExact(fileInfo[2] + fileInfo[3], "yyyyMMddHHmmss", CultureInfo.InvariantCulture);

                                //Dex file meter variables list
                                List<string> ProductMeters = new List<string>();

                                //Dex file data list
                                List<string> MachineData = new List<string>();

                                //Using Array.Find to find the meter readings in the dex file based on the reading tag
                                Meters = Array.Find(FileData,
                                        element => element.StartsWith("VA1", StringComparison.Ordinal));

                                CoinMechSerials = Array.Find(FileData,
                                    element => element.StartsWith("CA1", StringComparison.Ordinal));

                                CoinMechMeters = Array.Find(FileData,
                                    element => element.StartsWith("CA2", StringComparison.Ordinal));

                                TubeMeters = Array.Find(FileData,
                                    element => element.StartsWith("CA3", StringComparison.Ordinal));

                                DispenseMeters = Array.Find(FileData,
                                    element => element.StartsWith("CA4", StringComparison.Ordinal));

                                DiscountMeters = Array.Find(FileData,
                                    element => element.StartsWith("CA7", StringComparison.Ordinal));

                                OverPayMeters = Array.Find(FileData,
                                    element => element.StartsWith("CA8", StringComparison.Ordinal));

                                CashFillMeters = Array.Find(FileData,
                                    element => element.StartsWith("CA10", StringComparison.Ordinal));

                                TubeContentsValue = Array.Find(FileData,
                                    element => element.StartsWith("CA15", StringComparison.Ordinal));

                                CashlessSales = Array.Find(FileData,
                                    element => element.StartsWith("DA2", StringComparison.Ordinal));

                                //This variable is declared before the foreach loop of the lines in the dex file because of the in structure of the dex file the meters are stored on seperate lines to there product tags
                                //The PA1 line contains the product tag so this variable catches that tag for when the loop continues to the next line containing the meter reading it can add that tag to the captured meter variable
                                string PA1 = "";

                                //Foreach loop through each line in the dex file
                                foreach (string Line in FileData)
                                {
                                    //Variable for matching product tag with associated meter
                                    string NewProduct = "";

                                    if (Line.StartsWith("PA1"))
                                    {
                                        //If the line starts with PA1 that indicates the line is a product tag
                                        PA1 = Line;
                                    }
                                    else if (Line.StartsWith("PA2"))
                                    {
                                        //If the line starts with PA2 that indicates its the product meter of the previously captured product tag
                                        //Add the tag and meter reading together in a variable delimited with a hyphen
                                        NewProduct = PA1 + "-" + Line;
                                        //Add product tag/meter to the products data list
                                        ProductMeters.Add(NewProduct);
                                    }
                                }

                                //Meter variable declaration
                                string TotalVends = "0";
                                string TotalCash = "0";
                                string ResetVends = "0";
                                string ResetCash = "0";

                                //If the dex file meter entry existed
                                if (Meters != null)
                                {
                                    //Split the line by its delimiter
                                    string[] MeterLine = Meters.Split('*');

                                    //Set variable's data from readings
                                    TotalVends = MeterLine[1];
                                    TotalCash = MeterLine[2];
                                    ResetVends = MeterLine[3];
                                    ResetCash = MeterLine[4];
                                }

                                //Coin mech variables
                                string CoinMechSerialNumber = "0";
                                string CoinMechModel = "0";
                                string CoinMechSoftwareVersion = "0";

                                if (CoinMechSerials != null)
                                {
                                    string[] CoinMechSerialLine = CoinMechSerials.Split('*');
                                    CoinMechSerialNumber = CoinMechSerialLine[1];
                                    CoinMechModel = CoinMechSerialLine[2];
                                    CoinMechSoftwareVersion = CoinMechSerialLine[3];
                                }

                                //Coin mech meter variables
                                string CoinMechTotalCash = "0";
                                string CoinMechTotalVends = "0";
                                string CoinMechResetCash = "0";
                                string CoinMechResetVends = "0";

                                if (CoinMechMeters != null)
                                {
                                    string[] CoinMechMeterLine = CoinMechMeters.Split('*');
                                    CoinMechTotalCash = CoinMechMeterLine[1];
                                    CoinMechTotalVends = CoinMechMeterLine[2];
                                    CoinMechResetCash = CoinMechMeterLine[3];
                                    CoinMechResetVends = CoinMechMeterLine[4];
                                }

                                //Cash in variables
                                string CashIn = "0";
                                string ToCashBoxReset = "0";
                                string CashToTubesReset = "0";
                                string ToCashBoxInit = "0";
                                string CashToTubesInit = "0";

                                if (TubeMeters != null)
                                {
                                    string[] TubeMeterLine = TubeMeters.Split('*');
                                    CashIn = TubeMeterLine[1];
                                    ToCashBoxReset = TubeMeterLine[2];
                                    CashToTubesReset = TubeMeterLine[3];
                                    ToCashBoxInit = TubeMeterLine[4];
                                    CashToTubesInit = TubeMeterLine[5];
                                }

                                //Cash out vairbales
                                string CashDispensedReset = "0";
                                string CashManualDispenseReset = "0";
                                string CashDispensedInit = "0";
                                string CashManualDispenseInit = "0";

                                if (DispenseMeters != null)
                                {
                                    string[] DispenseMeterLine = DispenseMeters.Split('*');
                                    CashDispensedReset = DispenseMeterLine[1];
                                    CashManualDispenseReset = DispenseMeterLine[2];
                                    CashDispensedInit = DispenseMeterLine[3];
                                    CashManualDispenseInit = DispenseMeterLine[4];
                                }

                                //Discount variables
                                string DiscountsValueReset = "0";
                                string DiscountsValueInit = "0";

                                if (DiscountMeters != null)
                                {
                                    string[] DiscountMeterLine = DiscountMeters.Split('*');
                                    DiscountsValueReset = DiscountMeterLine[1];
                                    DiscountsValueInit = DiscountMeterLine[2];
                                }

                                //Overpay variables
                                string OverPayValueReset = "0";
                                string OverPayValueInit = "0";

                                if (OverPayMeters != null)
                                {
                                    string[] OverPayMeterLine = OverPayMeters.Split('*');
                                    OverPayValueReset = OverPayMeterLine[1];
                                    OverPayValueInit = OverPayMeterLine[2];
                                }

                                //Cash in from driver vairables
                                string CashFillValueReset = "0";
                                string CashFillValueInit = "0";

                                if (CashFillMeters != null)
                                {
                                    string[] CashFillMeterLine = CashFillMeters.Split('*');
                                    CashFillValueReset = CashFillMeterLine[1];
                                    CashFillValueInit = CashFillMeterLine[2];
                                }

                                //Value of coins in coin mech variable
                                string TubeValue = "0";

                                if (TubeContentsValue != null)
                                {
                                    string[] TubeContentsLine = TubeContentsValue.Split('*');
                                    TubeValue = TubeContentsLine[1];
                                }

                                //Cashless meter variables
                                string CashlessCashMeter = "";
                                string CashlessVendMeter = "";

                                if (CashlessSales != null)
                                {
                                    string[] CashlessLine = CashlessSales.Split('*');
                                    CashlessCashMeter = CashlessLine[1];
                                    CashlessVendMeter = CashlessLine[2];
                                }

                                //Combine all product variables to string deleimited with a comma
                                string ProductList = string.Join(",", ProductMeters.ToArray());

                                //Combine all variables into one string to become a single line entry
                                databaseData = originTag + "-" + DexDate.ToString("ddMMyyyy,HHmmss")
                                    + "*A2-" + machinenumber + "*A3-" + machinelocation + "*A4-" + telemetrydevice + "*A5-"
                                    + telemetrynumber + "*A6-" + drivername + "*A7-" + routenumber + "*A8-" + machinecapacity + "*A9-"
                                    + machinemodel + "*A10-" + machinetype + "*A11-" + machinesector + "*A12-" + TotalVends
                                    + "*A13-" + TotalCash + "*A14-" + ResetVends + "*A15-" + ResetCash + "*A16-" + CoinMechSerialNumber
                                    + "*A17-" + CoinMechModel + "*A18-" + CoinMechSoftwareVersion + "*A19-" + CoinMechTotalCash
                                    + "*A20-" + CoinMechTotalVends + "*A21-" + CoinMechResetCash + "*A22-" + CoinMechResetVends
                                    + "*A23-" + CashIn + "*A24-" + ToCashBoxReset + "*A25-" + CashToTubesReset + "*A26-" + ToCashBoxInit
                                    + "*A27-" + CashToTubesInit + "*A28-" + CashDispensedReset + "*A29-" + CashManualDispenseReset
                                    + "*A30-" + CashDispensedInit + "*A31-" + CashManualDispenseInit + "*A32-" + DiscountsValueReset
                                    + "*A33-" + DiscountsValueInit + "*A34-" + OverPayValueReset + "*A35-" + OverPayValueInit
                                    + "*A36-" + CashFillValueReset + "*A37-" + CashFillValueInit + "*A38-" + TubeValue
                                    + "*A39-" + CashlessCashMeter + "*A40-" + CashlessVendMeter
                                    + "*A41_" + ProductList;

                                //Check if current months machine file exists if not creates it and adds the data to the file
                                if (File.Exists(MachineArchive + machinenumber + "-" + DexDate.ToString("MMM-yy") + ".dat"))
                                {
                                    string[] machineData = File.ReadAllLines(MachineArchive + machinenumber + "-" + DexDate.ToString("MMM-yy") + ".dat");

                                    foreach (string machineLine in machineData)
                                    {
                                        MachineData.Add(machineLine);
                                    }

                                    if (databaseData != "")
                                    {
                                        MachineData.Add(databaseData);
                                    }

                                    File.WriteAllLines(MachineArchive + machinenumber + "-" + DexDate.ToString("MMM-yy") + ".dat", MachineData);
                                }
                                else
                                {
                                    if (databaseData != "")
                                    {
                                        MachineData.Add(databaseData);
                                    }

                                    File.WriteAllLines(MachineArchive + machinenumber + "-" + DexDate.ToString("MMM-yy") + ".dat", MachineData);
                                }
                            }
                        }
                    }
                    catch (Exception fileError)
                    {
                        //Catch any errors and add them to the error list
                        Error.Add(fileError.ToString());
                    }
                }

                try
                {
                    //Sort temporary files into archives
                    foreach (string fileName in nayaxNames)
                    {
                        string[] NameData = fileName.Split('_');
                        DateTime DexTime = DateTime.ParseExact(NameData[2] + NameData[3], "yyyyMMddHHmmss", CultureInfo.InstalledUICulture);

                        //Copy to machine database to update latest machine records
                        File.Copy(TempDexFolder + fileName, MachineDatabase + NameData[1] + ".dex", true);
                        //Set creation time to dex date
                        File.SetCreationTime(MachineDatabase + NameData[1] + ".dex", DexTime);
                        //Copy to Dex file tray to be converted to luci file later
                        File.Copy(TempDexFolder + fileName, DexFileTray + NameData[1] + "_" + DexTime.ToString("yyyyMMdd_HHmmss") + "_" + NameData[3] + ".dex", true);
                        try
                        {
                            //Move the file to the dex file archive
                            File.Move(TempDexFolder + fileName, DexFolder + NameData[1] + "_" + NameData[2] + "_" + NameData[3] + "_" + NameData[5] + ".dex");
                        }
                        catch
                        {
                            Error.Add("File could not be moved: " + TempDexFolder + fileName);
                        }
                    }
                }
                catch (Exception fileCleanError)
                {
                    //Catch any errors and add them to the error list
                    Error.Add(fileCleanError.ToString());
                }

                //If there are any errors write them to an error log in the error logs folder
                if (Error.Count > 0)
                {
                    File.WriteAllLines(CrashLogs + @"Nayax Crash Logs\" + "Nayax_Log_" + DateTime.Now.ToString("dd-MMM-yyyy_HH-mm-ss") + ".txt", Error);
                }
            }

            Console.WriteLine("Process complete with " + Error.Count.ToString() + " errors");
        }

        private void DownloadCPIFiles()
        {
            string hostIP = (SFTP_IP);
            string userName = (SFTP_Username);
            string passWord = (SFTP_Password);
            string remotePath = (SFTP_CPIPath);

            //List for CPI files initialization
            List<string> CPINames = new List<string>();

            //Error log list declaration
            List<string> Error = new List<string>();

            try
            {
                //Session details for WinSCP
                SessionOptions sessionOptions = new SessionOptions
                {
                    Protocol = Protocol.Sftp,
                    HostName = hostIP,
                    UserName = userName,
                    Password = passWord,
                    PortNumber = Int32.Parse(SFTP_PortNumber),
                    SshHostKeyFingerprint = SFTP_SSHKey
                };

                //Start new WinSCP session
                using (Session session = new Session())
                {
                    session.SessionLogPath = SessionLogFolder + @"CPI\Session_Log_File_" + DateTime.Now.ToString("dd-MM-yy_HH-mm-ss") + ".txt";
                    session.Open(sessionOptions);
                    TransferOptions transferOptions = new TransferOptions();

                    //Transfer option to binary for ZIP files
                    transferOptions.TransferMode = TransferMode.Binary;

                    //Download files to CPI temporary folder and delete file after download(*true*)
                    session.GetFiles(remotePath, TempZipFolder.ToString(), true).Check();
                    //Re-create directory as WinSCP deletes it
                    session.CreateDirectory(SFTP_CPIPath);
                }
            }
            catch(Exception sessionErrors)
            {
                //Add any errors to error log list
                Error.Add(sessionErrors.ToString());
            }

            foreach (FileInfo file in TempZipFolder.GetFiles("*.zip"))
            {
                //Add donwload file's names to list
                CPINames.Add(file.Name);
            }

            try
            { 
                //Simple file counting variable for console
                int fileCount = CPINames.Count;

                //Display to console the file count
                if (fileCount > 0)
                {
                    Console.WriteLine(fileCount + " Files Found, Processing...");
                }
                else
                {
                    Console.WriteLine("No files to process");
                }

                //Variable for storing data later
                string databaseData = "";

                //Date of dex file set to current as default
                DateTime NameDate = DateTime.Now;

                //Foreach loop to extract files from downloaded ZIP folders
                if (fileCount > 0)
                {
                    foreach (string file in CPINames)
                    {
                        //File path variable
                        string zipFilePath = TempZipFolder.ToString() + file;

                        //Read ZIP file
                        ZipFile zip1 = ZipFile.Read(zipFilePath);

                        //selection = file starting with "0", which is the dex file
                        var selection = (from a in zip1.Entries
                                         where (a.FileName).StartsWith("0")
                                         select a);

                        //selection2 = file starting with feed, which is the indicator of the dex type
                        var selection2 = (from b in zip1.Entries
                                          where (b.FileName).StartsWith("feed")
                                          select b);

                        //Split file name and add items to array
                        string[] names = file.Split('_', '.');

                        //Dex PHYSID is represented by a hexidecimal value and must be converted to decimal to get the actual PHYSID
                        int Name = Int32.Parse(names[0], System.Globalization.NumberStyles.HexNumber);
                        string nameDate = names[1] + names[2];

                        //Parse Dex date from name variables
                        NameDate = DateTime.ParseExact(nameDate, "MMddyyyyHHmmss", CultureInfo.InvariantCulture);

                        //Bool for checking if file is Refill or not
                        bool check = false;

                        //If there is a feed file in the zip file set check as true
                        foreach (var b in selection2)
                        {
                            check = true;
                        }

                        //If check is not true then file is automatic
                        if (check != true)
                        {
                            foreach (var a in selection)
                            {
                                string[] fileName = a.FileName.Split('.');
                                //Extract file to temp folder
                                a.Extract(TempZipFolder.ToString(), ExtractExistingFileAction.OverwriteSilently);
                                try
                                {
                                    //Move file to same folder with formatted date/time and automatic added to the name 
                                    File.Move(TempZipFolder.ToString() + a.FileName, TempDexFolder.ToString() + Int32.Parse(fileName[0], NumberStyles.HexNumber).ToString() + "_" + NameDate.ToString("ddMMyy_HHmmss") + "_AUTOMATIC.dex");
                                }
                                catch
                                {
                                    Error.Add("File could not be moved: " + TempZipFolder + a.FileName);
                                }
                            }
                        }
                        else
                        {
                            foreach (var a in selection)
                            {
                                string[] fileName = a.FileName.Split('.');
                                //Extract file to temp folder
                                a.Extract(TempZipFolder.ToString());
                                //Move file to same folder with formatted date/time and refill added to the name
                                try
                                {
                                    File.Move(TempZipFolder.ToString() + a.FileName, TempDexFolder.ToString() + Int32.Parse(fileName[0], NumberStyles.HexNumber).ToString() + "_" + NameDate.ToString("ddMMyy_HHmmss") + "_REFILL.dex");
                                }
                                catch
                                {
                                    Error.Add("File could not be moved: " + TempZipFolder + a.FileName);
                                }
                            }
                        }                        
                    }

                    Console.WriteLine("CPI files downloaded. processing...");

                    //List of extracted file names
                    List<string> unzippedFileNames = new List<string>();


                    foreach (FileInfo unzippedFileInfo in TempDexFolder.GetFiles("*.dex"))
                    {
                        //Get files from temp folder and add to a list
                        unzippedFileNames.Add(unzippedFileInfo.Name);
                    }

                    //Read all lines of the input excel file
                    string[] ExcelData = File.ReadAllLines(ExcelFile.ToString());

                    int processCount = 0;
                    int excelLineCount = ExcelData.Length;

                    foreach (string line in ExcelData)
                    {
                        processCount++;
                        Console.WriteLine(processCount + " out of " + excelLineCount + " machines processed");

                        DateTime DexDate = DateTime.Now;
                        string[] lineData = line.Split(',');

                        string machinenumber = "";
                        string machinelocation = "";
                        string telemetrydevice = "";
                        string telemetrynumber = "";
                        string drivername = "";
                        string routenumber = "";
                        string machinecapacity = "";
                        string machinemodel = "";
                        string machinetype = "";
                        string machinesector = "";

                        telemetrynumber = lineData[14].Trim('"');
                        machinenumber = lineData[0].Trim('"');
                        machinelocation = lineData[2].Trim('"');
                        string RouteNumberFull = lineData[17].Trim('"');
                        string[] DriverName = lineData[18].Trim('"').Split('(');
                        telemetrydevice = lineData[8].Trim('"');
                        string[] RouteNumberData = RouteNumberFull.Split(' ');
                        drivername = DriverName[0].Trim('"');
                        machinecapacity = lineData[10].Trim('"');
                        machinemodel = lineData[19].Trim('"');
                        machinetype = lineData[20].Trim('"');
                        machinesector = lineData[21].Trim('"');

                        string Meters = "";
                        string CoinMechSerials = "";
                        string CoinMechMeters = "";
                        string TubeMeters = "";
                        string DispenseMeters = "";
                        string DiscountMeters = "";
                        string OverPayMeters = "";
                        string CashFillMeters = "";
                        string TubeContentsValue = "";
                        string CashlessSales = "";

                        foreach (string unzippedFile in unzippedFileNames)
                        {
                            string[] fileInfo = unzippedFile.Split('_', '.');
                            string downloadedFilesPath = TempDexFolder + unzippedFile;

                            string Refill = "";
                            string originTag = "A1";

                            if (fileInfo.Length < 5)
                            {
                                Refill = "AUTOMATIC";
                            }
                            else
                            {
                                Refill = fileInfo[3];
                                if (fileInfo[3] != "REFILL" && fileInfo[3] != "AUTOMATIC")
                                {
                                    Refill = fileInfo[4];
                                }
                            }

                            if (Refill == "REFILL")
                            {
                                originTag = "F1";
                            }

                                string dexPHYSID = fileInfo[0];

                                try
                                {
                                    routenumber = RouteNumberData[1] + " " + RouteNumberData[2];
                                }
                                catch
                                {

                                }

                            if (dexPHYSID == telemetrynumber)
                            {
                                string[] FileData = File.ReadAllLines(downloadedFilesPath);
                                DexDate = DateTime.ParseExact(fileInfo[1] + fileInfo[2], "ddMMyyHHmmss", CultureInfo.InvariantCulture);

                                List<string> ProductMeters = new List<string>();
                                List<string> MachineData = new List<string>();

                                string PA1 = "";

                                foreach (string Line in FileData)
                                {

                                    string NewProduct = "";

                                    Meters = Array.Find(FileData,
                                        element => element.StartsWith("VA1", StringComparison.Ordinal));

                                    CoinMechSerials = Array.Find(FileData,
                                        element => element.StartsWith("CA1", StringComparison.Ordinal));

                                    CoinMechMeters = Array.Find(FileData,
                                        element => element.StartsWith("CA2", StringComparison.Ordinal));

                                    TubeMeters = Array.Find(FileData,
                                        element => element.StartsWith("CA3", StringComparison.Ordinal));

                                    DispenseMeters = Array.Find(FileData,
                                        element => element.StartsWith("CA4", StringComparison.Ordinal));

                                    DiscountMeters = Array.Find(FileData,
                                        element => element.StartsWith("CA7", StringComparison.Ordinal));

                                    OverPayMeters = Array.Find(FileData,
                                        element => element.StartsWith("CA8", StringComparison.Ordinal));

                                    CashFillMeters = Array.Find(FileData,
                                        element => element.StartsWith("CA10", StringComparison.Ordinal));

                                    TubeContentsValue = Array.Find(FileData,
                                        element => element.StartsWith("CA15", StringComparison.Ordinal));

                                    CashlessSales = Array.Find(FileData,
                                        element => element.StartsWith("DA2", StringComparison.Ordinal));

                                    if (Line.StartsWith("PA1"))
                                    {
                                        PA1 = Line;
                                    }
                                    else if (Line.StartsWith("PA2"))
                                    {
                                        NewProduct = PA1 + "-" + Line;
                                        ProductMeters.Add(NewProduct);
                                    }


                                }

                                string TotalVends = "0";
                                string TotalCash = "0";
                                string ResetVends = "0";
                                string ResetCash = "0";

                                if (Meters != null)
                                {
                                    string[] MeterLine = Meters.Split('*');
                                    TotalVends = MeterLine[1];
                                    TotalCash = MeterLine[2];
                                    ResetVends = MeterLine[3];
                                    ResetCash = MeterLine[4];
                                }

                                string CoinMechSerialNumber = "0";
                                string CoinMechModel = "0";
                                string CoinMechSoftwareVersion = "0";

                                if (CoinMechSerials != null)
                                {
                                    string[] CoinMechSerialLine = CoinMechSerials.Split('*');
                                    CoinMechSerialNumber = CoinMechSerialLine[1];
                                    CoinMechModel = CoinMechSerialLine[2];
                                    CoinMechSoftwareVersion = CoinMechSerialLine[3];
                                }

                                string CoinMechTotalCash = "0";
                                string CoinMechTotalVends = "0";
                                string CoinMechResetCash = "0";
                                string CoinMechResetVends = "0";

                                if (CoinMechMeters != null)
                                {
                                    string[] CoinMechMeterLine = CoinMechMeters.Split('*');
                                    CoinMechTotalCash = CoinMechMeterLine[1];
                                    CoinMechTotalVends = CoinMechMeterLine[2];
                                    CoinMechResetCash = CoinMechMeterLine[3];
                                    CoinMechResetVends = CoinMechMeterLine[4];
                                }

                                string CashIn = "0";
                                string ToCashBoxReset = "0";
                                string CashToTubesReset = "0";
                                string ToCashBoxInit = "0";
                                string CashToTubesInit = "0";

                                if (TubeMeters != null)
                                {
                                    string[] TubeMeterLine = TubeMeters.Split('*');
                                    CashIn = TubeMeterLine[5];
                                    ToCashBoxReset = TubeMeterLine[2];
                                    CashToTubesReset = TubeMeterLine[3];
                                    ToCashBoxInit = TubeMeterLine[6];
                                    CashToTubesInit = TubeMeterLine[7];
                                }

                                string CashDispensedReset = "0";
                                string CashManualDispenseReset = "0";
                                string CashDispensedInit = "0";
                                string CashManualDispenseInit = "0";

                                if (DispenseMeters != null)
                                {
                                    string[] DispenseMeterLine = DispenseMeters.Split('*');
                                    CashDispensedReset = DispenseMeterLine[1];
                                    CashManualDispenseReset = DispenseMeterLine[2];
                                    CashDispensedInit = DispenseMeterLine[3];
                                    CashManualDispenseInit = DispenseMeterLine[4];
                                }

                                string DiscountsValueReset = "0";
                                string DiscountsValueInit = "0";

                                if (DiscountMeters != null)
                                {
                                    string[] DiscountMeterLine = DiscountMeters.Split('*');
                                    DiscountsValueReset = DiscountMeterLine[1];
                                    DiscountsValueInit = DiscountMeterLine[2];
                                }

                                string OverPayValueReset = "0";
                                string OverPayValueInit = "0";

                                if (OverPayMeters != null)
                                {
                                    string[] OverPayMeterLine = OverPayMeters.Split('*');
                                    OverPayValueReset = OverPayMeterLine[1];
                                    OverPayValueInit = OverPayMeterLine[2];
                                }

                                string CashFillValueReset = "0";
                                string CashFillValueInit = "0";

                                if (CashFillMeters != null)
                                {
                                    string[] CashFillMeterLine = CashFillMeters.Split('*');
                                    CashFillValueReset = CashFillMeterLine[1];
                                    CashFillValueInit = CashFillMeterLine[2];
                                }

                                string TubeValue = "0";

                                if (TubeContentsValue != null)
                                {
                                    string[] TubeContentsLine = TubeContentsValue.Split('*');
                                    TubeValue = TubeContentsLine[1];
                                }

                                string CashlessCashMeter = "";
                                string CashlessVendMeter = "";

                                if (CashlessSales != null)
                                {
                                    string[] CashlessLine = CashlessSales.Split('*');
                                    CashlessCashMeter = CashlessLine[1];
                                    CashlessVendMeter = CashlessLine[2];
                                }

                                string ProductList = string.Join(",", ProductMeters.ToArray());

                                databaseData = originTag + "-" + DexDate.ToString("ddMMyyyy,HHmmss")
                                    + "*A2-" + machinenumber + "*A3-" + machinelocation + "*A4-" + telemetrydevice + "*A5-"
                                    + telemetrynumber + "*A6-" + drivername + "*A7-" + routenumber + "*A8-" + machinecapacity + "*A9-"
                                    + machinemodel + "*A10-" + machinetype + "*A11-" + machinesector + "*A12-" + TotalVends
                                    + "*A13-" + TotalCash + "*A14-" + ResetVends + "*A15-" + ResetCash + "*A16-" + CoinMechSerialNumber
                                    + "*A17-" + CoinMechModel + "*A18-" + CoinMechSoftwareVersion + "*A19-" + CoinMechTotalCash
                                    + "*A20-" + CoinMechTotalVends + "*A21-" + CoinMechResetCash + "*A22-" + CoinMechResetVends
                                    + "*A23-" + CashIn + "*A24-" + ToCashBoxReset + "*A25-" + CashToTubesReset + "*A26-" + ToCashBoxInit
                                    + "*A27-" + CashToTubesInit + "*A28-" + CashDispensedReset + "*A29-" + CashManualDispenseReset
                                    + "*A30-" + CashDispensedInit + "*A31-" + CashManualDispenseInit + "*A32-" + DiscountsValueReset
                                    + "*A33-" + DiscountsValueInit + "*A34-" + OverPayValueReset + "*A35-" + OverPayValueInit
                                    + "*A36-" + CashFillValueReset + "*A37-" + CashFillValueInit + "*A38-" + TubeValue
                                    + "*A39-" + CashlessCashMeter + "*A40-" + CashlessVendMeter
                                    + "*A41_" + ProductList;

                                if (File.Exists(MachineArchive + machinenumber + "-" + DexDate.ToString("MMM-yy") + ".dat"))
                                {
                                    string[] machineData = File.ReadAllLines(MachineArchive + machinenumber + "-" + DexDate.ToString("MMM-yy") + ".dat");

                                    foreach (string machineLine in machineData)
                                    {
                                        MachineData.Add(machineLine);
                                    }

                                    if (databaseData != "")
                                    {
                                        MachineData.Add(databaseData);
                                    }

                                    File.WriteAllLines(MachineArchive + machinenumber + "-" + DexDate.ToString("MMM-yy") + ".dat", MachineData);
                                }
                                else
                                {
                                    if (databaseData != "")
                                    {
                                        MachineData.Add(databaseData);
                                    }

                                    File.WriteAllLines(MachineArchive + machinenumber + "-" + DexDate.ToString("MMM-yy") + ".dat", MachineData);
                                }
                            }                      
                        }
                    }

                    foreach (string unzippedFile in unzippedFileNames)
                    {
                        string[] NameData = unzippedFile.Split('_', '.');
                        DateTime DexTime = DateTime.ParseExact(NameData[1] + NameData[2], "ddMMyyHHmmss", CultureInfo.InstalledUICulture);

                        File.Copy(TempDexFolder + unzippedFile, MachineDatabase + NameData[0] + ".dex", true);
                        File.SetCreationTime(MachineDatabase + NameData[0] + ".dex", DexTime);
                        File.Copy(TempDexFolder + unzippedFile, DexFileTray + NameData[0] + "_" + DexTime.ToString("yyyyMMdd_HHmmss") + "_" + NameData[3] + ".dex", true);
                        try
                        {
                            File.Move(TempDexFolder + unzippedFile, DexFolder + NameData[0] + "_" + DexTime.ToString("yyyyMMdd_HHmmss") + "_" + NameData[3] + ".dex");
                        }
                        catch
                        {
                            Error.Add("File could not be moved: " + TempDexFolder + unzippedFile);
                        }
                    }
                }
            }
            catch(Exception fileError)
            {
                Error.Add(fileError.ToString());
            }

            if (Error.Count > 0)
            {
                File.WriteAllLines(CrashLogs + @"CPI Crash Logs\" + "CPI_Logs_" + DateTime.Now.ToString("dd-MMM-yyyy_HH-mm-ss") + ".txt", Error);              
            }

            Console.WriteLine("Process complete with " + Error.Count.ToString() + " errors");
        }

        private void CleanTempFolder()
        {
            //List for error logging
            List<string> Error = new List<string>();
            try
            {
                //List for names of all files in temp folder
                List<string> TempFileNames = new List<string>();

                //List for CPI log data
                List<string> CPILogFileNames = new List<string>();

                //List for Nayax log data
                List<string> NayaxLogFileNames = new List<string>();

                //Directories to CPI and Nayax log files
                DirectoryInfo CPILogFilePath = new DirectoryInfo(SessionLogFolder + @"CPI\");
                DirectoryInfo NayaxLogFilePath = new DirectoryInfo(SessionLogFolder + @"Nayax\");

                foreach (FileInfo logFile in (CPILogFilePath.GetFiles("*.txt")))
                {
                    //Add CPI log file names to a list
                    CPILogFileNames.Add(logFile.Name);
                }

                foreach (FileInfo logFile in NayaxLogFilePath.GetFiles("*.txt"))
                {
                    //Add Nayax log file names to list
                    NayaxLogFileNames.Add(logFile.Name);
                }

                foreach (FileInfo tempFile in TempZipFolder.GetFiles("*.zip"))
                {
                    //Add temp file names to list
                    TempFileNames.Add(tempFile.Name);
                }

                //Foreach loop to delete all files in temp folder
                foreach (string file in TempFileNames)
                {
                    string filePath = TempZipFolder + file;
                    try
                    {
                        File.Delete(filePath);
                    }
                    catch
                    {
                        Error.Add("File could not be deleted: " + filePath);
                    }
                }

                foreach (string file in CPILogFileNames)
                {
                    //Path of log file
                    string filePath = CPILogFilePath + file;

                    //Split name of log file and add data to array
                    string[] fileNameData = file.Split('_', '.');

                    //Parse log file name date
                    DateTime logDate = DateTime.ParseExact(fileNameData[3] + fileNameData[4], "dd-MM-yyHH-mm-ss", CultureInfo.InvariantCulture);

                    //If log file directory does not exist create it
                    if (!Directory.Exists(CPILogFilePath + logDate.ToString("MMM-yyyy")))
                    {
                        Directory.CreateDirectory(CPILogFilePath + logDate.ToString("MMM-yyyy"));
                        try
                        {
                            //Move log file to created folder
                            File.Move(filePath, CPILogFilePath + logDate.ToString("MMM-yyyy") + @"\" + file);
                        }
                        catch
                        {
                            Error.Add("File could not be moved: " + filePath);
                        }
                    }
                    else
                    {
                        try
                        {
                            //Move log file to relevant folder
                            File.Move(filePath, CPILogFilePath + logDate.ToString("MMM-yyyy") + @"\" + file);
                        }
                        catch
                        {
                            Error.Add("File could not be moved: " + filePath);
                        }
                    }
                }

                foreach (string file in NayaxLogFileNames)
                {
                    //Path of Nayax log files
                    string filePath = NayaxLogFilePath + file;

                    //Split name and add data to array
                    string[] fileNameData = file.Split('_', '.');

                    //Parse log date from name data
                    DateTime logDate = DateTime.ParseExact(fileNameData[3] + fileNameData[4], "dd-MM-yyHH-mm-ss", CultureInfo.InvariantCulture);

                    //If directory doesnt exist create it
                    if (!Directory.Exists(NayaxLogFilePath + logDate.ToString("MMM-yyyy")))
                    {
                        Directory.CreateDirectory(NayaxLogFilePath + logDate.ToString("MMM-yyyy"));
                        try
                        {
                            //Move file to created directory
                            File.Move(filePath, NayaxLogFilePath + logDate.ToString("MMM-yyyy") + @"\" + file);
                        }
                        catch
                        {
                            Error.Add("File could not be moved: " + filePath);
                        }
                    }
                    else
                    {
                        try
                        {
                            //Move file to relevant directory
                            File.Move(filePath, NayaxLogFilePath + logDate.ToString("MMM-yyyy") + @"\" + file);
                        }
                        catch
                        {
                            Error.Add("File could not be moved: " + filePath);
                        }
                    }
                }
            }
            catch(Exception fileCleanError)
            {
                //Catch any errors caused during runtime and add to list
                Error.Add(fileCleanError.ToString());
            }

            //If there are any errors write them to a log file
            if (Error.Count > 0)
            {
                File.WriteAllLines(CrashLogs + @"File Error Logs\" + "File_Error_" + DateTime.Now.ToString("dd-MMM-yyyy_HH-mm-ss") + ".txt", Error);
            }
                       
            //Display in console if any errors occurred
            Console.WriteLine("Cleaning completed with " + Error.Count.ToString() + " errors");            
        }
    }
}
