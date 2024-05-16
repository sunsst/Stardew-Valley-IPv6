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
                            MyPatch.PlaySound("cat");
                            MyPatch.GameLog("server enable IPv4");
                            break;
                        case MyPatch.IPMode.IPv6:
                            m_configuration.LocalAddress = IPAddress.IPv6Any;
                            MyPatch.PlaySound("dog_bark");
                            MyPatch.GameLog("server enable IPv6");
                            break;
                        default:
                        case MyPatch.IPMode.IPv4IPv6:
                            m_configuration.DualStack = true;
                            m_configuration.LocalAddress = IPAddress.IPv6Any;
                            MyPatch.PlaySound("cat");
                            MyPatch.PlaySound("dog_bark");
                            MyPatch.GameLog("server enable IPv4 and IPv6");
                            break;
                    }
                }
            }
            base.Start();
        }
    }
}
