// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.
open RabbitMQ.Client
open System.Text

[<EntryPoint>]
let main argv = 
    let factory = new ConnectionFactory(HostName = "localhost")
    use connection = factory.CreateConnection()
    use channel = connection.CreateModel()

    channel.QueueDeclare( "hello", false, false, false, null )
    let message = "Hello, World"
    let body = Encoding.UTF8.GetBytes(message)

    while true do
        channel.BasicPublish("", "hello", null, body)
        System.Threading.Thread.Sleep(2000)

    printfn "%A" argv
    0 // return an integer exit code
