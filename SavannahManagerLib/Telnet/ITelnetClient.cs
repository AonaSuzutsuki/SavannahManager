using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SvManagerLibrary.Telnet
{
    public interface ITelnetClient
    {
        /// <summary>
        /// Get a state of connection
        /// </summary>
        bool Connected { get; }

        /// <summary>
        /// Get or Set Sending and Receiving text encoding.
        /// </summary>
        Encoding Encoding { get; set; }

        /// <summary>
        /// Get or Set Receiving Buffer Size.
        /// </summary>
        int ReceiveBufferSize { get; set; }

        /// <summary>
        /// Get or Set Receiving Time Out Time (millisecond).
        /// </summary>
        int ReceiveTimeout { get; set; }

        /// <summary>
        /// This event occurs when the Telnet connection is terminated.
        /// </summary>
        event TelnetClient.TelnetReadEventHandler Finished;

        /// <summary>
        /// This event occurs when the log is read by the telnet.
        /// </summary>
        event TelnetClient.TelnetReadEventHandler ReadEvent;

        /// <summary>
        /// This event occurs when the Telnet connection is started.
        /// </summary>
        event TelnetClient.TelnetReadEventHandler Started;

        /// <summary>
        /// Connect to address and port.
        /// </summary>
        /// <param name="address">The address to connect.</param>
        /// <param name="port">The port to connect.</param>
        /// <returns>Whether the connection was established or not.</returns>
        bool Connect(string address, int port);

        /// <summary>
        /// Release the telnet connection and resources.
        /// </summary>
        void Dispose();

        /// <summary>
        /// Read log from the connection.
        /// </summary>
        /// <returns>The read log.</returns>
        string Read();

        /// <summary>
        /// Send command and suppresses the occurrence of events and reads the log.
        /// </summary>
        /// <param name="cmd">The command to send</param>
        /// <returns>The read log.</returns>
        string DestructionEventRead(string cmd);

        /// <summary>
        /// Send command and suppresses the occurrence of events and reads the log.
        /// Detect the end by regular expression in the last read line.
        /// </summary>
        /// <param name="cmd">The command to send</param>
        /// <param name="expressionForLast">A regular expression indicating the end of reading.</param>
        /// <returns>The read log.</returns>
        string DestructionEventRead(string cmd, string expressionForLast);

        /// <summary>
        /// Get the time required to communicate with the destination.
        /// </summary>
        /// <param name="maxMilliseconds">Timeout in milliseconds</param>
        /// <returns>The time required to communicate with the destination</returns>
        int CalculateWaitTime(int maxMilliseconds = 10000);

        /// <summary>
        /// Send the data to connected server.
        /// </summary>
        /// <param name="data">Sent byte array</param>
        /// <returns>Length of sent byte</returns>
        int Write(byte[] data);

        /// <summary>
        /// Send the string to connected server.
        /// </summary>
        /// <param name="cmd">Sent string</param>
        /// <returns>Length of sent byte</returns>
        int Write(string cmd);

        /// <summary>
        /// Send the data to connected server and send linebreak.
        /// </summary>
        /// <param name="data">Sent byte array</param>
        /// <returns>Length of sent byte</returns>
        int WriteLine(byte[] data);

        /// <summary>
        /// Send the string to connected server and send linebreak.
        /// </summary>
        /// <param name="cmd">Sent string</param>
        /// <returns>Length of sent byte</returns>
        int WriteLine(string cmd);
    }
}