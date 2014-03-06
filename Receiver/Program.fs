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

    channel.QueueDeclare( "hello", false, false, false, null ) |> ignore
    let consumer = new QueueingBasicConsumer(channel) 
    channel.BasicConsume("hello", true, consumer) |> ignore

    // I wrap the queue in a sequence expression
    let queue = seq{
                    while true do
                        let ea = consumer.Queue.Dequeue()
                        let body = ea.Body
                        let message = Encoding.UTF8.GetString(body)
                        yield message
                }

    let validIds = [1; 2; 3; 4]
    let idQuery = query{
                    for id in validIds do
                    select id
                  }

    // This will read all the messages coming in from RabbitMQ
    // the messages have the format "id,text"
    // this query extracts the id and then joins on the list of
    // valid Ids.  So that only messages which have a valid id will
    // be selected (the rest are all discarded).
    let qQuery = query{
                    for message in queue do
                    let id,text = message.Split(',') |> (fun s -> (System.Int32.Parse(s.[0]), s.[1]))
                    join validId in validIds on
                        (id = validId)
                    select (id,text)
                 }
    qQuery |> Seq.iter (printfn "%A")
    printfn "%A" argv
    0 // return an integer exit code
