namespace Gadema.Core.Utils;

public static class ReservedGuid
{
    // The bit-shifting logic remains the same
    public static Guid Create(int moduleIndex, int serial)
    {
        byte[] bytes = new byte[16];
        bytes[8] = (byte)((moduleIndex >> 8) & 0xFF);
        bytes[9] = (byte)(moduleIndex & 0xFF);
        bytes[12] = (byte)((serial >> 24) & 0xFF);
        bytes[13] = (byte)((serial >> 16) & 0xFF);
        bytes[14] = (byte)((serial >> 8) & 0xFF);
        bytes[15] = (byte)(serial & 0xFF);
        return new Guid(bytes);
    }
}