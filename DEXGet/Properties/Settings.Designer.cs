﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DEXGet.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "15.3.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("\\DEX\\Archive\\")]
        public string DexFolderPath {
            get {
                return ((string)(this["DexFolderPath"]));
            }
            set {
                this["DexFolderPath"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("\\Excel Files\\Input\\input.xml")]
        public string ExcelFilePath {
            get {
                return ((string)(this["ExcelFilePath"]));
            }
            set {
                this["ExcelFilePath"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("\\DEX\\Machine Database\\")]
        public string MachineDatabasePath {
            get {
                return ((string)(this["MachineDatabasePath"]));
            }
            set {
                this["MachineDatabasePath"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("\\Luci Files\\DEX File Tray\\")]
        public string DexFileTray {
            get {
                return ((string)(this["DexFileTray"]));
            }
            set {
                this["DexFileTray"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Enter SFTP server IP here")]
        public string SFTP_IP {
            get {
                return ((string)(this["SFTP_IP"]));
            }
            set {
                this["SFTP_IP"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Enter SFTP account username here")]
        public string SFTP_Username {
            get {
                return ((string)(this["SFTP_Username"]));
            }
            set {
                this["SFTP_Username"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Enter SFTP account password here")]
        public string SFTP_Password {
            get {
                return ((string)(this["SFTP_Password"]));
            }
            set {
                this["SFTP_Password"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Enter SFTP server directory of text files")]
        public string SFTP_Path_to_Nayax_folder {
            get {
                return ((string)(this["SFTP_Path_to_Nayax_folder"]));
            }
            set {
                this["SFTP_Path_to_Nayax_folder"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Enter SFTP server directory of ZIP files containing text files")]
        public string SFTP_Path_to_CPI_folder {
            get {
                return ((string)(this["SFTP_Path_to_CPI_folder"]));
            }
            set {
                this["SFTP_Path_to_CPI_folder"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("22")]
        public string SFTP_PortNumber {
            get {
                return ((string)(this["SFTP_PortNumber"]));
            }
            set {
                this["SFTP_PortNumber"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Enter SFTP account SSH key here")]
        public string SFTP_SSHKey {
            get {
                return ((string)(this["SFTP_SSHKey"]));
            }
            set {
                this["SFTP_SSHKey"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("\\DEX\\Temp\\")]
        public string TempDexFolder {
            get {
                return ((string)(this["TempDexFolder"]));
            }
            set {
                this["TempDexFolder"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("\\DEX\\Machine Archive\\")]
        public string MachineArchive {
            get {
                return ((string)(this["MachineArchive"]));
            }
            set {
                this["MachineArchive"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("\\DEX\\ZIP Files\\")]
        public string ZipFolder {
            get {
                return ((string)(this["ZipFolder"]));
            }
            set {
                this["ZipFolder"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("\\DEX\\ZIP Files\\Temp\\")]
        public string TempZipFolder {
            get {
                return ((string)(this["TempZipFolder"]));
            }
            set {
                this["TempZipFolder"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("\\Error Logs\\DEXGet Logs\\WinSCP Session Logs\\")]
        public string WinSCP_Session_Logs {
            get {
                return ((string)(this["WinSCP_Session_Logs"]));
            }
            set {
                this["WinSCP_Session_Logs"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("\\Error Logs\\DEXGet Logs\\Master Log\\")]
        public string MasterErrorLogFolder {
            get {
                return ((string)(this["MasterErrorLogFolder"]));
            }
            set {
                this["MasterErrorLogFolder"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("\\Error Logs\\DEXGet Logs\\")]
        public string Crash_Logs {
            get {
                return ((string)(this["Crash_Logs"]));
            }
            set {
                this["Crash_Logs"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Insert workbench folder directory here(Master folder for all files. Should be an " +
            "empty folder)")]
        public string Lucifer_2017_Database_Path {
            get {
                return ((string)(this["Lucifer_2017_Database_Path"]));
            }
            set {
                this["Lucifer_2017_Database_Path"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string Session_Logs_Folder {
            get {
                return ((string)(this["Session_Logs_Folder"]));
            }
            set {
                this["Session_Logs_Folder"] = value;
            }
        }
    }
}