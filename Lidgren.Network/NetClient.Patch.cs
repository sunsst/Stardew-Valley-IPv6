namespace Lidgren.Network
{
    public partial class NetClient
    {
        /// <summary>
		/// Binds to socket and spawns the networking thread
		/// </summary>
		public override void Start()
        {
            if (MyPatch.ok)
            {
                if (MyPatch.CallClientStarCheck())
                {
                    MyPatch.LogInfo($"cancel call {GetType().FullName}.Start() ");
                    return;
                }
                MyPatch.LogInfo($"call {GetType().FullName}.Start() ");
            }
            base.Start();
        }
    }
}
