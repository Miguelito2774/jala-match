namespace SharedKernel;

public static class Utilities
{
    public static class StreamHelper
    {
        public static Stream Base64ToStream(string base64Data)
        {
            int comma = base64Data.IndexOf(',');
            string payload = comma >= 0 ? base64Data[(comma + 1)..] : base64Data;

            byte[] bytes = Convert.FromBase64String(payload);
            return new MemoryStream(bytes);
        }
    }
}
