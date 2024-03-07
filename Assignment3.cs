using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

// Written in C# using .NET 7.0
// Note:	This program intentionally does not perform exception handling
//			This will allow you to more easily diagnose issues with your connection and protocol

public static class Assignment3
{
	private static void Main()
	{
		Client();
	}

	private static void Client()
	{
		// Create a TCP stream socket local to this computer using port 7777
		var ipAddress = IPAddress.Parse("127.0.0.1");
		var remoteEp = new IPEndPoint(ipAddress, 7777);
		using var sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
		sender.Connect(remoteEp);

		// The server must initially send the client a 'hello' message upon connection
		var buffer = new byte[1024];
		var numBytesReceived = sender.Receive(buffer);
		var textReceived = Encoding.ASCII.GetString(buffer, 0, numBytesReceived);

		// If the server does not respond with the 'hello' message, terminate the program
		if (textReceived != "hello\r\n")
		{
			Console.WriteLine("Server is not ready yet");
			return;
		}

		// Loop forever to receive user input
		for (; ; )
		{
			// Allow the user to guess a number, or an empty string to quit
			Console.Write("Enter your guess (leave empty to disconnect): ");
			var input = Console.ReadLine();

			string message;
			if (string.IsNullOrWhiteSpace(input))
			{
				// If the user's input was empty, our message to the server will be a 'quit' message
				input = null;
				message = "quit\r\n";
			}
			else if (int.TryParse(input, out int guess))
			{
				// If the user entered a valid integer, our message will be a 'guess' message
				message = string.Format("guess\t{0}\r\n", input);
			}
			else
			{
				// If the user entered anything else, make them to try again
				Console.WriteLine("Invalid input");
				continue;
			}

			// Send the ASCII encoded message to the server
			var bytes = Encoding.ASCII.GetBytes(message);
			sender.Send(bytes);

			// If the user didn't enter anything, the client can safely disconnect and end the program
			if (input == null)
			{
				break;
			}

			// Receive a response from the server and decode it using ASCII encoding
			numBytesReceived = sender.Receive(buffer);
			textReceived = Encoding.ASCII.GetString(buffer, 0, numBytesReceived);

			// If the server responded with a 'correct' message, tell the user they won and stop the loop
			if (textReceived == "correct\r\n")
			{
				Console.WriteLine("Congratulations! You won!");
				break;
			}
			// The server should have responded with either a 'too-high' or 'too-low' message as a hint
			else
			{
				Console.WriteLine("Incorrect guess. Here's a hint: {0}", textReceived.TrimEnd());
			}
		}

		// Shut down the connection to the server
		sender.Shutdown(SocketShutdown.Both);
	}

	private static void Server()
	{
		throw new NotImplementedException("Create the server software by following these steps for Assignment 3");

		// Create a TCP stream socket bound to 127.0.0.1 and port 7777 to listen for incoming clients
		var ipAddress = IPAddress.Parse("127.0.0.1");
		var localEp = new IPEndPoint(ipAddress, 7777);

		using var listener = new Socket(ipAddress.AddressFamily,SocketType.Stream, ProtocolType.Tcp);
		listener.Bind(localEp);

		var buffer = new byte[1024];

		// Loop forever to listen for connections
		for (; ; )
		{
			Console.WriteLine("Waiting...");
			// Accept an incoming connection, creating a socket for that client
			using var handler = listener.Accept();
			var numBytesReceived = handler.Receive(buffer);
			// Generate a random integer from 1 to 99 (inclusive)
			var rand = new Random().Next(100);
		// Send a 'hello' message to the client
			Console.WriteLine("Hello!\r\n");
		// Loop forever to receive new messages from this client
			for(; ; ){
				Console.WriteLine("Waiting for message...");
		// 		Receive an incoming message and decode it using ASCII encoding
				var textReceived = Encoding.ASCII.GetString(buffer, 0, numBytesReceived).Parse("\t");
				Switch(textReceived[0]){
		// 		If the message is a 'guess' message, parse the client's guess, then...
					case "guess"
		// 			...If their guess matches the answer, send a 'correct' message to them and disconnect from the client
						if(TryParse(textReceived[1], out int guess) == rand) Console.WriteLine("correct\r\n");
		// 			...If their guess is greater than the answer, send a 'too-high' message and continue listening
						if(TryParse(textReceived[1], out int guess) > rand) Console.WriteLine("too-high\r\n");
		// 			...If their guess is less than the answer, send a 'too-low' message and continue listening
						if(TryParse(textReceived[1], out int guess) < rand) Console.WriteLine("too-low\r\n");
		// 		If the message is a 'quit' message, disconnect from this client and start listening again
					case "quit":
					break;
					default:
					Console.WriteLine("Unexpected Command. Try again\r\n");
				}
				
			}
			Console.WriteLine("Game Over. Thanks for playing!\r\n");
		}
	}
}
