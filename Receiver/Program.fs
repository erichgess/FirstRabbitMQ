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
    let (readFrom,_) = createQueueFuntions myChannel

    let helloQueue = readFrom "hello"

    // I wrap the queue in a sequence expression
    //let queue = seq{
    while true do
        let message = helloQueue ()
        match message with
        | Some(s) -> printfn "%s" s
        | _ -> ()
                //}

    //queue |> Seq.iter (printfn "%A")
    printfn "%A" argv
    0 // return an integer exit code
