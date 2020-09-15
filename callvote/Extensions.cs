namespace callvote
{
	public static class Extensions
	{
		//These are two commonly used extensions that will make your life considerably easier
		//When sending RaReply's, you need to identify the 'source' of the message with a string followed by '#' at the start of the message, otherwise the message will not be sent
		public static void RAMessage(this CommandSender sender, string message, bool success = true) =>
			sender.RaReply("Sample Plugin#" + message, success, true, string.Empty);

		public static void Broadcast(this ReferenceHub rh, ushort time, string message) => rh.GetComponent<Broadcast>().TargetAddElement(rh.scp079PlayerScript.connectionToClient, message, time, global::Broadcast.BroadcastFlags.Normal);
	}
}