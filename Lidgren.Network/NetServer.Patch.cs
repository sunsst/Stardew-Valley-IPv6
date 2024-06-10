using System.Net;

namespace Lidgren.Network
{
    public partial class NetServer
    {
        /// <summary>
        /// Binds to socket and spawns the networking thread
        /// </summary>
        public override void Start()
        {
            if (MyPatch.ok)
            {
                MyPatch.LogInfo($"call {GetType().FullName}.Start() ");
                if (MyPatch.CallServerStarCheck() && Status == NetPeerStatus.NotRunning)
                {
                    MyPatch.LogInfo($"set {GetType().FullName}.m_configuration ");
                    switch (MyPatch.ChooseIPMode())
                    {
                        case MyPatch.IPMode.IPv4:
                            MyPatch.GameLog("server enable IPv4");
                            MyPatch.addHUDMessage("服务器仅支持 IPv4 客户端连接");
                            break;
                        case MyPatch.IPMode.IPv6:
                            m_configuration.LocalAddress = IPAddress.IPv6Any;
                            MyPatch.GameLog("server enable IPv6");
                            MyPatch.addHUDMessage("服务器仅支持 IPv6 客户端连接");
                            break;
                        default:
                        case MyPatch.IPMode.IPv4IPv6:
                            m_configuration.DualStack = true;
                            m_configuration.LocalAddress = IPAddress.IPv6Any;
                            MyPatch.GameLog("server enable IPv4 and IPv6");
                            MyPatch.addHUDMessage("服务器支持 IPv4 与 IPv6 客户端连接");
                            break;
                    }
                }
            }
            base.Start();
        }
    }
}
