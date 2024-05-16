using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Lidgren.Network
{
    internal static class MyPatch
    {
        private static readonly (object log, MethodInfo InfoMethod) log;

#if DEBUG
        private static readonly MethodInfo harmonylog;
#endif

        private static readonly MethodInfo playSound;
        private static readonly (object input, MethodInfo GetKeyboardStateMethod, MethodInfo IsKeyDownMethod, object D4, object D6) input;
        public static readonly (FieldInfo address, FieldInfo lastAttemptMs, FieldInfo client) clientFiled;

        public static readonly bool ok;

        static MyPatch()
        {
            try
            {
                Assembly svAssembly = Assembly.Load("Stardew Valley");
                Assembly hAssembly;
                try { hAssembly = Assembly.Load("0Harmony"); }
                catch { hAssembly = Assembly.Load(Resource1._0Harmony); }

                Type Game1Type = svAssembly.GetType("StardewValley.Game1")
                    ?? throw new Exception("Class StardewValley.Game1 error");

                // 日志接口
#if DEBUG
                Type FileLogType = hAssembly.GetType("HarmonyLib.FileLog")
                    ?? throw new Exception("Class HarmonyLib.FileLog error");
                harmonylog = FileLogType.GetMethod("Log", BindingFlags.Static | BindingFlags.Public, [typeof(string)])
                    ?? throw new Exception($"Method {FileLogType.FullName}.Log() error");
#endif

                log.log = Game1Type.GetField("log", BindingFlags.Static | BindingFlags.NonPublic)
                    ?.GetValue(null)!
                    ?? throw new Exception($"Field {Game1Type.FullName}.log error");

                log.InfoMethod = log.log.GetType()
                    ?.GetMethod("Info", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, [typeof(string)])
                    ?? throw new Exception($"Method {Game1Type.FullName}.log.Info() error");

                // 输入接口
                input.input = Game1Type.GetField("input", BindingFlags.Static | BindingFlags.Public)
                    ?.GetValue(null)
                    ?? throw new Exception($"Field {Game1Type.FullName}.input error");

                input.GetKeyboardStateMethod = input.input.GetType()
                    ?.GetMethod("GetKeyboardState", BindingFlags.Instance | BindingFlags.Public, [])
                    ?? throw new Exception($"Method {Game1Type.FullName}.input.GetKeyboardState() error");

                Type KeysType;
                input.IsKeyDownMethod = input.GetKeyboardStateMethod.ReturnType
                      ?.GetMethod("IsKeyDown", BindingFlags.Instance | BindingFlags.Public)!;
                if (input.IsKeyDownMethod == null
                    || input.IsKeyDownMethod.GetParameters().Length != 1
                    || (KeysType = input.IsKeyDownMethod.GetParameters()[0].ParameterType).FullName != "Microsoft.Xna.Framework.Input.Keys"
                    || input.IsKeyDownMethod?.ReturnType?.FullName != typeof(bool).FullName)
                    throw new Exception($"Method {Game1Type.FullName}.input.GetKeyboardState().IsKeyDown() error");

                input.D4 = KeysType.GetField("D4", BindingFlags.Static | BindingFlags.Public)
                   ?.GetValue(null)
                   ?? throw new Exception($"Field Keys.D4 error");

                input.D6 = KeysType.GetField("D6", BindingFlags.Static | BindingFlags.Public)
                   ?.GetValue(null)
                   ?? throw new Exception($"Field Keys.D6 error");

                // 音效播放接口
                playSound = Game1Type.GetMethod("playSound", BindingFlags.Static | BindingFlags.Public, [typeof(string), typeof(int?)])
                    ?? throw new Exception($"Method {Game1Type.FullName}.playSound() error");

                // 客户端接口
                Type LidgrenClientType = svAssembly.GetType("StardewValley.Network.LidgrenClient")
                    ?? throw new Exception("Class StardewValley.Network.LidgrenClient error");

                clientFiled.address = LidgrenClientType.GetField("address", BindingFlags.Instance | BindingFlags.Public)!;
                if (clientFiled.address == null || clientFiled.address.FieldType != typeof(string))
                    throw new Exception($"Field {LidgrenClientType.FullName}.address error");

                clientFiled.client = LidgrenClientType.GetField("client", BindingFlags.Instance | BindingFlags.Public)!;
                if (clientFiled.client == null || clientFiled.client.FieldType != typeof(NetClient))
                    throw new Exception($"Field {LidgrenClientType.FullName}.client error");

                clientFiled.lastAttemptMs = LidgrenClientType.GetField("lastAttemptMs", BindingFlags.Instance | BindingFlags.NonPublic)!;
                if (clientFiled.lastAttemptMs == null || clientFiled.lastAttemptMs.FieldType != typeof(double))
                    throw new Exception($"Field {LidgrenClientType.FullName}.lastAttemptMs error");

                if (LidgrenClientType.GetMethod("connectImpl", BindingFlags.Instance | BindingFlags.NonPublic, []) == null)
                    throw new Exception($"Method {LidgrenClientType.FullName}.connectImpl() error");

                var attemptConnection = LidgrenClientType.GetMethod("attemptConnection", BindingFlags.Instance | BindingFlags.NonPublic, [])
                    ?? throw new Exception($"Method {LidgrenClientType.FullName}.attemptConnection() error");


                // 服务端接口
                Type LidgrenServerType = svAssembly.GetType("StardewValley.Network.LidgrenServer")
                    ?? throw new Exception("Class StardewValley.Network.LidgrenServer error");

                if (LidgrenServerType.GetMethod("initialize", BindingFlags.Instance | BindingFlags.Public, []) == null)
                    throw new Exception($"Method {LidgrenServerType.FullName}.initialize() error");

                // 修复地址解析过程
                Type HarmonyType = hAssembly.GetType("HarmonyLib.Harmony")
                    ?? throw new Exception("Class HarmonyLib.Harmony error");
                Type HarmonyMethodType = hAssembly.GetType("HarmonyLib.HarmonyMethod")
                    ?? throw new Exception("Class HarmonyLib.HarmonyMethod error");
                var hm = Activator.CreateInstance(HarmonyMethodType, [typeof(MyPatch).GetMethod("attemptConnection", BindingFlags.Static | BindingFlags.NonPublic)
                    ?? throw new Exception($"Method attemptConnection path not found")])
                    ?? throw new Exception($"Create {HarmonyMethodType.FullName} error");
                var h = Activator.CreateInstance(HarmonyType, ["Stardew Vally IPv6 Patch"])
                    ?? throw new Exception($"Create {HarmonyType.FullName} error");
                var pm = HarmonyType.GetMethod("Patch", BindingFlags.Instance | BindingFlags.Public, [typeof(MethodBase), HarmonyMethodType, HarmonyMethodType, HarmonyMethodType, HarmonyMethodType])
                    ?? throw new Exception($"Method {HarmonyType.FullName}.Patch error");
                pm.Invoke(h, [attemptConnection, hm, null, null, null]);

                ok = true;
                LogInfo("patch ok!");
            }
#if DEBUG
            catch (Exception e)
            {
                LogInfo(e.ToString());
                playSound = null!;
                harmonylog = null!;
            }
#else
            catch
            {
                playSound = null!;
            }
#endif
        }

        public static void GameLog(string msg)
        {
            log.InfoMethod.Invoke(log.log, [msg]);
#if DEBUG
            harmonylog.Invoke(null, [msg]);
#endif
        }

        public static void LogInfo(string msg)
        {
#if DEBUG
            if (ok)
                harmonylog.Invoke(null, [msg]);
            else
                System.IO.File.AppendAllText("_log.txt", msg);
#endif
        }


        public static bool CallClientStarCheck() => (new StackTrace()).GetFrame(2)?.GetMethod()?.Name == "connectImpl";
        public static bool CallServerStarCheck() => (new StackTrace()).GetFrame(2)?.GetMethod()?.Name == "initialize";

        public enum IPMode { IPv4, IPv6, IPv4IPv6 };
        public static IPMode ChooseIPMode()
        {
            var keystate = input.GetKeyboardStateMethod.Invoke(input.input, []);
            bool d4 = (bool)input.IsKeyDownMethod.Invoke(keystate, [input.D4])!;
            bool d6 = (bool)input.IsKeyDownMethod.Invoke(keystate, [input.D6])!;

            if (d4 && d6)
                return IPMode.IPv4IPv6;
            else if (d4)
                return IPMode.IPv4;
            else if (d6)
                return IPMode.IPv6;
            else if (Socket.OSSupportsIPv4 && !Socket.OSSupportsIPv6)
                return IPMode.IPv4;
            else if (!Socket.OSSupportsIPv4 && Socket.OSSupportsIPv6)
                return IPMode.IPv6;
            else
                return IPMode.IPv4IPv6;
        }

        public static void PlaySound(string cueName)
        {
            playSound.Invoke(null, [cueName, null]);
        }

        private static bool attemptConnection(object __instance)
        {
            if (ok)
            {
                LogInfo("method attemptConnection patched");
            }
            else
            {
                LogInfo($"method attemptConnection patched, but ok={false}");
                return true;
            }

            int port = 24642;
            var address = (string)clientFiled.address.GetValue(__instance)!;
            var client = (NetClient)clientFiled.client.GetValue(__instance)!;

            if (!IPEndPoint.TryParse(address, out var addr))
            {
                var res = Regex.Match(address, @"^(.+?)(?:\:([0-9]+))?$");
                if (res.Success)
                {
                    try
                    {
                        addr = NetUtility.Resolve(res.Groups[1].Value,
                        res.Groups[2].Value != "" ? int.Parse(res.Groups[2].Value) : 0);
                    }
                    catch { }
                    if (addr != null)
                    {
                        GameLog($"{address} => {addr}");
                        address = addr.ToString();
                        clientFiled.address.SetValue(__instance, address);
                    }
                }
            }

            if (client.Status == NetPeerStatus.NotRunning)
            {
                if (addr != null && addr.AddressFamily == AddressFamily.InterNetworkV6)
                {
                    client.m_configuration.LocalAddress = IPAddress.IPv6Any;
                    GameLog($"client enable IPv6");
                }
                else
                {
                    GameLog($"client enable IPv4");
                }
                client.Start();
            }

            if (addr != null)
            {
                if (addr.Port != 0)
                    port = addr.Port;
                address= addr.Address.ToString();
                clientFiled.address.SetValue(__instance, address);
            }

            GameLog($"client target address {address} port {port}");

            client.DiscoverKnownPeer(address, port);
            clientFiled.lastAttemptMs.SetValue(__instance, DateTime.UtcNow.TimeOfDay.TotalMilliseconds);

            return false;
        }

    }
}
