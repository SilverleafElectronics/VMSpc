﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using VMSpc.DevHelpers;
using VMSpc.Exceptions;
using VMSpc.JsonFileManagers;
using VMSpc.Loggers;

namespace VMSpc.UI.CustomComponents
{
    /// <summary>
    /// Interaction logic for FileSelector.xaml
    /// </summary>
    public partial class FileSelector : Window
    {
        protected string CurrentFilepath { get; set; }
        protected string RelativeFilePath {get;set;}
        protected string RelativeDirectoryPath {get;set;}
        protected List<string> DisplayedfileNames;
        protected List<string> AllowedExtensions {get;set;}
        public string ResultFilePath { get; set; }
        public bool ExcludeLockedFiles { get; set; } = false;
        /// <summary>
        /// Allows the user to add a new empty file to this directory
        /// </summary>
        public bool AllowNewFiles { get; set; } = false;
        /// <summary>
        /// Allows the user to import files into this directory from anywhere in their filesystem
        /// </summary>
        public bool AllowImports { get; set; } = false;
        public bool AllowDeletes { get; set; } = false;
        /// <summary>
        /// The filter that should be used in the OpenFileDlg when importing files. See
        /// https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.filedialog.filter?view=net-5.0#System_Windows_Forms_FileDialog_Filter
        /// for explanation and examples of filters. This property only applies if AllowImports is true
        /// </summary>
        public string ImportFilter { get; set; }
        /// <summary>
        /// Optional addition to ImportFilter. The default extension that will be used when opening
        /// an OpenFileDlg
        /// </summary>
        public string DefaultImportExtension { get; set; }
        /// <summary>
        /// The string extension that will automatically be added to whatever filename the
        /// user enters when adding a new file. Only applies if AllowNewFiles is true
        /// </summary>
        public string NewFilesExtension { get; set; }

        public FileSelector(string RelativeDirectoryPath, string RelativeFilePath, params string[] AllowedExtensions)
        {
            CurrentFilepath = string.Copy(RelativeFilePath);
            DisplayedfileNames = new List<string>();
            this.RelativeDirectoryPath = RelativeDirectoryPath;
            this.RelativeFilePath = RelativeFilePath;
            if (AllowedExtensions.Length == 0)
            {
                this.AllowedExtensions = null;
            }
            else
            {
                this.AllowedExtensions = AllowedExtensions.ToList();
            }
            InitializeComponent();
        }

        protected override void OnActivated(EventArgs e)
        {
            RemoveDisallowedButtons();
            AddFileRows();
            base.OnActivated(e);
        }

        private void RemoveDisallowedButtons()
        {
            if (!(AllowNewFiles || AllowImports))
            {
                MainGrid.Children.Remove(OptionalButtonsGrid);
                MainGrid.RowDefinitions.Remove(OptionalButtonsRow);
            }
            else
            {
                if (!AllowNewFiles)
                {
                    OptionalButtonsGrid.Children.Remove(NewFileButton);
                    OptionalButtonsGrid.ColumnDefinitions.Remove(NewFileButtonColumn);
                }
                if (!AllowImports)
                {
                    OptionalButtonsGrid.Children.Remove(ImportButton);
                    OptionalButtonsGrid.ColumnDefinitions.Remove(ImportButtonColumn);
                }
            }
        }

        private void AddFileRows()
        {
            DisplayedFiles.Items.Clear();
            DisplayedfileNames.Clear();
            foreach (var filename in FileOpener.GetDirectoryFiles(RelativeDirectoryPath))
            {
                if (PassesConditions(filename))
                {
                    var item = new ListBoxItem()
                    {
                        Content = FileOpener.GetFileName(filename)
                    };
                    DisplayedFiles.Items.Add(item);
                    DisplayedfileNames.Add(FileOpener.GetFileName(filename));
                    var resolvedItemName = FileOpener.GetFileName(filename);
                    var resolvedCurrentFile = FileOpener.GetFileName(RelativeFilePath);
                    if (resolvedItemName.Equals(resolvedCurrentFile))
                    {
                        item.IsSelected = true;
                    }
                }
            }
        }

        private bool PassesConditions(string filename)
        {
            if (!PassesFilter(filename))
            {
                return false;
            }
            if (ExcludeLockedFiles)
            {
                bool isUsed = !FileOpener.IsFileLocked(filename, FilePathType.Absolute);
                return isUsed;
            }
            return true;
        }

        private bool PassesFilter(string filename)
        {
            if (AllowedExtensions == null)
            {
                return true;
            }
            else
            {
                foreach (var extension in AllowedExtensions)
                {
                    var extensionLength = extension.Length;
                    var fileExtension = filename.Substring(filename.Length - extensionLength).ToLower();
                    if (fileExtension.Equals(extension.ToLower()))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            ResultFilePath = RelativeDirectoryPath + "\\" + ((DisplayedFiles.SelectedItem as ListBoxItem).Content as string);
            DialogResult = true;
            Close();
        }

        private void DisplayedFiles_MouseDown(object sender, MouseButtonEventArgs e)
        {
            VMSConsole.PrintLine("The stack panel has been clicked");
        }

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog()
            {
                Multiselect = true,
                InitialDirectory = System.IO.Path.GetPathRoot(Environment.SystemDirectory),
            };
            if (!string.IsNullOrEmpty(ImportFilter))
            {
                openFileDialog.Filter = ImportFilter;
            }
            if (!string.IsNullOrEmpty(DefaultImportExtension))
            {
                openFileDialog.DefaultExt = DefaultImportExtension;
            }
            if (openFileDialog.ShowDialog() == true)
            {
                bool hasInvalidFiles = false;
                var invalidFiles = new List<string>();
                foreach (string filename in openFileDialog.FileNames)
                {
                    try
                    {
                        FileOpener.CopyFile(filename, RelativeDirectoryPath + "\\" + FileOpener.GetFileName(filename));
                    }
                    catch (IOException) //should only get thrown if a user tries to import a file from the same directory
                    {
                        hasInvalidFiles = true;
                        invalidFiles.Add(FileOpener.GetFileName(filename));
                    }
                    catch (Exception ex)
                    {
                        ErrorLogger.GenerateErrorRecord(ex);
                    }
                }
                if (hasInvalidFiles)
                {
                    string errorMessage = "Some files failed to copy because of naming conflicts with files in the current directory. Please rename the following files before trying to import them: ";
                    foreach (var file in invalidFiles)
                    {
                        errorMessage += "\n" + file;
                    }
                    MessageBox.Show(errorMessage);
                }
            }
            AddFileRows();
        }

        private void NewFileButton_Click(object sender, RoutedEventArgs e)
        {
            var addFileDlg = new AddFileDlg()
            {
                Extension = NewFilesExtension,
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
            };
            bool added = false;
            if ((bool)addFileDlg.ShowDialog())
            {
                if (!string.IsNullOrEmpty(addFileDlg.FileName))
                {
                    var newFile = addFileDlg.FileName + addFileDlg.Extension;
                    if (!DisplayedfileNames.Contains(newFile))
                    {
                        FileOpener.WriteAllText(RelativeDirectoryPath + "\\" + newFile, string.Empty);
                        added = true;
                    }
                    else
                    {
                        MessageBox.Show($"Cannot add {newFile}, because it already exists in this directory. Please enter a unique file name when adding new files");
                    }
                }
                else
                {
                    MessageBox.Show("Cannot add a new file without a file name");
                }
            }
            AddFileRows();
            if (added)
            {
                foreach (var item in DisplayedFiles.Items)
                {
                    if ((item as ListBoxItem) != null)
                    {
                        if ((item as ListBoxItem).Content.ToString() == (addFileDlg.FileName + addFileDlg.Extension))
                        {
                            (item as ListBoxItem).IsSelected = true;
                        }
                    }
                }
            }
        }
    }
}
