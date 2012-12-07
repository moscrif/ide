using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Threading;
using Moscrif.IDE.Execution; 

namespace Moscrif.IDE.Iface
{
	public class SocketServer
	{
		public delegate void UpdateRichEditCallback(string text);
		public static event ProcessEventHandler OutputClientChanged;

		private static string m_currentmessage = "";

		public static AsyncCallback pfnWorkerCallBack ;
		private static System.Net.Sockets.Socket m_mainSocket;

		private static System.Collections.ArrayList m_workerSocketList = 
				ArrayList.Synchronized(new System.Collections.ArrayList());

		private static int m_clientCount = 0;

		private static bool isRunning;

		public static string StartListen(string IPadress,string portStr){
			try
			{

				IPAddress ipAddress = IPAddress.Parse(IPadress);

				int port = System.Convert.ToInt32(portStr);

				m_mainSocket = new  System.Net.Sockets.Socket(AddressFamily.InterNetwork,SocketType.Stream, 
					ProtocolType.Tcp);
				IPEndPoint ipLocal = new IPEndPoint(ipAddress,port); //(IPAddress.Any, port);

				m_mainSocket.Bind( ipLocal );

				m_mainSocket.Listen(4);

				m_mainSocket.BeginAccept(new AsyncCallback (OnClientConnect), null);
				isRunning = true;
				return ipLocal.ToString();

			}
			catch(SocketException se)
			{
				Console.WriteLine( se.Message );
				return null;
			}
		}


		public static void OnClientConnect(IAsyncResult asyn)
		{
			try
			{
				System.Net.Sockets.Socket workerSocket = m_mainSocket.EndAccept (asyn);
				Interlocked.Increment(ref m_clientCount);
				m_workerSocketList.Add(workerSocket);

				WaitForData(workerSocket, m_clientCount);
				string msg = "Client " + m_clientCount + " Connected" + "\n";
				if(OutputClientChanged != null)
					OutputClientChanged(null,msg);
				m_mainSocket.BeginAccept(new AsyncCallback ( OnClientConnect ),null);
			}
			catch(ObjectDisposedException)
			{
				System.Diagnostics.Debugger.Log(0,"1","\n OnClientConnection: Socket has been closed\n");
			}
			catch(SocketException se)
			{
				Console.WriteLine( se.Message );
			}
			
		}

		public class SocketPacket
		{
			public SocketPacket(System.Net.Sockets.Socket socket, int clientNumber)
			{
				m_currentSocket = socket;
				m_clientNumber  = clientNumber;
			}
			public System.Net.Sockets.Socket m_currentSocket;
			public int m_clientNumber;
			public byte[] dataBuffer = new byte[1024];
		}

		private static void SendDataClient(System.Net.Sockets.Socket workerSocket){
				// Send back the reply to the client
				string replyMsg = "Server Reply:" + m_currentmessage; 
				// Convert the reply to byte array
				byte[] byData = System.Text.Encoding.ASCII.GetBytes(replyMsg);

				workerSocket.Send(byData);
		}

		public static void OnDataReceived(IAsyncResult asyn)
		{
			SocketPacket socketData = (SocketPacket)asyn.AsyncState ;
			try
			{	
				int numbytes = 0;
				numbytes = socketData.m_currentSocket.EndReceive (asyn);
				char[] chars = new char[numbytes];
				//System.Text.Decoder d = System.Text.Encoding.UTF8.GetDecoder();
				string recievedData = new String(chars);

				m_currentmessage += recievedData;

				if (recievedData.Contains("\n" ) && OutputClientChanged != null)
				{
					OutputClientChanged(null,m_currentmessage);//(spacket.Socket, node);
					m_currentmessage = "";
				}

				WaitForData(socketData.m_currentSocket, socketData.m_clientNumber );
				System.Net.Sockets.Socket workerSocket2 = ( System.Net.Sockets.Socket)socketData.m_currentSocket;
				SendDataClient(workerSocket2);
			}
			catch (ObjectDisposedException )
			{
				System.Diagnostics.Debugger.Log(0,"1","\nOnDataReceived: Socket has been closed \n");
			}
			catch(SocketException se)
			{
				if(se.ErrorCode == 10054) // Error code for Connection reset by peer
				{	
					string msg = "Client " + socketData.m_clientNumber + " Disconnected" + "\n";
					if(OutputClientChanged!=null){
						OutputClientChanged(null,msg);
					}

					m_workerSocketList[socketData.m_clientNumber - 1] = null;
				}
				else
				{
					if(OutputClientChanged!=null){
						OutputClientChanged(null,se.Message);
					}
				}
			}
		}


		static void SendMsgToClient(string msg, int clientNumber)
		{
			byte[] byData = System.Text.Encoding.ASCII.GetBytes(msg);

			System.Net.Sockets.Socket workerSocket = (System.Net.Sockets.Socket)m_workerSocketList[clientNumber - 1];
			workerSocket.Send(byData);
		}

		public static void WaitForData(System.Net.Sockets.Socket soc, int clientNumber)
		{
			try
			{
				if  ( pfnWorkerCallBack == null )
				{		
					pfnWorkerCallBack = new AsyncCallback (OnDataReceived);
				}
				SocketPacket theSocPkt = new SocketPacket (soc, clientNumber);

				soc.BeginReceive (theSocPkt.dataBuffer, 0, 
					theSocPkt.dataBuffer.Length,
					SocketFlags.None,
					pfnWorkerCallBack,
					theSocPkt);
			}
			catch(SocketException se)
			{
				Console.WriteLine(se.Message );
			}
		}

		public static bool Running {
			get{
				return isRunning;
			}
		}

		public static void CloseSockets()
		{
			if(m_mainSocket != null)
			{
				m_mainSocket.Close();
			}
			System.Net.Sockets.Socket workerSocket = null;
			for(int i = 0; i < m_workerSocketList.Count; i++)
			{
				workerSocket = (System.Net.Sockets.Socket)m_workerSocketList[i];
				if(workerSocket != null)
				{
					workerSocket.Close();
					workerSocket = null;
				}
			}
			m_currentmessage = "";
			isRunning = false;
		}
	}
}

