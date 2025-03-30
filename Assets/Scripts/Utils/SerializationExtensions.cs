using Unity.Netcode;

namespace ChessMonsterTactics
{
    internal static class SerializationExtensions
    {
        public static void WriteValueSafe(this FastBufferWriter writer, in UserProfile profile)
        {
            string json = SerializationUtility.SerializeToJson(profile);
            writer.WriteValueSafe(json);
        }

        public static void ReadValueSafe(this FastBufferReader reader, out UserProfile profile)
        {
            reader.ReadValueSafe(out string json);
            profile = SerializationUtility.DeserializeFromJson<UserProfile>(json);
        }

        public static void WriteValueSafe(this FastBufferWriter writer, in TeamConfig teamConfig)
        {
            writer.WriteValueSafe(teamConfig.Pieces);
        }

        public static void ReadValueSafe(this FastBufferReader reader, out TeamConfig teamConfig)
        {
            reader.ReadValueSafe(out ushort[] pieces);
            teamConfig = new TeamConfig(pieces.Length);
            for (int i = 0; i < pieces.Length; i++)
            {
                teamConfig.Pieces[i] = pieces[i];
            }
        }
    }
}
