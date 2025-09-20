using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallDemoManager.HelperClass
{
    /// <summary>
    /// Provides a static container for storing information about audio files
    /// that were discovered in the "Saved-Voice-Files" directory.
    /// </summary>
    public static class SavedAudioFiles
    {
        /// <summary>
        /// A collection of all discovered audio files, including their folder name, file name, and full path.
        /// </summary>
        public static List<AudioFileInfo> Files { get; } = new List<AudioFileInfo>();

        /// <summary>
        /// Represents metadata about a single audio file, 
        /// including the folder where it is located, its name, and its absolute path.
        /// </summary>
        public class AudioFileInfo
        {
            /// <summary>
            /// The name of the folder where the audio file is located.
            /// </summary>
            public string FolderName { get; set; }

            /// <summary>
            /// The file name of the audio file (without the folder path).
            /// </summary>
            public string FileName { get; set; }

            /// <summary>
            /// The full absolute path to the audio file, including folder and file name.
            /// </summary>
            public string FullPath { get; set; }
        }
    }
}
