// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.
open RabbitMQ.Client
open System.Text

[<EntryPoint>]
let main argv = 
    let factory = new ConnectionFactory(HostName = "localhost")
    use connection = factory.CreateConnection()
    use channel = connection.CreateModel()

    channel.QueueDeclare( "hello", false, false, false, null ) |> ignore
    
    let mutable i = 0
    while true do
        i <- i + 1
        let message = sprintf "%d,test" ((i + 1) % 10)  // send a message with a number from 0 to 9 along with some text
        printfn "Sending: %s" message
        let body = Encoding.UTF8.GetBytes(message)
        channel.BasicPublish("", "hello", null, body)
        System.Threading.Thread.Sleep(2000)

    printfn "%A" argv
    0 // return an integer exit code
