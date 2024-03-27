using System.Net.Sockets;
using System.Text;

namespace Common.Network
{
    /// <summary>
    /// 这是Socket异步接收器，可以对接收的数据粘包与拆包
    /// 事件委托：
    ///     -- DataReceived 数据包接收完成事件，参数为接收的数据包
    ///     -- Disconnected 接收异常事件
    /// </summary>
    public class LengthFieldDecoder
    {

        private Socket mSocket;

        private int lengthFieldOffset = 8;  //第几个是body长度字段
        private int lengthFieldLength = 4; //长度字段占几个字节

        /// <summary>
        //长度字段和内容之间距离几个字节，
        //负数代表向前偏移，body实际长度要减去这个绝对值
        /// </summary>
        private int lengthAdjustment = 0;
        private int initialBytesToStrip = 0;//结果数据中前几个字节不需要的字节数

        private byte[] mBuffer;   //接收数据的缓存空间
        private int mOffect;    //读取位置
        /// <summary>
        ///	一次性接收数据的最大字节，默认1Mb
        /// </summary>
        private int mSize = 1024 * 1024;

        //成功收到消息的委托事件
        public delegate void OnReceived(byte[] data);
        public event OnReceived DataReceived;

        //连接失败的委托事件
        public delegate void OnDisconnectedEventHandler(Socket soc);
        public event OnDisconnectedEventHandler Disconnected;


        public LengthFieldDecoder(Socket socket, int lengthFieldOffset, int lengthFieldLength)
        {
            mSocket = socket;
            this.lengthFieldOffset = lengthFieldOffset;
            this.lengthFieldLength = lengthFieldLength;
            mBuffer = new byte[mSize];
        }

        public LengthFieldDecoder(Socket socket, int maxBufferLength, int lengthFieldOffset, int lengthFieldLength,
            int lengthAdjustment, int initialBytesToStrip)
        {
            mSocket = socket;
            mSize = maxBufferLength;
            this.lengthFieldOffset = lengthFieldOffset;
            this.lengthFieldLength = lengthFieldLength;
            this.lengthAdjustment = lengthAdjustment;
            this.initialBytesToStrip = initialBytesToStrip;
            mBuffer = new byte[mSize];
        }


        public void Start()
        {
            BeginAsyncReceive();
        }

        public void BeginAsyncReceive()
        {
            //Debug.Log("开始接收");
            mSocket.BeginReceive(mBuffer, mOffect, mSize - mOffect, SocketFlags.None, new AsyncCallback(Receive), null);
        }

        public void Receive(IAsyncResult result)
        {
            try{
                int len = mSocket.EndReceive(result);
                // 0代表连接失败
                if (len == 0)
                {
                    _disconnected();
                    return;
                }

                //headLen+bodyLen=totalLen
                int headLen = lengthFieldOffset + lengthFieldLength;
                int adj = lengthAdjustment; //body偏移量

                //size是待处理的数据长度，mOffect每次都从0开始，
                //循环开始之前mOffect代表上次剩余长度
                int size = len;
                if (mOffect > 0)
                {
                    size += mOffect;
                    mOffect = 0;
                }
                //循环解析
                while (true)
                {
                    int remain = size - mOffect;//剩余未处理的长度
                    //Debug.Log("剩余未处理的长度：" + remain);
                    //Debug.Log("remain=" + remain);
                    //如果未处理的数据超出限制
                    if (remain > mSize)
                    {
                        throw new IndexOutOfRangeException("数据超出限制");
                        //mOffect = 0;
                        //BeginAsyncReceive();
                        //return;
                    }
                    if (remain < headLen)
                    {
                        //接收的数据不够一个完整的包，继续接收
                        Array.Copy(mBuffer, mOffect, mBuffer, 0, remain);
                        mOffect = remain;
                        BeginAsyncReceive();
                        return;
                    }

                    //获取包长度
                    int bodyLen = BitConverter.ToInt32(mBuffer, mOffect + lengthFieldOffset);
                    if (remain < headLen + adj + bodyLen)
                    {
                        //接收的数据不够一个完整的包，继续接收
                        Array.Copy(mBuffer, mOffect, mBuffer, 0, remain);
                        mOffect = remain;
                        BeginAsyncReceive();
                        return;
                    }

                    //body的读取位置
                    int bodyStart = mOffect + Math.Max(headLen, headLen + adj);
                    //body的真实长度
                    int bodyCount = Math.Min(bodyLen, bodyLen + adj);
                    //Debug.Log("bodyStart=" + bodyStart + ", bodyCount=" + bodyCount+ ", remain=" + remain);

                    //获取包体
                    int total = headLen + adj + bodyLen; //数据包总长度
                    int count = total - initialBytesToStrip;
                    byte[] data = new byte[count];
                    Array.Copy(mBuffer, mOffect + initialBytesToStrip, data, 0, count);
                    mOffect += total;

                    //完成一个数据包
                    DataReceived?.Invoke(data);
                    //Debug.Log("完成一个数据包");
                }

            }
            catch (SocketException)
            {
                _disconnected();
            }
            catch (ObjectDisposedException)
            {
                _disconnected();
            }
            
        }

        private void _disconnected()
        {
            Disconnected?.Invoke(mSocket);
        }
    }
}
