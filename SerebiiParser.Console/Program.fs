module SerebiiParser.Console.Program

open System
open SerebiiParser.CodeGenerators.Ruby

[<EntryPoint>]
let main argv = 
    let dir = @"C:\Temp"
    PokemonCodeGenerator.generateDbSeedsFromSerebii dir
    AbilityCodeGenerator.generateDbSeedsFromSerebii dir
    Console.ReadKey() |> ignore
    0
