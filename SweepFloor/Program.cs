using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CSR;
namespace SweepFloor
{
    public class Program
    {
        public static void init(MCCSAPI api)
        {
             bool f = true;
            api.addBeforeActListener(EventKey.onServerCmdOutput, x =>
             {
                 var b = BaseEvent.getFrom(x) as ServerCmdOutputEvent;
                 if (f&&b.output== "No targets matched selector")
                 {
                     return false;
                 }
                 //Console.WriteLine("OUTPUT:{0}",b.output);
                 return true;
             });
            api.addBeforeActListener(EventKey.onServerCmd, x =>
             {
                 var a = BaseEvent.getFrom(x) as ServerCmdEvent;
                 if (a.cmd == "cleaner")
                 {
                     if (f == true)
                     { 
                         f = false;
                         Console.WriteLine("[Cleaner]自动清理功能关闭");
                         return false;
                     }
                     else
                     { f = true; Console.WriteLine("[Cleaner]自动清理功能开启"); }
                     return false;
                 }
                 return true;
             });
            
            Task t = new Task(() =>
        {
            while (f)
            {
                api.runcmd("tellraw @a {\"rawtext\":[{\"text\":\"§b[§e扫地姬§b]§a掉落物将在§c60§a秒后清除！\"}]}");
                Thread.Sleep(30000);
                api.runcmd("tellraw @a {\"rawtext\":[{\"text\":\"§b[§e扫地姬§b]§a掉落物将在§c30§a秒后清除！\"}]}");
                Thread.Sleep(20000);
                api.runcmd("tellraw @a {\"rawtext\":[{\"text\":\"§b[§e扫地姬§b]§a掉落物将在§c10§a秒后清除！\"}]}");
                Thread.Sleep(5000);
                api.runcmd("tellraw @a {\"rawtext\":[{\"text\":\"§b[§e扫地姬§b]§a马上你的掉落物就没了，还不捡起来！\"}]}");
                Thread.Sleep(5000);
                api.runcmd("tellraw @a {\"rawtext\":[{\"text\":\"§b[§e扫地姬§b]§a掉落物清除完毕！\"}]}");
                api.runcmd("kill @e[type=item]");
                Thread.Sleep(1200000);
            }
        });
            t.Start();
            
        }
    }
    
}
namespace CSR
{
    partial class Plugin
    {
        public static void onStart(MCCSAPI api)
        { 
            SweepFloor.Program.init(api);
Console.WriteLine("[SweepFloor]加载成功！");
        }
    }
}
    
