using SmallDemoManager.HelperClass;
using ReaLTaiizor.Child.Material;
using ReaLTaiizor.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallDemoManager.AudioExtract
{
    /// <summary>
    /// Provides static methods to check for saved audio files, 
    /// load them into memory, and populate a MaterialListBox with the results.
    /// </summary>
    public static class AudioFileManager
    {
        /// <summary>
        /// Checks if the "Saved-Voice-Files" folder exists inside the given base path.
        /// </summary>
        /// <param name="basePath">The base directory where the "Saved-Voice-Files" folder is expected to be located.</param>
        /// <returns>True if the folder exists, otherwise false.</returns>
        public static bool SavedVoiceFilesExists(string basePath)
        {
            string targetPath = Path.Combine(basePath, "Saved-Voice-Files");
            return Directory.Exists(targetPath);
        }

        /// <summary>
        /// Scans all subfolders inside "Saved-Voice-Files" for .wav files 
        /// and stores them in the static <see cref="SavedAudioFiles"/> class.
        /// </summary>
        /// <param name="basePath">The base directory where the "Saved-Voice-Files" folder is located.</param>
        public static void LoadAudioFiles(string basePath)
        {
            // Clear previously loaded files before scanning again
            SavedAudioFiles.Files.Clear();

            string targetPath = Path.Combine(basePath, "Saved-Voice-Files");
            if (!Directory.Exists(targetPath))
                return;

            // Iterate through each subdirectory inside "Saved-Voice-Files"
            foreach (var dir in Directory.GetDirectories(targetPath))
            {
                string folderName = Path.GetFileName(dir);

                // Add every .wav file found in the current folder
                foreach (var file in Directory.GetFiles(dir, "*.wav"))
                {
                    string fileName = Path.GetFileName(file);

                    SavedAudioFiles.Files.Add(new SavedAudioFiles.AudioFileInfo
                    {
                        FolderName = folderName,
                        FileName = fileName,
                        FullPath = file
                    });
                }
            }
        }

        /// <summary>
        /// Populates the given MaterialListBox with the audio files
        /// stored in <see cref="SavedAudioFiles"/>.
        /// </summary>
        /// <param name="listBox">The MaterialListBox to be filled with audio file entries.</param>
        public static void PopulateListBox(MaterialListBox listBox)
        {
            // Clear old items before adding new ones
            listBox.Clear();

            // Add each audio file as a list item with FolderName as Text and FileName as SecondaryText
            foreach (var audioFile in SavedAudioFiles.Files)
            {
                listBox.Items.Add(new MaterialListBoxItem
                {
                    Text = audioFile.FolderName,
                    SecondaryText = audioFile.FileName
                });
            }
        }
    }
}
