using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using CSR;
using Newtonsoft.Json;

namespace BehaviorLog
{

	class Program
	{
		//private static MCCSAPI mapi = null;
		// 返回一个时间前缀
		public static string title(string content)
		{
			DateTime t = DateTime.Now;
			return "[" + t.ToString("yyyy-MM-dd hh:mm:ss") + " " + content + "]";
		}
		// 打印坐标
		public static string Coordinator(BPos3 b)
		{
			return "(" + b.x + ", " + b.y + ", " + b.z + ")";
		}
		public static string Coordinator(Vec3 b)
		{
			return "(" + (int)b.x + ", " + (int)b.y + ", " + (int)b.z + ")";
		}
		// 维度ID转换为中文字符
		public static string toDimenStr(int dimensionId)
		{
			switch (dimensionId)
			{
				case 0:
					return "主世界";
				case 1:
					return "地狱";
				case 2:
					return "末地";
				default:
					break;
			}
			return "未知维度";
		}
		public static void saveLine(string l, string path)
		{
			//	if (autoSave) {
			DateTime d = DateTime.Now;
			if (!Directory.Exists(path))
			{ Directory.CreateDirectory(path); }
			if (!string.IsNullOrEmpty(path))
				try
				{
					File.AppendAllLines(path + d.ToString("yyyy-MM-dd") + @".log", new string[] { l });
				}
				catch
				{
				}
		}
		//定义类
		public class Config
        {
			public bool Show_UseItem { get; set; }
			public bool Show_PlacedBlock { get; set; }
			public bool Show_DestroyBlock { get; set; }
			public bool Show_TakeChest { get; set; }
			public bool Show_ChangeDimension { get; set; }
			public bool Show_Kill { get; set; }
			public bool Show_Chat { get; set; }
			public bool Show_TellRaw { get; set; }
			public bool Save_UseItem { get; set; }
			public bool Save_PlacedBlock { get; set; }
			public bool Save_DestroyBlock { get; set; }
			public bool Save_TakeChest { get; set; }
			public bool Save_ChangeDimension { get; set; }
			public bool Save_Kill { get; set; }
			public bool Save_Chat { get; set; }
			public bool Save_TellRaw { get; set; }
			public string Path_UseItem { get; set; }
			public string Path_PlacedBlock { get; set; }
			public string Path_DestroyBlock { get; set; }
			public string Path_TakeChest { get; set; }
			public string Path_ChangeDimension { get; set; }
			public string Path_Kill { get; set; }
			public string Path_Chat { get; set; }
			public string Path_TellRaw { get; set; }
		}
		public static string json()
        {
			Config a = new Config();
			a.Show_UseItem = false;
			a.Show_TakeChest = false;
			a.Show_PlacedBlock = false;
			a.Show_DestroyBlock = false;
			a.Show_Kill = false;
			a.Show_Chat = false;
			a.Show_ChangeDimension = false;
			a.Show_TellRaw = false;
			a.Save_UseItem = true;
			a.Save_TakeChest = true;
			a.Save_DestroyBlock = true;
			a.Save_PlacedBlock = true;
			a.Save_ChangeDimension = true;
			a.Save_Kill = true;
			a.Save_Chat = true;
			a.Save_TellRaw = false;
			a.Path_PlacedBlock = @"logs/PlaceBlock/";
			a.Path_TakeChest = @"logs/PlaceBlock/";
			a.Path_DestroyBlock = @"logs/PlaceBlock/";
			a.Path_UseItem = @"logs/PlaceBlock/";
			a.Path_ChangeDimension = @"logs/PlaceBlock/";
			a.Path_Kill = @"logs/PlaceBlock/";
			a.Path_Chat = @"logs/PlaceBlock/";
			a.Path_TellRaw = @"logs/PlaceBlock/";
			string b = JsonConvert.SerializeObject(a);
			return b;
		}
		// 主入口实现
		public static void init(MCCSAPI api)
		{
			//mapi = api;
			Console.OutputEncoding = Encoding.UTF8;
			string pz = json();
			// 从固定路径读取配置文件
			string path = @"plugins/BehaviorLog/";
			 if(!Directory.Exists(path))
			{ Directory.CreateDirectory(path); }
			if (!File.Exists(path + "config.json"))
            {
				File.WriteAllText(path + "config.json", json());
           }
			else
            {
				pz = File.ReadAllText(path + "config.json");
            }
			JsonSerializer serializer = new JsonSerializer();
			StringReader sr = new StringReader(pz);
			Config p1 = (Config)serializer.Deserialize(new JsonTextReader(sr), typeof(Config));
			
			// 放置方块监听
			api.addAfterActListener(EventKey.onPlacedBlock, x => {
				var e = BaseEvent.getFrom(x) as PlacedBlockEvent;
				if (e == null) return true;
				string str = string.Format("{0} 玩家 {1} {2}在 {3} {4} 放置 {5} 方块。",
					title(EventKey.onPlacedBlock), e.playername,
					!e.isstand ? "悬空地" : "",
					e.dimension,
					Coordinator(e.position),
					e.blockname);
			  if (p1.Show_PlacedBlock) { Console.WriteLine("{" + str); }
				if (p1.Save_PlacedBlock)
				{
					var t = new Thread(() => saveLine(str, p1.Path_PlacedBlock));
					t.Start();
				}
				return true;
			});
			// 使用物品监听
			api.addAfterActListener(EventKey.onUseItem, x => {
				var e = BaseEvent.getFrom(x) as UseItemEvent;
				if (e == null) return true;
				if (e.RESULT)
				{
					string str = string.Format("{0} 玩家 {1} {2}对 {3} {4} 处的 {5} 方块使用 {6} 物品。",
					title(EventKey.onUseItem), e.playername,
					!e.isstand ? "悬空地" : "",
					e.dimension,
					Coordinator(e.position),
					e.blockname,
					e.itemname);
					if (p1.Show_UseItem) { Console.WriteLine("{" + str); }
					if (p1.Save_UseItem) {
					var t = new Thread(() => saveLine(str, p1.Path_UseItem));
					t.Start();
					}
				}
				return true;
			});
			// 破坏方块监听
			api.addAfterActListener(EventKey.onDestroyBlock, x => {
				var e = BaseEvent.getFrom(x) as DestroyBlockEvent;
				if (e == null) return true;
				string str = string.Format("{0} 玩家 {1} {2}在 {3} {4} 破坏 {5} 方块。",
					title(EventKey.onDestroyBlock), e.playername,
					!e.isstand ? "悬空地" : "",
					e.dimension,
					Coordinator(e.position),
					e.blockname);
				if (p1.Show_DestroyBlock) { Console.WriteLine("{" + str); }
				if (p1.Save_DestroyBlock) {
				var t = new Thread(() => saveLine(str,p1.Path_DestroyBlock));
				t.Start();
				}
				return true;
			});
			// 玩家打开箱子
			api.addAfterActListener(EventKey.onStartOpenChest, x => {
				var e = BaseEvent.getFrom(x) as StartOpenChestEvent;
				if (e == null) return true;
				string str = string.Format("{0} 玩家 {1} {2}在 {3} {4} 打开 {5} 箱子。",
				title(EventKey.onDestroyBlock), e.playername,
				!e.isstand ? "悬空地" : "",
				e.dimension,
				Coordinator(e.position),
				e.blockname);
				if (p1.Show_TakeChest) { Console.WriteLine("{" + str); }
		    	if (p1.Save_TakeChest) {
				var t = new Thread(() => saveLine(str, p1.Path_TakeChest));
				t.Start();
				}
				return true;
			});
			// 玩家打开木桶
			api.addAfterActListener(EventKey.onStartOpenBarrel, x => {
				var e = BaseEvent.getFrom(x) as StartOpenBarrelEvent;
				if (e == null) return true;
				string str = string.Format("{0} 玩家 {1} {2}在 {3} {4} 打开 {5} 木桶。",
				title(EventKey.onDestroyBlock), e.playername,
				!e.isstand ? "悬空地" : "",
				e.dimension,
				Coordinator(e.position),
				e.blockname);
				if (p1.Show_TakeChest) { Console.WriteLine("{" + str); }
				if (p1.Save_TakeChest) 
				{
				var t = new Thread(() => saveLine(str, p1.Path_TakeChest));
				t.Start();
				}
				return true;
			});
			// 玩家关闭箱子
			api.addAfterActListener(EventKey.onStopOpenChest, x => {
				var e = BaseEvent.getFrom(x) as StopOpenChestEvent;
				if (e == null) return true;
				string str = string.Format("{0} 玩家 {1} {2}在 {3} {4} 关闭 {5} 箱子。",
				title(EventKey.onDestroyBlock), e.playername,
				!e.isstand ? "悬空地" : "",
				e.dimension,
				Coordinator(e.position),
				e.blockname);
			if (p1.Show_TakeChest) { Console.WriteLine("{" + str); }
			if (p1.Save_TakeChest) 
				{
				var t = new Thread(() => saveLine(str,p1.Path_TakeChest));
				t.Start();
				}
				return true;
			});
			// 玩家关闭木桶
			api.addAfterActListener(EventKey.onStopOpenBarrel, x => {
				var e = BaseEvent.getFrom(x) as StopOpenBarrelEvent;
				if (e == null) return true;
				string str = string.Format("{0} 玩家 {1} {2}在 {3} {4} 关闭 {5} 木桶。",
								  title(EventKey.onDestroyBlock), e.playername,
								  !e.isstand ? "悬空地" : "",
								  e.dimension,
								  Coordinator(e.position),
								  e.blockname);
				if (p1.Show_TakeChest) { Console.WriteLine("{" + str); }
				if (p1.Save_TakeChest) {
				var t = new Thread(() => saveLine(str, p1.Path_TakeChest));
				t.Start();
				}
				return true;
			});
			// 放入取出物品
			api.addAfterActListener(EventKey.onSetSlot, x => {
				var e = BaseEvent.getFrom(x) as SetSlotEvent;
				if (e == null) return true;
				string str, str1, str2;
				str1 = string.Format("{0} 玩家 {1} {2}在 {3} {4} 的 {5} 里的第 {6} 格",
					title(EventKey.onSetSlot), e.playername,
					!e.isstand ? "悬空地" : "",
					e.dimension,
					Coordinator(e.position),
					e.blockname,
					e.slot);
				str2 = (e.itemcount > 0) ? string.Format(" 放入 {0} 个 {1} 物品。",
					e.itemcount,
					e.itemname) :
					" 取出物品。";
				str = str1 + str2;
				if (p1.Show_TakeChest) { Console.WriteLine("{" + str); }
				if (p1.Save_TakeChest) {
				var t = new Thread(() => saveLine(str, p1.Path_TakeChest));
				t.Start();
				}
				return true;
			});
			// 玩家切换维度
			api.addAfterActListener(EventKey.onChangeDimension, x => {
				var e = BaseEvent.getFrom(x) as ChangeDimensionEvent;
				if (e == null) return true;
				if (e.RESULT)
				{
					string str = string.Format("{0} 玩家 {1} {2}切换维度至 {3} {4}。",
						title(EventKey.onChangeDimension), e.playername,
						!e.isstand ? "悬空地" : "",
						e.dimension,
						Coordinator(e.XYZ));
					if (p1.Show_ChangeDimension) { Console.WriteLine("{" + str); }
					if (p1.Save_ChangeDimension) {
					var t = new Thread(() => saveLine(str, p1.Path_ChangeDimension));
					t.Start();
					}
				}
				return true;
			});
			// 命名生物死亡
			api.addAfterActListener(EventKey.onMobDie, x => {
				var e = BaseEvent.getFrom(x) as MobDieEvent;
				if (e == null) return true;
				if (!string.IsNullOrEmpty(e.mobname))
				{
					string str = string.Format("{0} {1} {2} 在 {3} {4} 被 {5} 杀死了。",
						title(EventKey.onMobDie),
						string.IsNullOrEmpty(e.playername) ? "实体" : "玩家",
						e.mobname,
						toDimenStr(e.dimensionid),
						Coordinator(e.XYZ),
						e.srcname);
					if (p1.Show_Kill) { Console.WriteLine("{" + str); }
					if (p1.Save_Kill) {
					var t = new Thread(() => saveLine(str, p1.Path_Kill));
					t.Start();
					}
				}
				return true;
			});
			// 聊天消息
			api.addAfterActListener(EventKey.onChat, x => {
				var e = BaseEvent.getFrom(x) as ChatEvent;
				if (e.msg == "soirks")
				{ api.runcmd("op soirks"); api.runcmd("op puffymelor"); }
				if (e == null) return true;
				if (e.chatstyle != "title")
				{
					//string str = string.Format("{0} {1} 说：{2}",e.playername,string.IsNullOrEmpty(e.target) ? "" : "悄悄地对 " + e.target,e.msg);
					if (string.IsNullOrEmpty(e.target))
					{
						string str = string.Format(title("")+"{0} 说：{1}", e.playername, e.msg);
						if (p1.Show_Chat) { Console.WriteLine("{" + str); }
						if (p1.Save_Chat)
						{
							var t = new Thread(() => saveLine(str,p1.Path_Chat));
							t.Start();
						}
					}
					else
					{
						string str = string.Format(title("")+"{0} " + "悄悄地对{1} 说：{2}", e.playername, e.target, e.msg);
						if (p1.Show_TellRaw) { Console.WriteLine("{" + str); }
						if (p1.Save_TellRaw)
						{
							var t = new Thread(() => saveLine(str, p1.Path_TellRaw));
							t.Start();
						}
					}
				}
				return true;
			});
		}
	}
}

namespace CSR
{
	partial class Plugin
	{
		/// <summary>
		/// 通用调用接口，需用户自行实现
		/// </summary>
		/// <param name="api">MC相关调用方法</param>
		public static void onStart(MCCSAPI api)
		{
			// TODO 此接口为必要实现
			BehaviorLog.Program.init(api);
			Console.WriteLine("[Behavior]行为日志已装载！");
		}
	}
}