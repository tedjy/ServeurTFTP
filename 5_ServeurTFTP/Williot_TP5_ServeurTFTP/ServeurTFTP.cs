using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Williot_TP5_ServeurTFTP
{
    internal class ServeurTFTP
    {
        public ServeurTFTP()
        {
            int port = 69;

                IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, port ); // IP local quelconque, port =11000
                UdpClient udpCommunicator = new UdpClient(localEndPoint); // objet pour échanger
                IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0); // pour récupérer le client

                byte[] request = udpCommunicator.Receive(ref remoteEndPoint); // attend un datagramme requête
                // ANALYSE DE LA TRAME TFTP
                //OPCODE
                int opCode = request[1];  // récup du opCode
                Console.WriteLine("opCode = " + opCode);
            afficher(request);
            if (opCode == 01)
            {
                {
                    // Démarre la recherche à partir de l'index 2
                    int byteIndex = Array.IndexOf(request, (byte)0, 2);

                    // Vérifier si le byte NULL a été trouvé
                    if (byteIndex != -1)
                    {
                        // Extraire le nom du fichier du buffer de la trame
                        int index = 2;
                        int longueurFichier = byteIndex - index;
                        string fileName = Encoding.ASCII.GetString(request, index, longueurFichier);

                        Console.WriteLine("File Name: " + fileName);

                        // Lire le contenu du fichier
                        string content = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), fileName));

                        // Lire le contenu du fichier en utilisant le chemin complet
                        byte[] fileBytes = File.ReadAllBytes(Path.Combine(Directory.GetCurrentDirectory(), fileName));

                        // Construire et envoyer les trames de données
                        int blockSize = 512;
                        int blockCount = (int)Math.Ceiling((double)fileBytes.Length / blockSize);

                        for (int blockNumber = 1; blockNumber <= blockCount; blockNumber++)
                        {
                            int startIndex = (blockNumber - 1) * blockSize;
                            int endIndex = Math.Min(startIndex + blockSize, fileBytes.Length);

                            // Afficher le contenu du fichier sur la console
                            Console.WriteLine("File Content:\n" + content);

                            // Construire la trame avec le code d'opération (03 pour Data)
                            List<byte> responseList = new List<byte> { 0, 3, 0, (byte)blockNumber };
                            responseList.AddRange(fileBytes.Skip(startIndex).Take(endIndex - startIndex));

                            byte[] response = responseList.ToArray();
                            udpCommunicator.Send(response, response.Length, remoteEndPoint);
                        }

                    }
                }

            }
            if (opCode == 04)
                {
                }
            Console.ReadKey();
        }

    public void afficher(byte[] tab)
        {
            foreach (byte b in tab)
            {
                Console.Write(b + " ");
            }
            Console.WriteLine();
        }


    static void Main(string[] args)
        {
           new ServeurTFTP();  
        }
    }

}
