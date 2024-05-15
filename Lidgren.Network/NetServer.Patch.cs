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
            if (MyPatch.ok
                && MyPatch.CallServerStarCheck()
                && Status == NetPeerStatus.NotRunning)
            {
                switch (MyPatch.ChooseIPMode())
                {
                    case MyPatch.IPMode.IPv4:
                        MyPatch.LogInfo("server enable IPv4");
                        MyPatch.PlaySound("cat");
                        break;
                    case MyPatch.IPMode.IPv6:
                        m_configuration.LocalAddress = IPAddress.IPv6Any;
                        MyPatch.LogInfo("server enable IPv6");
                        MyPatch.PlaySound("dog_bark");
                        break;
                    default:
                    case MyPatch.IPMode.IPv4IPv6:
                        m_configuration.DualStack = true;
                        m_configuration.LocalAddress = IPAddress.IPv6Any;
                        MyPatch.LogInfo("server enable IPv4 and IPv6");
                        MyPatch.PlaySound("cat");
                        MyPatch.PlaySound("dog_bark");
                        break;
                }
            }
            base.Start();
        }
    }
}
