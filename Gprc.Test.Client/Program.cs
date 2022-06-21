// See https://aka.ms/new-console-template for more information
using Grpc.Net.Client;
using Image.Grpc.Service;

using var channel = GrpcChannel.ForAddress("https://localhost:5061");
// создаем клиента
var client = new Greeter.GreeterClient(channel);
Console.Write("Введите имя: ");
string name = Console.ReadLine();
// обмениваемся сообщениями с сервером
var reply = await client.SayHelloAsync(new HelloRequest { Name = name });
Console.WriteLine("Ответ сервера: " + reply.Message);
Console.ReadKey();