using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using StackExchange.Redis.Message.Common.Interface;

namespace StackExchange.Redis.Message.Common.Messages
{
    public class FileWriteMessage : IMessage, IKillable
    {
        public List<string> Lines { get; set; }
        public string FileName { get; set; }

        public FileWriteMessage(string fileName)
        {
            FileName = fileName;
        }

        public void Execute()
        {
            PrepareFolders();

            using (var writer = new StreamWriter(File.Open(FileName,FileMode.Append)))
            {
                foreach (var line in Lines)
                {
                    writer.WriteLine(line);
                }
            }
        }

        public bool IsKillMessage { get; set; }

        private void PrepareFolders()
        {
            var folder = Path.GetDirectoryName(FileName);

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
        }
    }
}
