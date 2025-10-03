using MdxLib.Model;
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
using Wa3Tuner.Helper_Classes;

namespace Wa3Tuner.Dialogs
{
    /// <summary>
    /// Interaction logic for Batch_Texture_Renamer.xaml
    /// </summary>
    public partial class Batch_Texture_Renamer : Window
    {
        private Dictionary<string, CModel> models = new Dictionary<string, CModel>();
        private List<string> textures = new List<string>();
        string ExcludedFile = string.Empty;
        StringBuilder Log;
        public Batch_Texture_Renamer(string exclude)
        {
            InitializeComponent();
            ExcludedFile = exclude;
            Log = new StringBuilder();
            RegisterEventForRAdioButtons();
        }
        private void RegisterEventForRAdioButtons()
        {
            checkName.Checked += CheckIfEnableReplace;
            checkAdd.Unchecked += CheckIfEnableReplace;
            checkSuffix.Checked += CheckIfEnableReplace;
            checkPrefix.Checked += CheckIfEnableReplace;
            checkRemove.Unchecked += CheckIfEnableReplace;
            checkPath.Checked += CheckIfEnableReplace;
            checkReplace.Unchecked += CheckIfEnableReplace;
            checkall.Checked += CheckIfEnableReplace;
        }
        private void FillTextures()
        {
            foreach (var model in models.Values)
            {
                foreach (var texture in model.Textures)
                {
                    if (texture.ReplaceableId != 0) continue; // Skip replaceable textures
                    if (texture.FileName.Length == 0) continue; // Skip empty texture names
                    if (!textures.Contains(texture.FileName, StringComparer.OrdinalIgnoreCase))
                    {
                        textures.Add(texture.FileName);
                    }
                }
            }
            
        }
        private void CollectModels()
        {
            models.Clear();
            string[] files;

            // Get all .mdx and .mdl files
            try {
                  files = Directory.EnumerateFiles(txtFolderPath.Text, "*.*", SearchOption.AllDirectories)
                               .Where(f => f.EndsWith(".mdx") || f.EndsWith(".mdl"))
                               .ToArray();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error accessing files: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                txtFolderPath.Text = string.Empty; return;
            }
          

            if (files.Length == 0)
            {
                MessageBox.Show("No .mdx or .mdl files found in the specified folder.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                txtFolderPath.Text = string.Empty;return;
            }
            foreach (var file in files)
            {
                CModel? model = ModelSaverLoader.Load(file);
                if (model == null) continue;
                models.Add(file, model);
            }
            if (models.ContainsKey(ExcludedFile))
            {
                models.Remove(ExcludedFile);
            }
            txtSelectedFolder.Text = $"Loaded: {files.Length}";
            FillTextures();
        }
        private void SaveAll()
        {
            EnableAllControls(false);
            foreach (var kvp in models)
            {
                string filePath = kvp.Key;
                CModel model = kvp.Value;
                ModelSaverLoader.Save(model, filePath);
            }
            EnableAllControls(true);
        }
        private void EnableAllControls(bool yes)
        {

            btnReplaceTextureName.IsEnabled = yes;
            btnBrowseFolder.IsEnabled = yes;
            btnClear.IsEnabled = yes;
        }
     
        private void BtnBrowseFolder_Click(object sender, RoutedEventArgs e)
        {
            txtFolderPath.Text = FileSeeker.BrowseForFolder();
            if (Directory.Exists(txtFolderPath.Text))
            {
                CollectModels();
            }
        }

        private bool check()
        {
            if (Directory.Exists(txtFolderPath.Text) == false)
            {
                MessageBox.Show("The specified folder path does not exist.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            else if (models.Count == 0)
            {
                MessageBox.Show("No models found in the specified folder.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }

        private Affix GetAffix()
        {
            if (checkSuffix.IsChecked == true) return Affix.Suffix;
            if (checkPrefix.IsChecked == true) return Affix.Prefix;
            return Affix.All;
        }
        private ActionType GetActionType()
        {
            if (checkAdd.IsChecked == true) return ActionType.Add;
            if (checkRemove.IsChecked == true) return ActionType.Remove;
            return ActionType.Replace;
        }
        private PartOfString GetPartOfString()
        {
            if (checkPath.IsChecked == true) return PartOfString.Path;
            return PartOfString.Name;
        }
        private enum Affix { Prefix,Suffix, All }
        private enum PartOfString {Path, Name }
        private enum ActionType { Add, Remove, Replace }
        private string ModifyString(string original, string replacement, Affix affix, PartOfString part, ActionType actionType)
        {
            if (string.IsNullOrEmpty(original))
                return original;

            string target = part == PartOfString.Path ? original : System.IO.Path.GetFileNameWithoutExtension(original);
            string directory = part == PartOfString.Name ? System.IO.Path.GetDirectoryName(original) ?? string.Empty : string.Empty;
            string extension = part == PartOfString.Name ? System.IO.Path.GetExtension(original) : string.Empty;

            bool HasPrefix() => target.StartsWith(replacement, StringComparison.OrdinalIgnoreCase);
            bool HasSuffix() => target.EndsWith(replacement, StringComparison.OrdinalIgnoreCase);

            bool hasAffix = affix switch
            {
                Affix.Prefix => HasPrefix(),
                Affix.Suffix => HasSuffix(),
                Affix.All => true, // All means the whole string is considered
                _ => false
            };

            switch (actionType)
            {
                case ActionType.Add:
                    if (!hasAffix || affix == Affix.All)
                    {
                        target = affix switch
                        {
                            Affix.Prefix => replacement + target,
                            Affix.Suffix => target + replacement,
                            Affix.All => replacement, // replace entire string
                            _ => target
                        };
                    }
                    break;

                case ActionType.Remove:
                    target = affix switch
                    {
                        Affix.Prefix => HasPrefix() ? target.Substring(replacement.Length) : target,
                        Affix.Suffix => HasSuffix() ? target.Substring(0, target.Length - replacement.Length) : target,
                        Affix.All => string.Empty, // remove entire string
                        _ => target
                    };
                    break;

                case ActionType.Replace:
                    target = affix switch
                    {
                        Affix.Prefix => HasPrefix() ? replacement + target.Substring(replacement.Length) : replacement + target,
                        Affix.Suffix => HasSuffix() ? target.Substring(0, target.Length - replacement.Length) + replacement : target + replacement,
                        Affix.All => replacement, // replace entire string
                        _ => target
                    };
                    break;
            }

            if (part == PartOfString.Name)
                return string.IsNullOrEmpty(directory) ? target + extension : System.IO.Path.Combine(directory, target + extension);

            return target;
        }

        public static bool HasExtension(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return false;

            return !string.IsNullOrEmpty(System.IO.Path.GetExtension(filePath));
        }
        private void ChangeTextures(TextBox t_original,TextBox t_replacement,  Affix a, PartOfString p, ActionType k)
        {
            if (!check()) return;
            string original = t_original.Text.Trim();
            string replacement = t_replacement.Text.Trim();
            if (original.Length == 0)
            {
                MessageBox.Show("Please enter a valid string.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (k == ActionType.Replace && replacement.Length == 0)
            {
                MessageBox.Show("Please enter a valid replacement string.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            foreach (var kvp in models)
            {
                CModel model = kvp.Value;
                foreach (var texture in model.Textures)
                {
                    if (texture.ReplaceableId != 0) continue; // Skip replaceable textures
                    if (texture.FileName.Length == 0) continue; // Skip empty texture names
                    texture.FileName = ModifyString(texture.FileName, original, a, p, k);
                }
            }
            SaveAll();
        }

        

        private void SetWar3Imported(object sender, RoutedEventArgs e)
        {
            tsxtOriginal.Text = "war3mapImported";
        }

        private void clearModels(object sender, RoutedEventArgs e)
        {
            models.Clear();
            textures.Clear();
            txtFolderPath.Text = string.Empty;
        }
       private void stipallPaths(object sender, RoutedEventArgs e)
        {

            if (!check()) return;
          
            foreach (var kvp in models)
            {
                CModel model = kvp.Value;
                foreach (var texture in model.Textures)
                {
                    texture.FileName = System.IO.Path.GetFileName(texture.FileName);
                }
            }
            SaveAll();
            AddLog($"Stripped all paths from texture names in {models.Count} in folder {txtFolderPath.Text}.");
        }
        private void AddLog(string message)
        {
            string message2 = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}";
            Log.AppendLine(message2);

            txtLog.Text = Log.ToString();
        }
        private void txtAddPrefixName_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void CheckIfEnableReplace(object sender, RoutedEventArgs e)
        {
           
                txtReplacement.IsEnabled = checkReplace.IsChecked == true;
            


        }

        private void BtnReplaceTextureName_Click(object sender, RoutedEventArgs e)
        {
            ChangeTextures(tsxtOriginal, txtReplacement, GetAffix(), GetPartOfString(), GetActionType());
            AddLog($"Replaced textures in {models.Count} models in folder {txtFolderPath.Text}. Saved.");
            DialogResult = true;
        }

        private void checkall_Checked(object sender, RoutedEventArgs e)
        {
            checkReplace.IsChecked = true;
        }

        private void peek(object sender, RoutedEventArgs e)
        {
            stringContainer sc = new stringContainer(ref textures);
            sc.ShowDialog();
        }
    }
}
