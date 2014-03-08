// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.
open System.Text
open RabbitMQ.Client
open RabbitMQ.Client.Events
open RabbitMQ.FSharp.Client

[<EntryPoint>]
let main argv = 
    let connection = openConnection "localhost"
    let myChannel = openChannel connection

    let readFromHelloQueue = createQueueConsumer myChannel "hello"

    while true do
        let message = readFromHelloQueue()
        match message with
        | s -> printfn "%s" s
        | _ -> ()

    0 // return an integer exit code
