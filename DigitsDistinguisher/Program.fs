open System

[<EntryPoint>]
let main _ =
    printfn "Calculating..."

    let stopWatch = System.Diagnostics.Stopwatch.StartNew()

    Calculation.execute 
    |> printfn "Accuracy: %f%%"

    stopWatch.Stop()
    printfn "Completed in %f." stopWatch.Elapsed.TotalMinutes

    printfn "Press any key to exit...!"
    Console.ReadKey() |> ignore
    0 // return an integer exit code
