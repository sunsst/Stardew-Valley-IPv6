using HarmonyLib;
using StardewModdingAPI;
using System.Reflection;
using System.Reflection.Emit;

namespace IPv6.Patch
{
    internal partial class MyPatch
    {
        public static void LogInfo(string msg)
        {
#if DEBUG
            FileLog.Log(msg);
#endif
        }

        public static void GameLog(string msg)
        {
#if DEBUG
            FileLog.Log(msg);
#endif
            log.Info(msg);
        }

        public static void CheckSomeTypes(this CodeInstruction code)
        {
#if DEBUG
            var codestr = code.ToString();
            foreach (var t in new Type[] {
            typeof(StardewValley.Network.LidgrenClient),
            typeof(StardewValley.Network.LidgrenServer),
        })
            {
                if (codestr.Contains(t.FullName!))
                {
                    LogInfo($"! check type | {t.FullName} in {codestr}");
                }
            }
#endif
        }

        public static void PatchingLoadFunction(this CodeInstruction code, HarmonyMethod transpiler)
        {
            if (code.opcode == OpCodes.Ldftn)
            {
                var m = (MethodBase)code.operand;
                LogInfo($"! load function | {m.ReflectedType!.FullName}:{m.Name}");
                Harmony.Patch(m, transpiler: transpiler);
            }
        }


        public static void Patching(IModHelper helper , string harmonyID)
        {
            InitRefValues(helper, harmonyID);
            //Harmony.DEBUG = true;

            // 该类只在Android上有效
            var menuType = AccessTools.TypeByName("StardewValley.Menus.CoopGameMenu");

            if (menuType == null)
            {
                GameLog("IPv6: Start on PC");
                menuType = AccessTools.TypeByName("StardewValley.Menus.CoopMenu");
                MyPatch.PatchingGame(helper, harmonyID, menuType);
            }
            else
            {
                GameLog("IPv6: Start on Android");
                MyPatch.PatchingAndroid(helper, harmonyID, menuType);
            }
            
        }


        public static void PatchingGame(IModHelper helper, string harmonyID, Type menuType)
        {
            // 该方法用于为PC版本SMAPI添加补丁

            foreach (var m in new MethodBase[] {
           AccessTools.Method(typeof(StardewValley.Game1), "UpdateTitleScreen"),
           AccessTools.Method(menuType, "enterIPPressed"),
           AccessTools.Method(typeof(StardewValley.Multiplayer), "LogDisconnect")})
            {
                Harmony.Patch(m, transpiler: ClientTranspiler);
            }

            Harmony.Patch(AccessTools.Constructor(typeof(StardewValley.Network.GameServer), new Type[] { typeof(bool) }), transpiler: ServerTranspiler);

            Harmony.Patch(AccessTools.Method(typeof(StardewValley.Network.GameServer), "UpdateLocalOnlyFlag"), transpiler: UpdateLocalOnlyFlagTranspiler);
        }


        private static void TitleTextInputMenu_Postfix(StardewValley.Menus.TitleTextInputMenu __instance, string title, StardewValley.Menus.NamingMenu.doneNamingBehavior b, string default_text, string context, bool filterInput)
        {
            __instance.textBox.textLimit = 100;
        }
        public static void PatchingAndroid(IModHelper helper, string harmonyID, Type menuType)
        {
            // 该方法用于为Android版本SMAPI添加补丁

            foreach (var m in new MethodBase[] {
           AccessTools.Method(typeof(StardewValley.Game1), "UpdateTitleScreen"),
           AccessTools.Method(menuType, "enterIPPressed"),
           AccessTools.Method(typeof(StardewValley.Multiplayer), "LogDisconnect")})
            {
                Harmony.Patch(m, transpiler: ClientTranspiler);
            }

            var original = AccessTools.Constructor(typeof(StardewValley.Menus.TitleTextInputMenu), new Type[] { typeof(string), typeof(StardewValley.Menus.NamingMenu.doneNamingBehavior), typeof(string), typeof(string), typeof(bool) });
            var postfix = typeof(MyPatch).GetMethod(nameof(TitleTextInputMenu_Postfix), BindingFlags.Static | BindingFlags.NonPublic);
            harmony.Patch(original, postfix: new HarmonyMethod(postfix));


            Harmony.Patch(AccessTools.Constructor(typeof(StardewValley.Network.GameServer), new Type[] { typeof(bool) }), transpiler: ServerTranspiler);

            Harmony.Patch(AccessTools.Method(typeof(StardewValley.Network.GameServer), "UpdateLocalOnlyFlag"), transpiler: UpdateLocalOnlyFlagTranspiler);
        }

    }
}
