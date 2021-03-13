using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using CSR;
using System.Threading;

namespace SimplePath
{
    public class Program
    {
        public static void init(MCCSAPI api)
        {

            Task t = new Task(() =>
             {
                 Thread.Sleep(20000);
                 Console.WriteLine("[SimplePath]开始清理根目录无用文件...");
                 delete("bedrock_server_how_to.html");
                 delete("release-notes.txt");
                 delete("valid_known_packs.json");
                 delete("cvd2.exe");
                 delete("bedrock_server.pdb");
                 delete("RoDB.exe");
                 delete("bedrock_server.symdef");
                 delete_path("development_behavior_packs");
                 delete_path("development_resource_packs");
                 delete_path("development_skin_packs");
                 delete_path("world_templates");
                 //delete(@"./internalStorage/Dedicated_Server.txt");
                 delete_path("internalStorage");
                 
             });
            t.Start();
        }
        public static void delete(string a)
        {
            if (File.Exists(a))
            {
                File.Delete(a);
            }
        }
        public static void delete_path(string a)
        {
            if (Directory.Exists(a))
            {
                Directory.Delete(a,true);
            }
        }
    }
}
