module SerebiiParser.Console.Program

open System
open SerebiiParser.CodeGenerators.Ruby

[<EntryPoint>]
let main argv = 
    let dir = @"C:\Temp"

    //PokemonCodeGenerator.generateDbSeedsFromSerebii dir
    AbilityCodeGenerator.generateDbSeedsFromSerebii dir

    Console.Write("Finished. Press enter to exit.")
    Console.ReadKey() |> ignore
    0
