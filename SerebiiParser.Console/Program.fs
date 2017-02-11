module SerebiiParser.Console.Program

open System

[<EntryPoint>]
let main argv = 
    
    //PokemonHelper.parseSerebiiPokemons @"C:\Temp"
    AbilityHelper.parseSerebiiAbilities @"C:\Temp" |> ignore

    Console.ReadKey() |> ignore

    0
