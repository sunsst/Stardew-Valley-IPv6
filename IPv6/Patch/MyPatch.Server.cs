using HarmonyLib;
using System.Reflection;
using System.Reflection.Emit;

namespace IPv6.Patch;

internal static partial class MyPatch
{
    public static HarmonyMethod ServerTranspiler = new HarmonyMethod(typeof(MyPatch), "ServerTranspilerMethod");

        public static IEnumerable<CodeInstruction> ServerTranspilerMethod(IEnumerable<CodeInstruction> instructions, MethodBase __originalMethod)
    {
        LogInfo($"\n========== server | {__originalMethod.DeclaringType}:{__originalMethod.Name}() ==========");

        var t = typeof(StardewValley.Network.LidgrenServer);
        var c = AccessTools.Constructor(t, new Type[] { typeof(StardewValley.Network.IGameServer) });
        foreach (CodeInstruction code in instructions)
        {
            if (code.Is(OpCodes.Isinst, t))
            {
                LogInfo($"< server | {code}");
                code.operand = typeof(Classes.LidgrenServer);
                LogInfo($"> server | {code}");
            }
            else if (code.Is(OpCodes.Newobj, c))
            {
                LogInfo($"< server | {code}");
                code.operand = AccessTools.Constructor(typeof(Classes.LidgrenServer), new Type[] { typeof(StardewValley.Network.IGameServer) });
                LogInfo($"> server | {code}");
            }

            code.CheckSomeTypes();
            code.PatchingLoadFunction(ServerTranspiler);

            yield return code;
        }
    }
}
