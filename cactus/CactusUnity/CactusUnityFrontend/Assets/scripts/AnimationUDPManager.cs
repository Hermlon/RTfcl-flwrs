using UnityEngine;
using System.Collections;

public class AnimationUDPManager {

  private string ip;
	private int port;

  private IPEndPoint remoteEndPoint;
	private UdpClient client;

	private Thread receiveThread;

  private int MATRIX_EYEL = 0;
  private int MATRIX_EYER = 1;
  private int MATRIX_MOUTHL = 2;
  private int MATRIX_MOUTHM = 3;
  private int MATRIX_MOUTHR = 4;

  public AnimationUDPManager(string i, int p) {
    ip = i;
    port = p;
    remoteEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
		client = new UdpClient();
  }

  public void playAnimation(Animation a, int matrix) {
    // /a <filename> <length> <matrix>
    // /s <filename> <data>
    string filename = a.getName();
    string length = a.getSize().ToString();
    string matrix = matrix.ToString();
    SendMsg("/a " + filename + " " + length + " " + matrix);
  }

  public void SendMsg(string msg) {
		byte[] data = Encoding.UTF8.GetBytes(msg);
		client.Send(data, data.Length, remoteEndPoint);
		receiveThread = new Thread(new ThreadStart(ReceiveData));
		receiveThread.IsBackground = true;
		receiveThread.Start();
	}

	private void ReceiveData() {
    List<int> missingFrames = new List<>();
		while (true)
		{
			try
			{
				// Bytes empfangen.
				IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
				byte[] data = client.Receive(ref anyIP);

				// Bytes mit der UTF8-Kodierung in das Textformat kodieren.
				string text = Encoding.UTF8.GetString(data);

        //Everything received
        if(text == "e") {
          sendMissingFrames(missingFrames);
          return;
        }
        else {
          try {
            int f = Convert.ToInt32(text);
            missingFrames.Add(f);
          }
          catch(Exceprion e) {

          }
        }
			}
			catch (Exception err)
			{
				//print(err.ToString());
			}
		}
	}

  private void sendMissingFrames(List<int> mf) {
//foreach send file...
  }
}
