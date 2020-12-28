using SecurityManager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subscriber
{
    public class Writer
    {
        public string Path { get; set; }

        public Writer(string path)
        {
            Path = path;
        }

        public void Write(string text, string entityId, string signature, string publicKey )
        {
            using (StreamWriter sw = new StreamWriter(Path,true)) 
            {
                sw.WriteLine(text);
                Audit.DatabaseInput(DateTime.Now, Path, entityId ,signature, publicKey );
            }
        }


    }
}
