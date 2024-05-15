namespace Lidgren.Network
{
    public partial class NetClient
    {
        /// <summary>
		/// Binds to socket and spawns the networking thread
		/// </summary>
		public override void Start()
        {
            if (!MyPatch.ok || !MyPatch.CallClientStarCheck())
                base.Start();
        }
    }
}
