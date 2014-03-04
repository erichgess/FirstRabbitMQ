// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.
open System.Text
open RabbitMQ.Client
open RabbitMQ.Client.Events

[<EntryPoint>]
let main argv = 
    let factory = new ConnectionFactory(HostName = "localhost")
    use connection = factory.CreateConnection()
    use channel = connection.CreateModel()

    channel.QueueDeclare( "hello", false, false, false, null )
    let consumer = new QueueingBasicConsumer(channel)
    channel.BasicConsume("hello", true, consumer)

    // I wrap the queue in a sequence expression
    let queue = seq{
                    while true do
                        let ea = consumer.Queue.Dequeue() :> BasicDeliverEventArgs
                        let body = ea.Body
                        let message = Encoding.UTF8.GetString(body)
                        yield message
                }

    // Which allows me to use queries on the queue, exactly as if it were any other
    // enumerated data source
    let qQuery = query{
                    for message in queue do
                    let i = System.Int32.Parse(message)
                    where (i%2 = 0)
                    select i
                 }
    qQuery |> Seq.iter (printfn "%d")
    printfn "%A" argv
    0 // return an integer exit code
