module SerebiiParser.Console.Program

open System

[<EntryPoint>]
let main argv = 
    
    PokemonHelper.parseSerebiiPokemons @"C:\Temp"

    Console.ReadKey() |> ignore

    0
