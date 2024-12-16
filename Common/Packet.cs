using System.Runtime.InteropServices;

// 마샬링을 사용시 Unmanaged Memory로 옮길때 순서가 유지되게 한다. Pack은 구조체, 클래스 크기를 정확하게 만들어 준다.
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class Packet
{
    // 헤더 부분 [패킷크기:2Byte][프로토콜ID:2Byte]
    public short size;
    public short protocolID;

    public Packet()
    {
        size = (short)Marshal.SizeOf(this);
    }

    /// <summary>
    /// 현재 패킷을 바이트 배열로 변환
    /// </summary>
    /// <returns></returns>
    public byte[] ToByte()
    {
        // 사용할 바이트 배열
        byte[] buffer = new byte[Marshal.SizeOf(this)];
        // 안전하지 않은 작업을 시작할때 unsafe 사용, C#에서 포인터를 사용할때 라던가...
        // 빌드설정에서 안전하지 않은코드 체크
        unsafe
        {
            // 메모리를 고정시킨다. 가비지컬렉터등이 메모리를 옮기지 못하게
            fixed (byte* ptr = buffer)
            {
                // 해당 구조체, 클래스를 옮겨준다. 관리되는 개체의 데이터를 관리되지 않는 메모리 블럭으로 마샬링
                Marshal.StructureToPtr(this, (IntPtr)ptr, false);
            }
        }
        return buffer;
    }

    /// <summary>
    /// 바이트 배열을 받아 패킷형태로 변환
    /// </summary>
    /// <param name="buffer"></param>
    public void ToPacket(byte[] buffer)
    {
        unsafe
        {
            fixed (byte* ptr = buffer)
            {
                // 해당 메모리를 구조체, 클래스로 옮겨준다.
                Marshal.PtrToStructure((IntPtr)ptr, this);
            }
        }
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class ChatMessage : Packet
{
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)]
    public string id;
    // 문자열의 크기를 정한다.
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100)]
    public string message;
}