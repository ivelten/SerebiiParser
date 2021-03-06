﻿namespace SerebiiParser.CodeGenerators.Ruby

open SerebiiParser.Core
open SerebiiParser.Core.PokemonParser
open System
open System.IO
open System.Collections.Generic
open System.Threading.Tasks
open System.Globalization
open System.Text
open SerebiiParser.CodeGenerators

module PokemonCodeGenerator =
    let generateDbSeedsFromSerebii dir =
        let errors = ErrorLog()
        let pokemons = List<Pokemon>()

        let storePokemon p =
            printfn "Parsed:\n%A\n" p
            pokemons.Add p

        Parallel.ForEach([1..802], (fun i -> 
            try 
                parsePokemon i |> storePokemon
            with 
                | _ as ex -> pokemonUrl i |> errors.Add ex.Message)) |> ignore

        let savePokemons (list : IEnumerable<Pokemon>) =
            let sorted = list |> List.ofSeq |> List.sortBy (fun p -> p.number)
            let chunks = sorted |> List.chunkBySize 20

            for chunk in chunks do
                let min = chunk |> List.map (fun i -> i.number) |> List.min |> threeDigitsNumber
                let max = chunk |> List.map (fun i -> i.number) |> List.max |> threeDigitsNumber
                let fname = Path.Combine(dir, sprintf "%s_%s_pokemons.rb" min max)
                use file = File.CreateText fname
                let sb = StringBuilder()

                for p in chunk do

                    let maleGenderRatio = 
                        match p.maleGenderRatio with
                            |Some n -> sprintf "\n  male_gender_ratio: %M," n
                            |None -> ""

                    let type2 =
                        match p.type2 with
                            |Some t -> sprintf"\n  type_2: Type.find_by(name: '%A')," t
                            |None -> ""

                    let format = CultureInfo("en-US")
                    format.NumberFormat.NumberGroupSeparator <- "_"

                    let experienceGrowth = p.experienceGrowth.ToString("N0", format)

                    let ability1 = p.ability1.Replace("'", @"\'")

                    let ability2 =
                        match p.ability2 with
                            |Some a -> sprintf "\n  ability_2: Ability.find_by(name: '%s')," (a.Replace("'", @"\'"))
                            |None -> ""

                    let hiddenAbility =
                        match p.hiddenAbility with
                            |Some a -> sprintf "\n  hidden_ability: Ability.find_by(name: '%s')," (a.Replace("'", @"\'"))
                            |None -> ""

                    let name = p.name.Replace("'", @"\'")
                    let classification = p.classification.Replace("'", @"\'")
                    
                    let code = (sprintf @"Pokemon.create!(
  id: %i,
  name: '%s',
  classification: '%s',
  height: %M,
  weight: %M,
  capture_rate: %i,
  base_egg_steps: %i,%s
  experience_growth: %s,
  base_happiness: %i,
  hp: %i,
  attack: %i,
  defense: %i,
  sp_attack: %i,
  sp_defense: %i,
  speed: %i,
  type_1: Type.find_by(name: '%A'),%s
  ability_1: Ability.find_by(name: '%s'),%s%s
)

" p.number name classification p.height p.weight p.captureRate p.baseEggSteps maleGenderRatio
        experienceGrowth p.baseHappiness p.hp p.attack p.defense 
            p.spAttack p.spDefense p.speed p.type1 type2 ability1 ability2 hiddenAbility)
            
                    code |> sb.Append |> ignore

                let text = sb.ToString()
                text.Substring(0, text.Length - 1).Replace("\r", "") |> file.Write

        if pokemons.Count > 0
        then
            printfn "Saving Pokémon..."
            savePokemons pokemons
        else 
            printfn "No Pokémon parsed."

        if errors.Count > 0
        then 
            printfn "Saving errors..."
            Path.Combine(dir, "PokemonParsingErrors.log") |> errors.Save
        else 
            printfn "No errors found. Perfect parsing!"