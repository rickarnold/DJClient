using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace CDG.Validation
{
    /// <summary>
    /// Encapsulates a file containing the validation repairs
    /// </summary>
    public class ResultsFile
    {
        #region Constants

        string FILE_ID_STRING = "CDGEditorRepair";
        const int FILE_VERSION = 1;

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of <see cref="RepairFile"/>.
        /// </summary>
        public ResultsFile()
        {
            _Results = new List<Result>();
        }

        /// <summary>
        /// Initializes a new instance of <see cref="RepairFile"/>.
        /// </summary>
        /// <param name="results">Results to save to file.</param>
        public ResultsFile(List<Result> results)
        {
            _Results = results;
        }

        #endregion

        #region Public Properties

        public List<Validation.Result> Results
        {
            get
            {
                return _Results;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Saves the results to a file.
        /// </summary>
        /// <param name="path">The path to save the result to.</param>
        /// <exception cref="System.IO.IOException">Thrown if there is a file access error.</exception>
        public void Save(string path)
        {
            // Repair file format:
            //
            // Header:
            // 16 bytes - always "CDGEditor:Repair"
            // 4 bytes - repair file version

            // Repairs:
            // 1 byte:  Repair type
            // 4 bytes: Repair size
            // 4 bytes: Repair version
            // n bytes: Repair data (repair size bytes)

            // Don't catch file exceptions here - this class has no means of 
            // dealing with them - leave that to the application.
            using (FileStream stream = File.Create(path))
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    WriteHeader(writer);

                    if (_Results != null)
                    {
                        foreach (Result result in _Results)
                        {
                            result.Save(writer);
                        }
                    }
                    writer.Close();
                }
            }
        }

        /// <summary>
        /// Loads the results from a file.
        /// </summary>
        /// <param name="path">The path to load the results from.</param>
        /// <param name="cdgFile">The CDG file to get chunks from.</param>
        public void Load(string path, CDGFile cdgFile)
        {
            using(FileStream stream = File.OpenRead(path))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    int version = ReadHeader(reader);

                    if (version > 0)
                    {
                        // Proceed to read the result objects from the stream
                        Result result = Result.Load(cdgFile, reader);
                        while (result != null && stream.Position < stream.Length)
                        {
                            _Results.Add(result);
                            result = Result.Load(cdgFile, reader);
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException(
                            string.Format("{0} is not a valid repairs file.", path));
                    }
                }
            }
        }

        #endregion

        #region Private Methods

        void WriteHeader(BinaryWriter writer)
        {
            writer.Write(FILE_ID_STRING);
            writer.Write(FILE_VERSION);
        }

        /// <summary>
        /// Reads the file header.
        /// </summary>
        /// <param name="reader">The <see cref="BinaryReader"/> to read the header from.</param>
        /// <returns>The file version number.</returns>
        int ReadHeader(BinaryReader reader)
        {
            int result = 0;

            string id = reader.ReadString();
            if (id.CompareTo(FILE_ID_STRING) == 0)
            {
                result = reader.ReadInt32();
            }

            return result;
        }

        #endregion

        #region Data

        /// <summary>
        /// Validation results read from file or to write to file.
        /// </summary>
        List<Validation.Result> _Results;

        #endregion
    }
}
