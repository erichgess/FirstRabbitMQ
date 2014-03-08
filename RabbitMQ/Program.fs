// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.
open RabbitMQ.Client
open System.Text
open RabbitMQ.FSharp.Client

[<EntryPoint>]
let main argv = 
    let connection = openConnection "localhost"
    let channel = openChannel connection
    
    let writeToHelloQueue = createQueueWriter channel "hello"

    let mutable i = 0
    while true do
        i <- i + 1
        let message = sprintf "%d,test" ((i + 1) % 10)  // send a message with a number from 0 to 9 along with some text
        printfn "Sending: %s" message
        message |> writeToHelloQueue
        System.Threading.Thread.Sleep(1000)

    0 // return an integer exit code
